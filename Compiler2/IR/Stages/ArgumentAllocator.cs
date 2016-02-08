using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.IR.Stages
{
    class ArgumentAllocator : CompileStage<IRProgram, IRProgram>
    {
        public ArgumentAllocator(CompilerSettings settings)
            : base(settings)
        {

        }

        protected override IRProgram ProcessCore(IRProgram input)
        {
            throw new NotImplementedException();
        }
    }
}
