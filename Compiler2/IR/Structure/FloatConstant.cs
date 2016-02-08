using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.IR.Structure
{
    class FloatConstant : Expression
    {
        public double Value { get; private set; }

        public FloatConstant(double val)
        {
            Value = val;
        }

        public T Accept<T>(IRExpressionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
