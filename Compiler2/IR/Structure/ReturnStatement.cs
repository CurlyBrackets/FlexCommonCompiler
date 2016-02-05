using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.IR.Structure
{
    class ReturnStatement : Statement
    {
        public Expression Value { get; private set; }

        public ReturnStatement(Expression value)
        {
            Value = value;
        }

        public ReturnStatement()
        {
            Value = null;
        }
    }
}
