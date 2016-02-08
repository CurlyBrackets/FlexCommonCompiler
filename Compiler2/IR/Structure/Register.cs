using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.IR.Structure
{
    class Register : AssignableExpression, IMemoryExpression
    {
        public RegisterType Type { get; private set; }
        public int Index { get; private set; }

        public Register(RegisterType type, int index = 0)
        {
            Type = type;
            Index = index;
        }

        public E Accept<E, S>(IRVisitor<E, S> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
