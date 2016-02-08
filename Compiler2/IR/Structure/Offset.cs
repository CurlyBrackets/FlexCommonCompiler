using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.IR.Structure
{
    class Offset : IMemoryExpression
    {
        public Register Base { get; private set; }
        public Constant Displacement { get; private set; }

        public Offset(Register @base, Constant displacement)
        {
            Base = @base;
            Displacement = displacement;
        }

        public E Accept<E, S>(IRVisitor<E, S> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
