using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.Program
{
    class IndirectValue : LValue
    {
        public ElementType Type { get; private set; }

        public IndirectValue(ElementType type)
        {
            Type = type;
        }

        public static LValue FromLValue(LValue val)
        {
            if(val is Pointer)
            {
                var p = val as Pointer;
                return p.Value;
            }

            return null;
        }

        public override ElementType ResolveType()
        {
            throw new NotImplementedException();
        }
    }
}
