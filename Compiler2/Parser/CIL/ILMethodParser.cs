using Compiler2.AST;
using Compiler2.AST.Expressions;
using Compiler2.AST.Statements;
using Compiler2.Program;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.Parser.CIL
{
    class ILMethodParser
    {
        public byte[] ILBytes { get; private set; }
        public int Index { get; private set; }
        public InspectableAssembly Ia { get; private set; }
        public MethodSignature Signature { get; private set; }
        public IList<ElementType> LocalTypes { get; private set; }
        public Stack<Expression> Stack { get; private set; }

        public ImmutableArray<ExceptionRegion> ExceptionRegions { get; private set; }
        public int RegionIndex { get; private set; }

        private bool m_performRegionCheck;

        public ILMethodParser(InspectableAssembly ia, MethodSignature signature, MethodBodyBlock rawBody)
        {
            Ia = ia;
            Signature = signature;
            ILBytes = rawBody.GetILBytes();

            Index = 0;
            Stack = new Stack<Expression>();

            LocalTypes = new Utils.SparseList<ElementType>();

            if (!rawBody.LocalSignature.IsNil)
            {
                var local = ia.Reader.GetStandaloneSignature(rawBody.LocalSignature);
                if (!local.Signature.IsNil)
                    LocalTypes = ILReader.ParseLocalSignature(ia, ia.Reader.GetBlobBytes(local.Signature));
            }

            ExceptionRegions = rawBody.ExceptionRegions;
            RegionIndex = 0;
            m_performRegionCheck = RegionIndex < ExceptionRegions.Length;
        }

        public INode Parse()
        {
            return ParseBlock();
        }

        private Block ParseBlock(int maxIndex = int.MaxValue)
        {
            var ret = new Block();

            while (Index < ILBytes.Length && Index < maxIndex)
            {
                if(m_performRegionCheck && Index == ExceptionRegions[RegionIndex].TryOffset)
                {
                    var region = ExceptionRegions[RegionIndex++];
                    m_performRegionCheck = RegionIndex < ExceptionRegions.Length;

                    var tryStatement = new Try();
                    tryStatement.Body = ParseBlock(region.TryOffset + region.TryLength);
                    if (Index != region.HandlerOffset)
                        throw new Exception("I don't know what to do here!");

                    switch (region.Kind)
                    {
                        case ExceptionRegionKind.Finally:
                            tryStatement.Finally = ParseBlock(region.HandlerOffset + region.HandlerLength);
                            break;
                        default:
                            throw new Exception("Unhandled exception region type: " + region.Kind);
                    }

                    ret.Statements.Add(tryStatement);
                    continue;
                }

                dynamic opCode = (OpCode)ILBytes[Index++];
                if (opCode == OpCode.extended)
                    opCode = (ExtendedOpCode)ILBytes[Index++];

                Console.Write($"{opCode} ");

                ParseSingle(ret, opCode);

                Console.WriteLine();
            }

            return ret;
        }

        private void ParseSingle(Block ret, OpCode opCode)
        {
            int sumIndex = 0, val32 = 0;
            bool brInvert = false;

            Handle handle;
            MethodInvocation call;

            switch (opCode)
            {
                case OpCode.nop:
                    break;
                case OpCode.ldstr:
                    Stack.Push(
                        new Identity(
                            new Program.Constant(
                                new ElementType(EElementType.String),
                                Ia.GetString(
                                    GetInt32()))));
                    break;

                case OpCode.newobj:
                    handle = MetadataTokens.Handle(GetInt32());
                    var obj = FillCall<ObjectCreation>(handle);

                    val32 = obj.Signature.NumNormal;
                    for (int i2 = 0; i2 < val32; i2++)
                        obj.Arguments[val32 - i2] = Stack.Pop();

                    obj.Class = Ia.Context.Program.Classes[obj.ClassName];
                    Stack.Push(obj);
                    break;

                #region Calls

                case OpCode.call:
                    handle = MetadataTokens.Handle(GetInt32());
                    call = FillCall<MethodInvocation>(handle);

                    int paramCount = call.Signature.NumNormal;
                    if (!call.Signature.IsStatic && !call.Signature.Flags.HasFlag(EMethodFlags.ExplicitThis))
                        paramCount++;

                    for (int i2 = 0; i2 < paramCount; i2++)
                        call.Arguments[paramCount - i2 - 1] = Stack.Pop();

                    if (call.Signature.ReturnType.Type != EElementType.Void)
                        Stack.Push(call);
                    else
                        ret.Statements.Add(new ExpressionStatement(call));

                    break;
                /*case OpCode.callvirt:
                    handle = MetadataTokens.Handle(GetInt32());
                    call = FillCall(handle);


                    break;*/

                #endregion

                case OpCode.ret:
                    Return statement = null;
                    if (Signature.ReturnType.Type != EElementType.Void)
                    {
                        var returnExpression = Stack.Pop();
                        if (Signature.ReturnType.HasTarget)
                        {
                            //resolve type of expression
                            bool indeterminate;
                            var resolved = returnExpression.EvaluateType(out indeterminate);
                            if (indeterminate)
                            {
                                // process other methods for a type
                                Ia.ClearQueue();
                            }

                            Signature.ReturnType.ConcreteTargets.Add(resolved.Target);
                        }

                        statement = new Return(returnExpression);
                    }
                    else
                        statement = new Return();

                    ret.Statements.Add(statement);
                    break;

                #region stloc

                case OpCode.stloc_s:
                    sumIndex = ILBytes[Index++];
                    goto case OpCode.stloc_0;
                case OpCode.stloc_3:
                    sumIndex++;
                    goto case OpCode.stloc_2;
                case OpCode.stloc_2:
                    sumIndex++;
                    goto case OpCode.stloc_1;
                case OpCode.stloc_1:
                    sumIndex++;
                    goto case OpCode.stloc_0;
                case OpCode.stloc_0:
                    ret.Statements.Add(
                        new ExpressionStatement(
                            new Assignment()
                            {
                                Target = new Variable(LocalTypes[sumIndex], VariableType.Local, sumIndex),
                                Value = Stack.Pop()
                            }));
                    break;

                #endregion

                #region ldloc

                case OpCode.ldloc_s:
                    sumIndex = ILBytes[Index++];
                    goto case OpCode.ldloc_0;
                case OpCode.ldloc_3:
                    sumIndex++;
                    goto case OpCode.ldloc_2;
                case OpCode.ldloc_2:
                    sumIndex++;
                    goto case OpCode.ldloc_1;
                case OpCode.ldloc_1:
                    sumIndex++;
                    goto case OpCode.ldloc_0;
                case OpCode.ldloc_0:
                    Stack.Push(
                        new Identity(
                            new Variable(LocalTypes[sumIndex], VariableType.Local, sumIndex)));
                    break;

                #endregion

                #region ldloca

                case OpCode.ldloca_s:
                    val32 = ILBytes[Index++];
                    Stack.Push(
                        new Identity(
                            new Pointer(
                                new Variable(LocalTypes[val32], VariableType.Local, val32))));

                    break;

                #endregion

                #region ldarg

                case OpCode.ldarg_s:
                    sumIndex = ILBytes[Index++];
                    goto case OpCode.ldarg_0;
                case OpCode.ldarg_3:
                    sumIndex++;
                    goto case OpCode.ldarg_2;
                case OpCode.ldarg_2:
                    sumIndex++;
                    goto case OpCode.ldarg_1;
                case OpCode.ldarg_1:
                    sumIndex++;
                    goto case OpCode.ldarg_0;
                case OpCode.ldarg_0:
                    Stack.Push(
                        new Identity(
                            new Variable(
                                (!Signature.IsStatic && !Signature.Flags.HasFlag(EMethodFlags.ExplicitThis)) ?
                                    Signature.ThisType :
                                    Signature.ParameterTypes[sumIndex],
                                VariableType.Parameter,
                                sumIndex)));
                    break;

                #endregion

                #region ldc_i4

                case OpCode.ldc_i4_s:
                    val32 = ILBytes[Index++];
                    goto case OpCode.ldc_i4_0;
                case OpCode.ldc_i4:
                    val32 = GetInt32();
                    goto case OpCode.ldc_i4_0;
                case OpCode.ldc_i4_m1:
                    val32 = -1;
                    goto case OpCode.ldc_i4_0;
                case OpCode.ldc_i4_8:
                    val32 = 8;
                    goto case OpCode.ldc_i4_0;
                case OpCode.ldc_i4_7:
                    val32 = 7;
                    goto case OpCode.ldc_i4_0;
                case OpCode.ldc_i4_6:
                    val32 = 6;
                    goto case OpCode.ldc_i4_0;
                case OpCode.ldc_i4_5:
                    val32 = 5;
                    goto case OpCode.ldc_i4_0;
                case OpCode.ldc_i4_4:
                    val32 = 4;
                    goto case OpCode.ldc_i4_0;
                case OpCode.ldc_i4_3:
                    val32 = 3;
                    goto case OpCode.ldc_i4_0;
                case OpCode.ldc_i4_2:
                    val32 = 2;
                    goto case OpCode.ldc_i4_0;
                case OpCode.ldc_i4_1:
                    val32 = 1;
                    goto case OpCode.ldc_i4_0;
                case OpCode.ldc_i4_0:
                    Stack.Push(
                        new Identity(
                            new Program.Constant(
                                new ElementType(EElementType.Int32),
                                val32)));
                    break;

                #endregion

                #region ldc other

                case OpCode.ldc_i8:
                    Stack.Push(
                        new Identity(
                            new Program.Constant(
                                new ElementType(EElementType.Int64),
                                GetInt64())));
                    break;
                case OpCode.ldc_r4:
                    Stack.Push(
                        new Identity(
                            new Program.Constant(
                                new ElementType(EElementType.Float32),
                                GetFloat())));
                    break;
                case OpCode.ldc_r8:
                    Stack.Push(
                        new Identity(
                            new Program.Constant(
                                new ElementType(EElementType.Float64),
                                GetDouble())));
                    break;
                case OpCode.ldnull:
                    Stack.Push(
                        new Identity(
                            new Program.Constant(
                                new ElementType(EElementType.Reference),
                                null)));
                    break;

                #endregion

                #region ldind

                case OpCode.ldind_u1:
                    break;

                #endregion

                #region Static fields

                case OpCode.ldsfld:
                    var token = GetInt32();
                    var f = Ia.ParseField(MetadataTokens.FieldDefinitionHandle(token));
                    Stack.Push(
                        new Identity(
                            f));
                    break;
                case OpCode.ldsflda:
                    val32 = GetInt32();
                    Stack.Push(
                        new Identity(
                            new Pointer(
                                Ia.ParseField(MetadataTokens.FieldDefinitionHandle(val32)))));
                    break;
                /*case OpCode.stsfld:
                    break;*/

                #endregion

                case OpCode.leave:
                    val32 = GetInt32();
                    goto case OpCode.leave_common;
                case OpCode.leave_s:
                    val32 = ILBytes[Index++];
                    goto case OpCode.leave_common;
                case OpCode.leave_common:
                    ret.Statements.Add(
                        new Break()); //exits the current block or something
                    break;

                case OpCode.@throw:
                    ret.Statements.Add(new Throw() { Exception = Stack.Pop() });
                    break;

                case OpCode.pop:
                    Stack.Pop();
                    break;

                #region Compare Branching

                case OpCode.beq_s:
                    ParseSingle(ret, ExtendedOpCode.ceq);
                    ParseSingle(ret, OpCode.brtrue_s);
                    break;
                case OpCode.beq:
                    ParseSingle(ret, ExtendedOpCode.ceq);
                    ParseSingle(ret, OpCode.brtrue);
                    break;
                case OpCode.bne_un_s:
                    ParseSingle(ret, ExtendedOpCode.ceq);
                    ParseSingle(ret, OpCode.brfalse_s);
                    break;
                case OpCode.bne_un:
                    ParseSingle(ret, ExtendedOpCode.ceq);
                    ParseSingle(ret, OpCode.brfalse);
                    break;

                #region bge

                case OpCode.bge_s:
                    ParseSingle(ret, ExtendedOpCode.clt_un);
                    ParseSingle(ret, OpCode.brfalse_s);
                    break;
                case OpCode.bge:
                    ParseSingle(ret, ExtendedOpCode.clt_un);
                    ParseSingle(ret, OpCode.brfalse);
                    break;
                case OpCode.bge_un_s:
                    ParseSingle(ret, ExtendedOpCode.clt);
                    ParseSingle(ret, OpCode.brfalse_s);
                    break;
                case OpCode.bge_un:
                    ParseSingle(ret, ExtendedOpCode.clt);
                    ParseSingle(ret, OpCode.brfalse);
                    break;

                #endregion
                #region bgt

                case OpCode.bgt_s:
                    ParseSingle(ret, ExtendedOpCode.cgt);
                    ParseSingle(ret, OpCode.brtrue_s);
                    break;
                case OpCode.bgt:
                    ParseSingle(ret, ExtendedOpCode.cgt);
                    ParseSingle(ret, OpCode.brtrue);
                    break;
                case OpCode.bgt_un_s:
                    ParseSingle(ret, ExtendedOpCode.cgt_un);
                    ParseSingle(ret, OpCode.brtrue_s);
                    break;
                case OpCode.bgt_un:
                    ParseSingle(ret, ExtendedOpCode.cgt_un);
                    ParseSingle(ret, OpCode.brtrue);
                    break;

                #endregion
                #region ble

                case OpCode.ble_s:
                    ParseSingle(ret, ExtendedOpCode.cgt);
                    ParseSingle(ret, OpCode.brfalse_s);
                    break;
                case OpCode.ble:
                    ParseSingle(ret, ExtendedOpCode.cgt);
                    ParseSingle(ret, OpCode.brfalse);
                    break;
                case OpCode.ble_un_s:
                    ParseSingle(ret, ExtendedOpCode.cgt_un);
                    ParseSingle(ret, OpCode.brfalse_s);
                    break;
                case OpCode.ble_un:
                    ParseSingle(ret, ExtendedOpCode.cgt_un);
                    ParseSingle(ret, OpCode.brfalse);
                    break;

                #endregion
                #region blt

                case OpCode.blt_s:
                    ParseSingle(ret, ExtendedOpCode.clt);
                    ParseSingle(ret, OpCode.brtrue_s);
                    break;
                case OpCode.blt:
                    ParseSingle(ret, ExtendedOpCode.clt);
                    ParseSingle(ret, OpCode.brtrue);
                    break;
                case OpCode.blt_un_s:
                    ParseSingle(ret, ExtendedOpCode.clt_un);
                    ParseSingle(ret, OpCode.brtrue_s);
                    break;
                case OpCode.blt_un:
                    ParseSingle(ret, ExtendedOpCode.clt_un);
                    ParseSingle(ret, OpCode.brtrue);
                    break;

                #endregion

                #endregion

                #region Branching

                /*
                    Forward skip with conditions is likely an if statement, either to the continued current block, or to the else
                    Else can be determined if the fall through case of this statement has a mandatory branch/jump, the address of that is
                    the end of else.

                    Backwards skip generally indicates a loop of some kind. If the loop started with a mandatory branch than it is either
                    a for or while and without it then it is a do-while. I don't know how to find where the condition starts for do-while
                */

                case OpCode.brtrue_s:
                    val32 = ILBytes[Index++];
                    brInvert = true;
                    goto case OpCode.br_common;
                case OpCode.brtrue:
                    val32 = GetInt32();
                    brInvert = true;
                    goto case OpCode.br_common;
                case OpCode.brfalse_s:
                    val32 = ILBytes[Index++];
                    brInvert = false;
                    goto case OpCode.br_common;
                case OpCode.brfalse:
                    val32 = GetInt32();
                    brInvert = false;
                    goto case OpCode.br_common;

                case OpCode.br_common:
                    if (val32 > 0)
                    {
                        var branch = new Branch() { Condition = Stack.Pop() };
                        Block blockA = ParseBlock(Index + val32), blockB = null;
                        if (blockA.Statements.Last() is Jump)
                        {
                            //var tBlock = ParseBlock()
                        }
                        if (brInvert)
                        {
                            branch.False = blockA;
                            branch.True = blockB;
                        }
                        else
                        {
                            branch.True = blockA;
                            branch.False = blockB;
                        }

                        ret.Statements.Add(branch);
                    }
                    else
                        Console.WriteLine("End of loop!");
                    break;

                #endregion

                default:
                    Console.Write("Missing opcode");
                    Index += ILReader.ArgumentLength(opCode);
                    break;
            }
        }

        private void ParseSingle(Block ret, ExtendedOpCode opCode)
        {
            switch (opCode) {
                #region Compare

                case ExtendedOpCode.clt:
                case ExtendedOpCode.clt_un:
                    Stack.Push(
                        new LessThan()
                        {
                            Right = Stack.Pop(),
                            Left = Stack.Pop()
                        });
                    break;
                case ExtendedOpCode.cgt:
                case ExtendedOpCode.cgt_un:
                    Stack.Push(
                        new GreaterThan()
                        {
                            Right = Stack.Pop(),
                            Left = Stack.Pop()
                        });
                    break;
                case ExtendedOpCode.ceq:
                    Stack.Push(
                        new Equal()
                        {
                            Right = Stack.Pop(),
                            Left = Stack.Pop()
                        });
                    break;

                #endregion
                default:
                    Console.Write("Missing extended opcode");
                    Index += ILReader.ArgumentLength(opCode);
                    break;
            }
        }

        private int GetInt32()
        {
            var ret = BitConverter.ToInt32(ILBytes, Index);
            Index += sizeof(int);
            return ret;
        }

        private long GetInt64()
        {
            var ret = BitConverter.ToInt64(ILBytes, Index);
            Index += sizeof(long);
            return ret;
        }

        private float GetFloat()
        {
            var ret = BitConverter.ToSingle(ILBytes, Index);
            Index += sizeof(float);
            return ret;
        }

        private double GetDouble()
        {
            var ret = BitConverter.ToDouble(ILBytes, Index);
            Index += sizeof(double);
            return ret;
        }

        private T FillCall<T>(Handle handle) where T : MethodInvocation, new()
        {
            var ret = new T();

            switch (handle.Kind)
            {
                case HandleKind.MemberReference:
                    var memberRef = Ia.Reader.GetMemberReference((MemberReferenceHandle)handle);
                    ret.MethodName = Ia.Reader.GetString(memberRef.Name);
                    ret.Signature = ILReader.ParseMethodSignature(Ia, Ia.Reader.GetBlobBytes(memberRef.Signature));

                    if (ret.Signature.IsStatic)
                    {
                        switch (memberRef.Parent.Kind)
                        {
                            case HandleKind.TypeDefinition:
                                var parent = Ia.Reader.GetTypeDefinition((TypeDefinitionHandle)memberRef.Parent);
                                ret.ClassName = Ia.GetFullyQualifiedName(parent);
                                break;
                            case HandleKind.TypeReference:
                                var parent2 = Ia.Reader.GetTypeReference((TypeReferenceHandle)memberRef.Parent);
                                ret.ClassName = Ia.GetFullyQualifiedName(parent2);
                                break;
                            default:
                                Console.WriteLine("Unknown MemberReference method parent type: " + memberRef.Parent.Kind);
                                break;
                        }
                    }

                    // resolve type immediately as necessary
                    if (ret.Signature.ReturnType.HasTarget)
                    {
                        ret.Signature.ReturnType.Target = Ia.ForceResolveType(memberRef);
                    }
                    // otherwise it can be parsed lazily
                    else
                    {
                        Ia.Context.AddTodo(Ia.CreateParseTarget(memberRef));
                    }
                    break;
                case HandleKind.MethodDefinition:
                    var m = Ia.ParseMethodHeader((MethodDefinitionHandle)handle);
                    ret.Signature = m.Signature;
                    ret.MethodName = m.Name;

                    // if the method is static, the classname can be determined now
                    if (ret.Signature.IsStatic || ret.MethodName == ".ctor")
                    {
                        var mDef = Ia.Reader.GetMethodDefinition((MethodDefinitionHandle)handle);
                        var parent = Ia.Reader.GetTypeDefinition(mDef.GetDeclaringType());
                        ret.ClassName = Ia.GetFullyQualifiedName(parent);
                    }
                    break;
                case HandleKind.MethodSpecification:
                    var spec = Ia.Reader.GetMethodSpecification((MethodSpecificationHandle)handle);
                    var genericSigBlob = Ia.Reader.GetBlobBytes(spec.Signature);
                    var m2 = Ia.ParseMethodHeader((MethodDefinitionHandle)spec.Method);
                    if (!m2.IsGeneric)
                        throw new Exception("Uhhhh...");

                    ret.Signature = m2.Signature;
                    ret.MethodName = m2.Name;
                    m2.GenericInstantiations.Add(ILReader.ParseGenericSignature(Ia, genericSigBlob));

                    // if the method is static, the classname can be determined now
                    if (ret.Signature.IsStatic || ret.MethodName == ".ctor")
                    {
                        var mDef = Ia.Reader.GetMethodDefinition((MethodDefinitionHandle)spec.Method);
                        var parent = Ia.Reader.GetTypeDefinition(mDef.GetDeclaringType());
                        ret.ClassName = Ia.GetFullyQualifiedName(parent);
                    }
                    break;
                case HandleKind.MethodImplementation:
                    throw new Exception("Method Impl");
            }

            return ret;
        }
    }
}
