using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.Compiler.RepresentationalISA
{
    class UnaryOperation<T> : RepresentationalBase<T>
    {
        public Operand Target { get; private set; }

        public UnaryOperation(T op, Operand target)
            : base(op)
        {
            Target = target;
        }

        public override T2 Accept<T2>(IRepresentationalVisitor<T, T2> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
