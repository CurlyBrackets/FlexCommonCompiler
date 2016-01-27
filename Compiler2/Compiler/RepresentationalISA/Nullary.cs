using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.Compiler.RepresentationalISA
{
    class NullaryOperation<T> : RepresentationalBase<T>
    {
        public NullaryOperation(T op)
            : base(op)
        {

        }

        public override T2 Accept<T2>(IRepresentationalVisitor<T, T2> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
