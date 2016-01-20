using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using Compiler2.Program;
using System.Reflection.Metadata;

namespace Compiler2.Parser.CIL
{
    class ParseTarget
    {
        public InspectableAssembly Ia
        {
            get; private set;
        }

        public string AssemblyName
        {
            get; private set;
        }

        public Version AssemblyVersion
        {
            get; private set;
        }

        public string ClassName
        {
            get; private set;
        }

        public bool SpecificMethod
        {
            get
            {
                return !string.IsNullOrEmpty(MethodName) || Method != null;
            }
        }

        public string MethodName
        {
            get; set;
        }

        public MethodSignature MethodSignature
        {
            get; set;
        }

        public Method Method
        {
            get; set;
        }

        private static readonly string[] Extensions = new string[] { ".dll", ".exe" };

        #region Find Constructors

        public ParseTarget(ParseContext context, string assemblyName, string fqName)
            : this(context, assemblyName, new Version(9,9,9,9), fqName)
        {

        }

        public ParseTarget(ParseContext context, string assemblyName, Version assemblyVersion, string fqName)
        {
            AssemblyName = assemblyName;
            ClassName = fqName;
            AssemblyVersion = assemblyVersion;

            for(int i = 0; i < Extensions.Length; i++)
            {
                try
                {
                    var path = TryExtenstion(Extensions[i]);
                    Ia = new InspectableAssembly(path, context);
                }
                catch
                {
                    if (i == Extensions.Length - 1)
                        throw;
                }
            }
        }

        public ParseTarget(InspectableAssembly ia, string fqName)
        {
            AssemblyName = Path.GetFileNameWithoutExtension(ia.Stream.Name);
            ClassName = fqName;
            AssemblyVersion = new Version();
            Ia = ia;
        }

        #endregion

        #region Delay constructors

        public ParseTarget(InspectableAssembly ia, Method m)
        {
            Ia = ia;
            Method = m;

            MethodSignature = m.Signature;
            MethodName = m.Name;

            ClassName = 
                ia.GetFullyQualifiedName(
                    ia.Reader.GetTypeDefinition(
                        ((MethodDefinition)m.SourceHandle).GetDeclaringType()));
        }

        #endregion

        private string TryExtenstion(string ext)
        {
            string filename = AssemblyName + ext;
            string path;

            if (File.Exists(filename) && VersionGood(filename))
                path = filename;
            else
            {
                var asm = Assembly.Load(AssemblyName);
                if (VersionGood(asm.Location))
                    path = asm.Location;
                else
                    throw new Exception("Invalid assembly");
            }
            return path;
        }

        private bool VersionGood(string filename)
        {
            return AssemblyVersion == new Version(9, 9, 9, 9) || Assembly.LoadFile(filename).GetName().Version == AssemblyVersion;
        }

        public override bool Equals(object obj)
        {
            var b = obj as ParseTarget;
            if (b == null)
                return false;

            if(AssemblyName != b.AssemblyName || ClassName != b.ClassName || SpecificMethod != b.SpecificMethod)
                return false;

            if (SpecificMethod)
            {
                if (MethodName != b.MethodName || MethodSignature != b.MethodSignature)
                    return false;
            }

            return true;
        }

        public static bool operator ==(ParseTarget a, ParseTarget b)
        {
            if (ReferenceEquals(a, b))
                return true;

            return a.Equals(b);
        }

        public static bool operator !=(ParseTarget a, ParseTarget b)
        {
            return !(a == b);
        }

        public void Process()
        {
            var typeHandle = Ia.Reader.GetTypeDefintionByName(ClassName);
            if (SpecificMethod)
            {
                if (Method != null)
                    Ia.ParseMethodBody(Method);
                else
                    Ia.ParseClass(typeHandle, MethodName, MethodSignature);
            }
            else
                Ia.ParseClass(typeHandle);
        }
    }
}
