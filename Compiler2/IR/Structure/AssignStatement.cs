using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.IR.Structure
{
    class AssignStatement : Statement
    {
        public AssignableExpression Destination { get; private set; }
        public Expression Source { get; private set; }

        public AssignStatement(AssignableExpression dest, Expression src)
        {
            Destination = dest;
            Source = src;
        }

        public override T Accept<T>(IRStatementVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
