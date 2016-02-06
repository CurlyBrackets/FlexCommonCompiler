using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.IR.Structure
{
    class Temporary : AssignableExpression
    {
        public int Index { get; private set; }

        public Temporary(int index)
        {
            Index = index;
        }

        public override T Accept<T>(IRExpressionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
