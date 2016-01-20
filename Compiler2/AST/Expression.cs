using Compiler2.Program;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.AST
{
    abstract class Expression : INode
    {
        public ElementType EvaluateType()
        {
            bool ignore;
            return EvaluateType(out ignore);
        }

        public virtual ElementType EvaluateType(out bool indeterminate)
        {
            indeterminate = false;
            return ElementType.Default;
        }
    }
}
