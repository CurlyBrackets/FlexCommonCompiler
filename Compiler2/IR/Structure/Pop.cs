using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.IR.Structure
{
    class Pop : Expression
    {
        public Pop()
        {

        }

        public T Accept<T>(IRExpressionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
