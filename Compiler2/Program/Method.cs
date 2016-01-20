using Compiler2.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.Program
{
    class Method
    {
        public string Name { get; private set; }

        public bool Static { get; set; }
        public bool InternalCall { get; set; }
        public bool PInvoke { get; set; }

        public MethodSignature Signature { get; set; }

        public byte[] ILBytes { get; set; }
        public INode Body { get; set; }
        public object SourceHandle { get; set; }

        public bool IsGeneric
        {
            get
            {
                return Signature.Flags.HasFlag(EMethodFlags.Generic);
            }
        }

        public IList<GenericSignature> GenericInstantiations { get; private set; }

        public Method(string name)
        {
            Name = name;
            GenericInstantiations = new List<GenericSignature>();
        }

        public override bool Equals(object obj)
        {
            var b = obj as Method;
            if (b == null)
                return false;

            if (Name != b.Name || ILBytes.Length != b.ILBytes.Length)
                return false;

            for (int i = 0; i < ILBytes.Length; i++)
                if (ILBytes[i] != b.ILBytes[i])
                    return false;

            return true;
        }

        public static bool operator ==(Method a, Method b)
        {
            if (ReferenceEquals(a, b))
                return true;

            return a.Equals(b);
        }

        public static bool operator !=(Method a, Method b)
        {
            return !(a == b);
        }

        public override string ToString()
        {
            return $"{Name}{Signature.ToString()}";
        }
    }
}
