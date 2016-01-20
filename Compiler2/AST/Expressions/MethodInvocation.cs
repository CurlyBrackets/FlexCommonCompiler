using Compiler2.Program;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.AST.Expressions
{
    class MethodInvocation : Expression
    {
        public MethodSignature Signature { get; set; }
        public string ClassName { get; set; }
        public string MethodName { get; set; }

        public IList<Expression> Arguments { get; private set; }

        public MethodInvocation()
        {
            Arguments = new Utils.SparseList<Expression>();
        }
    }
}
