using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.Compiler.ExternalResolver
{
    abstract class ExternalResolver : CompileStage<FinalProgram<AddressIndependentThing>, FinalProgram<AddressIndependentThing>>
    {
        public ExternalResolver(CompilerSettings settings)
            : base(settings)
        {

        }

        protected abstract void ResolveExternals(FinalProgram<AddressIndependentThing> input);

        protected override FinalProgram<AddressIndependentThing> ProcessCore(FinalProgram<AddressIndependentThing> input)
        {
            ResolveExternals(input);
            if (!AllResolved(input))
                throw new Exception("External Resolver Failed");

            return input;
        }

        private bool AllResolved(FinalProgram<AddressIndependentThing> program)
        {
            foreach(var section in program.Sections)
            {
                foreach (var item in section.Data)
                    if (item is AddressIndependentExternalCall)
                        return false;
            }

            return true;
        }
    }
}
