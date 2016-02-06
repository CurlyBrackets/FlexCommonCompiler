using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.IR.Structure
{
    class BinaryExpression : Expression
    {
        public Operation Operation { get; private set; }
        public Expression Left { get; private set; }
        public Expression Right { get; private set; }

        public BinaryExpression(Operation op, Expression left, Expression right)
        {
            Operation = op;
            Left = left;
            Right = right;
        }

        public override T Accept<T>(IRExpressionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
