using Compiler2.Program;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.AST.Expressions
{
    class ObjectCreation : MethodInvocation
    {
        public Class Class { get; set; }

        public override ElementType EvaluateType(out bool indeterminate)
        {
            indeterminate = false;
            return new ElementType(EElementType.Class) { Target = Class };
        }
    }
}
