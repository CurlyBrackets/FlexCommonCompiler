using Compiler2.Program;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.AST.Expressions
{
    class Assignment : Expression
    {
        public LValue Target { get; set; }
        public Expression Value { get; set; }

        public Assignment()
        {

        }

        public override ElementType EvaluateType(out bool indeterminate)
        {
            return Value.EvaluateType(out indeterminate);
        }
    }
}
