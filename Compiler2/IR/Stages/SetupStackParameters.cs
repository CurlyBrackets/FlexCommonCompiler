using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.IR.Stages
{
    class SetupStackParameters : CompileStage<IRProgram, IRProgram>
    {
        public SetupStackParameters(CompilerSettings settings)
            : base(settings)
        {

        }

        protected override IRProgram ProcessCore(IRProgram input)
        {
            if (Settings.ExecutableType == ExecutableType.PortableExecutable)
            {
                if (Settings.Is64Bit)
                {
                    // Windows x86_64
                    input.ArgCount = 4;
                }
                else
                {
                    // stdcall maybe?
                    input.ArgCount = 0;
                }
            }
            else
            {
                if (Settings.Is64Bit)
                {
                    // System V ABI
                    input.ArgCount = 6;
                }
                else
                {
                    input.ArgCount = 0;
                }
            }

            if (Settings.Is64Bit)
            {
                input.SlotSize = 8;
            }
            else
            {
                input.SlotSize = 4;
            }

            if(Settings.ISA == ISA.x86 &&
               Settings.ExecutableType == ExecutableType.PortableExecutable &&
               Settings.Is64Bit)
            {
                input.ShadowSpace = 32;
            }
            else
            {
                input.ShadowSpace = 0;
            }

            return input;
        }
    }
}
