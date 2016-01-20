using Compiler2.Compiler.Binary;
using Compiler2.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.Compiler.ExternalResolver
{
    /*
        IAT (referenced by import symbol table and iat entry in directories)
            -list of IMAGE_IMPORT_BY_NAME rva
            -native int rva to above
            -zero terminated
            - before ist

        Import descriptor table
            -(uint32) Rva to original IAT (after this)
            -(uint32) 0 timestamp
            -(uint32)Forwarder chain (0)
            -(uint32)rva to name (after named imports)
            -(uint32)rva to IAT (before this)
            -zero terminated

        named imports
            -(uint16) hint; number of named entry in dll export table, can be 0
            -ACIIZ name
            -not zero terminated

        things to figure out
            -multiple imports?
    */
    class WindowsResolver : ExternalResolver
    {
        class ImportEntry
        {
            public string Module;
            public string Function;
            public int Hint;
            
            public string LabelName
            {
                get
                {
                    return string.Join("::", Module, Function);
                }
            }

            public byte[] GetBytes()
            {
                using (var ms = new MemoryStream(3 + Function.Length))
                using (var br = new BinaryWriter(ms))
                {
                    br.Write((ushort)Hint);
                    br.Write(Function.ToCharArray());
                    br.Write((byte)0);

                    return ms.ToArray();
                }
            }
        }

        private IDictionary<string, IDictionary<string, int>> m_exportedSymbols;

        private const string RealIatLabel = "::RealIat";
        private const string NameIatLabel = "::NameIat";
        private const string NameLabel = "::ModuleName";
        private const string EntryJoin = "@";

        private InstructionEmitter m_emitter;
        
        public WindowsResolver(CompilerSettings settings, InstructionEmitter instructionEmitter)
            : base(settings)
        {
            m_exportedSymbols = new Dictionary<string, IDictionary<string, int>>();
            m_emitter = instructionEmitter;
        }

        protected override void ResolveExternals(FinalProgram<AddressIndependentThing> input)
        {
            m_exportedSymbols.Clear();

            var entries = GatherEntries(input);
            FillEntryHints(entries);

            long idtSum = 20, iatSum = 8; // start with size of zero pad

            var rdata = GetOrCreateRData(input);

            IList<IList<AddressIndependentThing>>   realIats = new List<IList<AddressIndependentThing>>(),
                                                    idts = new List<IList<AddressIndependentThing>>(),
                                                    nameIats = new List<IList<AddressIndependentThing>>(),
                                                    namedImports = new List<IList<AddressIndependentThing>>(),
                                                    moduleNames = new List<IList<AddressIndependentThing>>();

            foreach (var kvp in m_exportedSymbols)
            {
                var mEntries = entries.Where((entry) => entry.Module == kvp.Key).ToList();

                var realIat = GenerateAddressTable(mEntries, true);
                iatSum += realIat.GetOffset();
                realIats.Add(realIat.Prepend(new AddressIndependentLabel(kvp.Key + RealIatLabel)));

                var idt = GenerateDescriptorTable(kvp.Key);
                idtSum += idt.GetOffset();
                idts.Add(idt);

                nameIats.Add(GenerateAddressTable(mEntries, false).Prepend(new AddressIndependentLabel(kvp.Key + NameIatLabel)));
                namedImports.Add(GenerateNamedImports(mEntries));

                var name = new List<AddressIndependentThing>();

                name.Add(new AddressIndependentLabel(kvp.Key + NameLabel));
                name.AddRange(Encoding.ASCII.GetBytes(kvp.Key).Convert());
                name.Add(new AddressIndependentByte(0));

                moduleNames.Add(name);
            }

            rdata.Add(new AddressIndependentLabel(Addresser.ImportAddressTableLabel));
            rdata.AddMany(realIats);
            rdata.AddRange(new byte[8].Convert()); //zero terminate
            rdata.Add(new AddressIndependentLabel(Addresser.ImportDescriptorTableLabel));
            rdata.AddMany(idts);
            rdata.AddRange(new byte[20].Convert());//zero terminate
            rdata.AddMany(nameIats);
            rdata.AddRange(new byte[8].Convert()); //zero terminate
            rdata.AddMany(namedImports);
            rdata.AddMany(moduleNames);

            // sizes are only known here
            input.ImportAddressTableSize = iatSum;
            input.ImportDescriptorTableSize = idtSum;

            var n = rdata.GetOffset();

            ResolveCalls(input);   
        }

        private IList<ImportEntry> GatherEntries(FinalProgram<AddressIndependentThing> input)
        {
            var ret = new List<ImportEntry>();

            foreach(var section in input.Sections)
            {
                foreach(var item in section.Data)
                {
                    var callItem = item as AddressIndependentExternalCall;
                    if(callItem != null)
                    {
                        ret.Add(new ImportEntry() { Module = callItem.Module, Function = callItem.Name });
                    }
                }
            }

            return ret;
        }

        private void FillEntryHints(IList<ImportEntry> entries)
        {
            foreach(var entry in entries)
            {
                if (!m_exportedSymbols.ContainsKey(entry.Module))
                    LoadExportedSymbols(entry.Module);

                entry.Hint = m_exportedSymbols[entry.Module][entry.Function];
            }
        }

        private void LoadExportedSymbols(string shortname)
        {
            var names = FindInPath(shortname);
            if (File.Exists(shortname))
            {
                var temp = names.ToList();
                temp.Insert(0, shortname);
                names = temp;
            }

            var targetMachine = Settings.Is64Bit ? Machine.Amd64 : Machine.I386;
            foreach (var name in names)
            {
                if (!string.IsNullOrEmpty(name))
                {
                    using (var fs = File.OpenRead(name))
                    using (var peReader = new PEReader(fs))
                    {
                        if (peReader.PEHeaders.CoffHeader.Machine != targetMachine)
                            continue;

                        var toAdd = new Dictionary<string, int>();

                        var rva = peReader.PEHeaders.PEHeader.ExportTableDirectory;
                        var data = peReader.GetSectionData(rva.RelativeVirtualAddress).GetContent().Take(36).ToArray();

                        int count = BitConverter.ToInt32(data, 24);
                        int rva2 = BitConverter.ToInt32(data, 32);

                        var section2 = peReader.GetSectionData(rva2).GetContent().ToArray();
                        for (int i = 0; i < count; i++)
                        {
                            var rva3 = BitConverter.ToInt32(section2, i * 4);
                            var name1 = Encoding.ASCII.GetString(peReader.GetSectionData(rva3).GetContent().TakeWhile((b) => b != 0).ToArray());
                            toAdd.Add(name1, i);
                        }

                        m_exportedSymbols.Add(shortname, toAdd);
                        break;
                    }
                }
            }
        }

        private IEnumerable<string> FindInPath(string filename)
        {
            return Environment.GetEnvironmentVariable("PATH")
                .Split(';')
                .Select((dir) => Path.Combine(dir, filename))
                .Where((path) => File.Exists(path));
        }

        private IList<AddressIndependentThing> GetOrCreateRData(FinalProgram<AddressIndependentThing> input)
        {
            foreach(var section in input.Sections)
            {
                if (section.Name == ".rdata" || section.Name == ".rodata")
                    return section.Data;
            }

            // create
            var rdataSection = new Section<AddressIndependentThing>()
            {
                Name = ".rdata",
                Flags = ESectionFlags.InitializedData | ESectionFlags.Read,
                Data = new List<AddressIndependentThing>()
            };

            input.Sections.Add(rdataSection);

            return rdataSection.Data;
        }

        private IList<AddressIndependentThing> GenerateNamedImports(IList<ImportEntry> entries)
        {
            var ret = new List<AddressIndependentThing>();

            foreach(var entry in entries)
            {
                ret.Add(new AddressIndependentLabel(entry.LabelName));
                ret.AddRange(entry.GetBytes().Convert());
            }

            return ret;        
        }

        private IList<AddressIndependentThing> GenerateAddressTable(IList<ImportEntry> entries, bool addLabel)
        {
            var ret = new List<AddressIndependentThing>();

            foreach (var entry in entries)
            {
                if (addLabel)
                    ret.Add(new AddressIndependentLabel(entry.Module + EntryJoin + entry.Function));
                ret.Add(new AddressIndependentAddress(entry.LabelName) { Is64Bit = true, Absolute = true }); //TODO: review this for 32bit exe, is possibly uint32 addr and uint32 size
            }

            return ret;
        }

        private IList<AddressIndependentThing> GenerateDescriptorTable(string module)
        {
            var ret = new List<AddressIndependentThing>();

            ret.Add(new AddressIndependentAddress(module + NameIatLabel) { Absolute = true });
            ret.AddRange((new byte[8]).Convert());
            ret.Add(new AddressIndependentAddress(module + NameLabel) { Absolute = true });
            ret.Add(new AddressIndependentAddress(module + RealIatLabel) { Absolute = true });

            return ret;
        }

        private void ResolveCalls(FinalProgram<AddressIndependentThing> input)
        {
            foreach(var section in input.Sections)
                ResolveCalls(section);   
        }

        private void ResolveCalls(Section<AddressIndependentThing> section)
        {
            for (int i = 0; i < section.Data.Count; i++)
            {
                var temp = section.Data[i] as AddressIndependentExternalCall;
                if (temp != null)
                {
                    section.Data.RemoveAt(i);
                    var newJump = m_emitter.FarJump(
                        new JumpTarget(
                            JumpTargetType.Relative,
                            temp.Name)
                        {
                            Module = temp.Module
                        });

                    section.Data.InsertRange(i, newJump);
                }
            }
        }
    }
}
