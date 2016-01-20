using Compiler2.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.Compiler
{
    class PhysicalAddresser : CompileStage<FinalProgram<byte>, FinalProgram<byte>>
    {
        public PhysicalAddresser(CompilerSettings settings)
            : base(settings)
        {

        }

        private long GetBaseAddress()
        {
            switch (Settings.ExecutableType)
            {
                case ExecutableType.PortableExecutable:
                    return 0x400;
                case ExecutableType.ExecutableAndLinkableFormat:
                    return 0;
            }

            return 0;
        }

        private long GetSizeIncrement()
        {
            switch (Settings.ExecutableType)
            {
                case ExecutableType.PortableExecutable:
                    return 0x200;
                default:
                    return 0;
            }
        }

        protected override FinalProgram<byte> ProcessCore(FinalProgram<byte> input)
        {

            long current = GetBaseAddress(), sizeInc = GetSizeIncrement();
            input.PhysicalBaseAddress = current;
            input.PhysicalAlignment = sizeInc;

            foreach (var section in input.Sections)
            {
                var size = section.VirtualSize.RoundUpTo(sizeInc);

                section.PhysicalAddress = current;
                section.PhysicalSize = size;

                current += size;
            }

            return input;
        }
    }
}
