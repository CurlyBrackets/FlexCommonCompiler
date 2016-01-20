using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.AST.Statements
{
    class ExpressionStatement : Statement
    {
        public Expression Expression { get; private set; }
        
        public ExpressionStatement(Expression exp)
        {
            Expression = exp;
        }
    }
}
