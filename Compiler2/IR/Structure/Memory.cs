using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.IR.Structure
{
    class Memory : AssignableExpression
    {
        public IMemoryExpression Target { get; private set; }

        public Memory(IMemoryExpression target)
        {
            Target = target;
        }

        public T Accept<T>(IRExpressionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
