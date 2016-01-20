using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.AST.Statements
{
    class Return : Statement
    {
        public Expression Value { get; private set; }

        public Return(Expression value = null)
        {
            Value = value;
        }
    }
}
