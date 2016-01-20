using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.Program
{
    class ElementType
    {
        public static readonly ElementType Default = new ElementType(EElementType.Object);

        public EElementType Type { get; private set; }
        public ElementType Secondary { get; set; }

        public int Number { get; set; } // mvar index, possibly more

        private Class m_target = null;
        public Class Target
        {
            get
            {
                return m_target;
            }
            set
            {
                m_target = value;
                if (ConcreteTargets == null)
                    ConcreteTargets = new List<Class>();
                else
                    ConcreteTargets.Clear(); //TODO: Check this behaviour
            }
        }
        public IList<Class> ConcreteTargets { get; private set; }

        public bool HasTarget
        {
            get
            {
                return Type == EElementType.Class || Type == EElementType.ValueType;
            }
        }

        public bool HasSecondary
        {
            get
            {
                return Type == EElementType.SzArray || Type == EElementType.Reference;
            }
        }

        public ElementType(EElementType type)
        {
            Type = type;
        }

        public override bool Equals(object obj)
        {
            var b = obj as ElementType;
            if (b == null)
                return false;

            if (Type != b.Type)
                return false;

            // Check target only if the the item is a class or struct
            if (HasTarget && Target != b.Target)
                return false;

            return true;
        }

        public static bool operator ==(ElementType a, ElementType b)
        {
            if (ReferenceEquals(a, b))
                return true;

            return a.Equals(b);
        }

        public static bool operator !=(ElementType a, ElementType b)
        {
            return !(a == b);
        }

        public override string ToString()
        {
            if (HasTarget)
                return $"{Type} {Target.Name}";
            else if (Type == EElementType.SzArray)
                return $"{Secondary}[]";
            else if (Type == EElementType.MVar)
                return $"<T{Number}>";
            else if (Type == EElementType.Reference)
                return $"{Secondary}&";

            return Type.ToString();
        }
    }
}
