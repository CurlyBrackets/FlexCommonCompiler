using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.IR.Structure
{
    class ExpressionStatement : Statement
    {
        public Expression Expression { get; private set; }

        public ExpressionStatement(Expression exp)
        {
            Expression = exp;
        }

        public override S Accept<E, S>(IRVisitor<E, S> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
