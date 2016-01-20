using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.Compiler
{
    class SectionOrderer : CompileStage<FinalProgram<AddressIndependentThing>, FinalProgram<AddressIndependentThing>>
    {
        public SectionOrderer(CompilerSettings settings)
            : base(settings)
        {

        }

        private static readonly IDictionary<string, int> SectionOrder = new Dictionary<string, int>()
        {
            [".text"] = 0,
            [".rdata"] = 1,
            [".rodata"] = 1,
            [".data"] = 2,
            [".pdata"] = 5,
            [".rsrc"] = 9
        };

        protected override FinalProgram<AddressIndependentThing> ProcessCore(FinalProgram<AddressIndependentThing> input)
        {
            return new FinalProgram<AddressIndependentThing>(input) {
                Sections = input.Sections.OrderBy((section) => SectionOrder[section.Name]).ToList()
            };
        }
    }
}
