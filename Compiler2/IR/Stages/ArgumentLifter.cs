using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.IR.Stages
{
    /// <summary>
    /// Lifts excess arguments from calls into the stack
    /// </summary>
    /// <remarks>This should happen before the stack allocator</remarks>
    class ArgumentLifter : CompileStage<IRProgram, IRProgram>
    {
        public ArgumentLifter(CompilerSettings settings)
            : base(settings)
        {

        }

        protected override IRProgram ProcessCore(IRProgram input)
        {
            throw new NotImplementedException();
        }
    }
}
