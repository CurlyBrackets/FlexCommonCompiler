using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.IR.Structure
{
    class UnaryExpression : Expression
    {
        public Operation Operation { get; private set; }
        public Expression Expression { get; private set; }

        public UnaryExpression(Operation op, Expression exp)
        {
            Operation = op;
            Expression = exp;
        }
    }
}
