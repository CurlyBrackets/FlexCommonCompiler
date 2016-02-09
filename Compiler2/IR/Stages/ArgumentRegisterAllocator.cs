using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.IR.Stages
{
    class ArgumentRegisterAllocatorVisitor : IRDefaultVisitor
    {

    }

    class ArgumentRegisterAllocator : CompileStage<IRProgram, IRProgram>
    {
        public ArgumentRegisterAllocator(CompilerSettings settings)
            : base(settings)
        {

        }

        protected override IRProgram ProcessCore(IRProgram input)
        {
            throw new NotImplementedException();
        }
    }
}
