using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.Program
{
    // not strictly correct, but whatever
    abstract class Element : LValue
    {
        public ElementType Type { get; private set; }

        public Element(ElementType type)
        {
            Type = type;
        }

        public override ElementType ResolveType()
        {
            return Type;
        }
    }
}
