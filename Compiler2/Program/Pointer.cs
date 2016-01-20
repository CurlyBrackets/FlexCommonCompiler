using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.Program
{
    class Pointer : LValue
    {
        public LValue Value { get; private set; }

        public Pointer(LValue value)
        {
            Value = value;
        }

        public override ElementType ResolveType()
        {
            return new ElementType(EElementType.Pointer)
            {
                Secondary = Value.ResolveType()
            };
        }
    }
}
