using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.IR.Structure
{
    class Call : Expression
    {
        public string Target { get; private set; }
        public IList<Expression> Arguments { get; private set; }

        public Call(string target, params Expression[] args)
        {
            Target = target;
            Arguments = args;
        }

        public Call(string target, IList<Expression> args)
        {
            Target = target;
            Arguments = args;
        }

        public T Accept<T>(IRExpressionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
