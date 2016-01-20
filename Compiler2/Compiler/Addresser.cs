using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.Compiler
{
    class Addresser : CompileStage<FinalProgram<AddressIndependentThing>, FinalProgram<byte>>
    {
        class IntermediateSection
        {
            public Section<byte> Core;
            public IList<AddressIndependentThing> Intermediate;
            public MemoryStream Final;
            public long BaseAddress;
        }

        private const long Base64Address = 0x140000000;
        private const int Base32Address = 0x4000000;

        private const int SectionOffset = 0x1000;

        internal const string ImportDescriptorTableLabel = "Meta::ImportDescriptorTable";
        internal const string ImportAddressTableLabel = "Meta::ImportAddressTable";
        internal const string ResourcesLabel = "Meta::Resources";
        internal const string FunctionStartPrefix = "Meta::FunctionStart::";
        internal const string FunctionEndPrefix = "Meta::FunctionEnd::";
        internal const string FunctionPrefix = "Meta::Function";

        public Addresser(CompilerSettings settings)
            : base(settings)
        {

        }

        protected override FinalProgram<byte> ProcessCore(FinalProgram<AddressIndependentThing> input)
        {
            var symbolLookup = new Dictionary<string, long>();
            var sections = new Dictionary<string, IntermediateSection>();

            int sectionIndex = 1;
            foreach (var section in input.Sections)
            {
                sections.Add(section.Name, new IntermediateSection()
                {
                    Core = new Section<byte>(section),
                    Intermediate = section.Data,
                    Final = new MemoryStream(),
                    BaseAddress = (sectionIndex++ * SectionOffset)
                });
            }

            bool final = false;
            do { 
                final = RunPass(final, sections, symbolLookup);
            } while (!final);
            RunPass(final, sections, symbolLookup);

            var ret = new FinalProgram<byte>(input)
            {
                VirtualAlignment = SectionOffset,
                VirtualBaseAddress = Settings.Is64Bit ? Base64Address : Base32Address,
                ImportAddressTableAddress = symbolLookup[ImportAddressTableLabel],
                ImportDescriptorTableAddress = symbolLookup[ImportDescriptorTableLabel],
                //ResourcesAddress = symbolLookup[ResourcesLabel],
                Sections = sections.Select(
                    (intermediate) =>
                    {
                        intermediate.Value.Core.Data = intermediate.Value.Final.ToArray();
                        intermediate.Value.Core.VirtualAddress = intermediate.Value.BaseAddress;
                        intermediate.Value.Core.VirtualSize = intermediate.Value.Core.Data.Count;

                        return intermediate.Value.Core;
                    }
                ).ToList()
            };

            ret.Sections.Add(GeneratePData(symbolLookup,sectionIndex));

            return ret;
        }

        private bool RunPass(bool final, IDictionary<string, IntermediateSection> sections, IDictionary<string, long> symbolTable)
        {
            bool ret = true;

            foreach(var kvp in sections)
            {
                var args = new ProcessArgs()
                {
                    FinalPass = final,
                    Base = kvp.Value.BaseAddress,

                    Settings = Settings,

                    Intermediate = final ? null : new List<AddressIndependentThing>(),
                    Writer = final ? new BinaryWriter(kvp.Value.Final) : null,
                    Symbols = symbolTable
                };

                foreach(var ait in kvp.Value.Intermediate)
                    ret = ait.Process(args) && ret;

                if (!final)
                    kvp.Value.Intermediate = args.Intermediate;
            }

            return ret;
        }

        private Section<byte> GeneratePData(IDictionary<string, long> symbols, int sectionIndex)
        {
            var ret = new Section<byte>()
            {
                Name = ".pdata",
                Flags = ESectionFlags.InitializedData | ESectionFlags.Read,
                VirtualAddress = sectionIndex * SectionOffset
            };

            var temp = symbols.Where((kvp) => kvp.Key.StartsWith(FunctionPrefix));
            var starts = temp.Where((kvp) => kvp.Key.StartsWith(FunctionStartPrefix));
            var markers = temp.ToDictionary((kvp) => kvp.Key, (kvp) => kvp.Value);

            using (var ms = new MemoryStream())
            using (var br = new BinaryWriter(ms))
            {
                foreach(var start in starts)
                {
                    br.Write((uint)start.Value);
                    var end = markers[FunctionEndPrefix + start.Key.Substring(FunctionStartPrefix.Length)];
                    br.Write((uint)end);
                    br.Write((uint)0);
                }

                ret.Data = ms.ToArray().ToList();
                ret.VirtualSize = ret.Data.Count;    
            }

            return ret;
        }
    }
}
