using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.Program
{
    class MethodSignature
    {
        public ElementType ReturnType { get; set; }
        public IList<ElementType> ParameterTypes { get; private set; }
        public EMethodFlags Flags { get; set; }
        public ElementType ThisType { get; set; }

        public int NumGeneric { get; set; }
        public int NumNormal { get; set; }

        public bool IsStatic
        {
            get
            {
                return !Flags.HasFlag(EMethodFlags.HasThis);
            }
        }

        public MethodSignature()
        {
            ParameterTypes = new List<ElementType>();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append("(");
            for (int i = 0; i < ParameterTypes.Count; i++)
            {
                sb.Append(ParameterTypes[i]);
                if (i < ParameterTypes.Count - 1)
                    sb.Append(", ");
            }
            sb.Append(") -> ");
            sb.Append(ReturnType);

            return sb.ToString();
        }

        public override bool Equals(object obj)
        {
            var b = obj as MethodSignature;
            if (b == null)
                return false;

            return
                (ReturnType == b.ReturnType) &&
                (Flags == b.Flags) &&
                (NumGeneric == b.NumGeneric) &&
                (NumNormal == b.NumNormal) &&
                (ParameterTypes.SequenceEqual(b.ParameterTypes));
        }

        public static bool operator ==(MethodSignature a, MethodSignature b)
        {
            if (ReferenceEquals(a, b))
                return true;

            return a.Equals(b);
        }

        public static bool operator !=(MethodSignature a, MethodSignature b)
        {
            return !(a == b);
        }

        public MethodSignature Clone()
        {
            var ret = new MethodSignature()
            {
                ReturnType = ReturnType,
                Flags = Flags,
                ThisType = ThisType,
                NumGeneric = NumGeneric,
                NumNormal = NumNormal,
                ParameterTypes = new List<ElementType>()
            };

            foreach (var type in ParameterTypes)
                ret.ParameterTypes.Add(type);

            return ret;
        }

    }
}
