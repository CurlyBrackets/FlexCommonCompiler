using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compiler2.Program;

namespace Compiler2.AST.Expressions
{
    abstract class BooleanExpression : Expression
    {
        public Expression Left { get; set; }
        public Expression Right { get; set; }

        public override ElementType EvaluateType(out bool indeterminate)
        {
            indeterminate = false;
            return new ElementType(EElementType.Bool);
        }
    }
}
