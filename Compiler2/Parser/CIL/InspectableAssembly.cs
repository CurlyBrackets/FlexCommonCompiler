using Compiler2.Program;
using Compiler2.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.Parser.CIL
{
    class InspectableAssembly : IDisposable
    {
        public FileStream Stream { get; private set; }
        public PEReader Pe { get; private set; }
        public MetadataReader Reader { get; private set; }

        public ParseContext Context { get; private set; }

        public InspectableAssembly(string filename, ParseContext context)
        {
            Stream = File.OpenRead(filename);
            Pe = new PEReader(Stream);
            Reader = Pe.GetMetadataReader();
            Context = context;
        }

        public void Dispose()
        {
            Reader = null;
            Pe.Dispose();
            Stream.Dispose();
        }

        #region Fetching

        public Class GetClassImmediate(int token)
        {
            var ret = Context.GetClass(token);
            if (ret != null)
                return ret;

            ParseClass(MetadataTokens.TypeDefinitionHandle(token), null);
            return Context.GetClass(token);
        }

        public string GetString(int token)
        {
            var handle = MetadataTokens.Handle(token);
            if (handle.Kind == HandleKind.UserString)
                return Reader.GetUserString((UserStringHandle)handle);
            else if (handle.Kind == HandleKind.String)
                return Reader.GetString((StringHandle)handle);

            throw new Exception("Token doesn't resolve to string");
        }

        public string GetFullyQualifiedName(TypeDefinition type)
        {
            return GetFullyQualifiedNameCore(type.Namespace, type.Name);
        }

        public string GetFullyQualifiedName(TypeReference type)
        {
            return GetFullyQualifiedNameCore(type.Namespace, type.Name);
        }

        private string GetFullyQualifiedNameCore(StringHandle ns, StringHandle name)
        {
            return string.Join(".",
                    Reader.GetString(ns),
                    Reader.GetString(name));
        }

        #endregion

        #region Parse Target

        public ParseTarget CreateParseTarget(int metadataToken)
        {
            var handleType = MetadataTokens.Handle(metadataToken).Kind;
            if (handleType == HandleKind.MemberReference)
                return CreateParseTarget(Reader.GetMemberReference(MetadataTokens.MemberReferenceHandle(metadataToken)));
            else if (handleType == HandleKind.MethodDefinition)
                return CreateParseTarget(Reader.GetMethodDefinition(MetadataTokens.MethodDefinitionHandle(metadataToken)));
            else if (handleType == HandleKind.MethodSpecification)
            {
                var spec = Reader.GetMethodSpecification(MetadataTokens.MethodSpecificationHandle(metadataToken));
                //return CreateParseTarget(Reader.GetMethodDefinition(MetadataTokens.MethodDefinitionHandle(metadataToken)));
                return null;
            }

            throw new ArgumentException("Metadata Token can't be resolved");
        }

        public ParseTarget CreateParseTarget(TypeDefinitionHandle typeHandle)
        {
            var type = Reader.GetTypeDefinition(typeHandle);
            return new ParseTarget(this, GetFullyQualifiedName(type));
        }

        public ParseTarget CreateParseTarget(MemberReference memberRef)
        {

            if (memberRef.GetKind() != MemberReferenceKind.Method)
                throw new ArgumentException("Metadata Token does not refer to a method");

            var methodName = Reader.GetString(memberRef.Name);
            if (memberRef.Parent.Kind != HandleKind.TypeReference)
                throw new ArgumentException("Owner of member is not a reference to a type");

            var sig = ILReader.ParseMethodSignature(this, Reader.GetBlobBytes(memberRef.Signature));

#if DEBUG_SIG
            Console.WriteLine("=== Real Sig ===");
            foreach (var b in sig)
                Console.Write("{0:X2} ", b);
            Console.WriteLine();
#endif

            var parent = Reader.GetTypeReference((TypeReferenceHandle)memberRef.Parent);
            if (parent.ResolutionScope.Kind != HandleKind.AssemblyReference)
                throw new ArgumentException("Owner of members resolution scope is not an assembly");

            var parentFqName = GetFullyQualifiedName(parent);
            var parentScope = Reader.GetAssemblyReference((AssemblyReferenceHandle)parent.ResolutionScope);

            var assemblyName = Reader.GetString(parentScope.Name);

            return new ParseTarget(Context, assemblyName, parentScope.Version, parentFqName)
            {
                MethodName = methodName,
                MethodSignature = sig
            };
        }

        public ParseTarget CreateParseTarget(MethodDefinition methodDef)
        {
            var ret = CreateParseTarget(methodDef.GetDeclaringType());
            ret.MethodName = Reader.GetString(methodDef.Name);
            ret.MethodSignature = ILReader.ParseMethodSignature(this, Reader.GetBlobBytes(methodDef.Signature));
            return ret;
        }

        /// <summary>
        /// Create a parse target assuming that only the body needs to be parsed
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public ParseTarget CreateParseTarget(Method m)
        {
            return new ParseTarget(this, m);
        }

        #endregion

        #region Parse Components

        public void ParseClass(TypeDefinitionHandle typeHandle, string methodName = "", MethodSignature signature = null)
        {
            var type = Reader.GetTypeDefinition(typeHandle);
            var typeName = Reader.GetString(type.Name);

            if (CilParser.IgnoreClassList.Contains(typeName))
                return;

            var fqName = GetFullyQualifiedName(type);
            Class clazz;
            if (Context.Program.Classes.ContainsKey(fqName))
                clazz = Context.Program.Classes[fqName];
            else
            {
                clazz = new Class(fqName);
                Context.AddClass(this, typeHandle, clazz);
            }

            //Console.WriteLine("=== Class: " + fqName + " ===");
            if (methodName != null)
            {
                ParseMethods(type.GetMethods(), methodName, signature);
            }
        }

        public void ParseMethods(MethodDefinitionHandleCollection methods, string name, MethodSignature signature)
        {
            foreach (var methodHandle in methods)
            {
                var method = Reader.GetMethodDefinition(methodHandle);
                var methodName = Reader.GetString(method.Name);

                if (name == string.Empty || name == methodName)
                {

                    if (signature != null)
                    {
                        var sig = ILReader.ParseMethodSignature(this, Reader.GetBlobBytes(method.Signature));
#if DEBUG_SIG
                        Console.WriteLine("=== Sig ===");
                        foreach (var b in sig)
                            Console.Write("{0:X2} ", b);
                        Console.WriteLine();
#endif
                        if (sig != signature)
                            continue;
                    }

                    var m = ParseMethodHeader(methodHandle, false);
                    ParseMethodBody(m);
                }
            }
        }

        public Method ParseMethodHeader(MethodDefinitionHandle handle, bool addToQueue = true)
        {
            var method = Reader.GetMethodDefinition(handle);
            var name = Reader.GetString(method.Name);
            var signature = ILReader.ParseMethodSignature(this, Reader.GetBlobBytes(method.Signature));

            var fullname = name + signature;
            var existing = Find(fullname, method.GetDeclaringType());
            if (existing != null)
                return existing;

            var newMethod = new Method(name)
            {
                Signature = signature,
                SourceHandle = method,
                Static = method.Attributes.HasFlag(MethodAttributes.Static),
                InternalCall = method.ImplAttributes.HasFlag(MethodImplAttributes.InternalCall),
                PInvoke = method.Attributes.HasFlag(MethodAttributes.PinvokeImpl)
            };        

            AddMethodToClass(newMethod, method.GetDeclaringType());

            if (IsEntryPoint(newMethod))
                Context.Program.EntryPoint = newMethod;

            if (signature.ReturnType.HasTarget)
            {
                Console.WriteLine("!! Force resolve !!");
                signature.ReturnType.Target = ForceResolveType(newMethod);
            }
            else
            {
                if (addToQueue)
                    Context.AddTodo(CreateParseTarget(newMethod));
            }

            return newMethod;
        }

        private Method Find(string fullName, TypeDefinitionHandle typeHandle)
        {
            int token = MetadataTokens.GetToken(typeHandle);
            var clazz = Context.GetClass(token);
            if (clazz == null)
                return null;

            if (clazz.Methods.ContainsKey(fullName))
                return clazz.Methods[fullName];

            return null;
        }
    
        private void AddMethodToClass(Method m, TypeDefinitionHandle typeHandle)
        {
            int token = MetadataTokens.GetToken(typeHandle);
            var clazz = Context.GetClass(token);
            if(clazz == null)
            {
                ParseClass(typeHandle, null);
                clazz = Context.GetClass(token);
            }

            clazz.Methods.Add(m.Name + m.Signature, m);
        }

        public void ParseMethodBody(Method method)
        {
            if (method.InternalCall)
            {
                // fetch internal call ast
                Console.WriteLine("Internal call: " + method.Name);
            }
            else if (method.PInvoke)
            {
                Console.WriteLine("PInvoke: " + method.Name);
            }
            else
            {
                Console.WriteLine("=== Method: " + method.Name + method.Signature + " ===");
                method.Body = ILReader.Read(this, method.Signature, Pe.GetMethodBody(((MethodDefinition)method.SourceHandle).RelativeVirtualAddress));
            }            
        }

        private bool IsEntryPoint(Method method)
        {
            return Context.Program.EntryPoint == null &&
                    method.Name == "Main" &&
                    method.Static;
        }

        public Field ParseField(FieldDefinitionHandle handle)
        {
            var ret = new Field();

            var fieldDef = Reader.GetFieldDefinition(handle);
            ret.Signature = ILReader.ParseFieldSignature(this, Reader.GetBlobBytes(fieldDef.Signature));
            ret.Name = Reader.GetString(fieldDef.Name);

            var defaultHandle = fieldDef.GetDefaultValue();
            if (defaultHandle.IsNil)
                ret.DefaultValue = new Program.Constant(ret.Signature.Type, null);
            else
                ret.DefaultValue = ParseConstant(fieldDef.GetDefaultValue());

            var parent = GetClassImmediate(MetadataTokens.GetToken(fieldDef.GetDeclaringType()));
            if (fieldDef.Attributes.HasFlag(FieldAttributes.Static))
                parent.StaticFields.AddUnique(ret.Name, ret);
            else
                parent.InstanceFields.AddUnique(ret.Name, ret);

            return ret;
        }

        #region Constant type map

        private static readonly IDictionary<ConstantTypeCode, EElementType> ConstantTypeMap =
            new Dictionary<ConstantTypeCode, EElementType>()
            {
                [ConstantTypeCode.Boolean] = EElementType.Bool,
                [ConstantTypeCode.Char] = EElementType.Char,
                [ConstantTypeCode.SByte] = EElementType.SignedByte,
                [ConstantTypeCode.Byte] = EElementType.Byte,
                [ConstantTypeCode.Int16] = EElementType.Int16,
                [ConstantTypeCode.UInt16] = EElementType.UInt16,
                [ConstantTypeCode.Int32] = EElementType.Int32,
                [ConstantTypeCode.UInt32] = EElementType.UInt32,
                [ConstantTypeCode.Int64] = EElementType.Int64,
                [ConstantTypeCode.UInt64] = EElementType.UInt64,
                [ConstantTypeCode.Single] = EElementType.Float32,
                [ConstantTypeCode.Double] = EElementType.Float64,
                [ConstantTypeCode.String] = EElementType.String,
                [ConstantTypeCode.NullReference] = EElementType.Class
            };

        #endregion

        public Program.Constant ParseConstant(ConstantHandle handle)
        {
            var temp = Reader.GetConstant(handle);
            var type = ConstantTypeMap[temp.TypeCode];

            var val = Reader.GetBlobBytes(temp.Value);

            return new Program.Constant(new ElementType(type), null);
        }

        #endregion

        #region Force Resolution

        public Class ForceResolveType(Method method)
        {
            return ForceResolveTypeCore(CreateParseTarget(method));
        }

        public Class ForceResolveType(MemberReference memberRef)
        {
            var target = CreateParseTarget(memberRef);
            return ForceResolveTypeCore(target);
        }

        private Class ForceResolveTypeCore(ParseTarget target)
        {
            Console.WriteLine("!! Force resolve !!");

            target.Process();
            var m = Context.Program.Classes[target.ClassName].Methods[target.MethodName + target.MethodSignature];
            return m.Signature.ReturnType.Target;
        }

        public void ClearQueue()
        {
            int baseCount = Context.Todo.Count;
            for(; baseCount > 0; baseCount--)
            {
                Context.Todo.Dequeue().Process();
            }
        }

        #endregion
    }
}
