using Compiler2.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.Compiler.Executables
{
    [Flags]
    enum CoffCharacteristics : uint
    {
        Executable =    0x0002,
        LargeAddress =  0x0020,
        Nonrelocatable= 0x0200,
        DynamicLibrary= 0x2000
    }

    class PeWriter : ExecutableWriter
    {
        private static readonly byte[] DosHeader = new byte[]
        {
            0x4D, 0x5A, 0x90, 0x00, 0x03, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0x00, 0x00,
            0xB8, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x40, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x80, 0x00, 0x00, 0x00,
            0x0E, 0x1F, 0xBA, 0x0E, 0x00, 0xB4, 0x09, 0xCD, 0x21, 0xB8, 0x01, 0x4C, 0xCD, 0x21, 0x54, 0x68,
            0x69, 0x73, 0x20, 0x70, 0x72, 0x6F, 0x67, 0x72, 0x61, 0x6D, 0x20, 0x63, 0x61, 0x6E, 0x6E, 0x6F,
            0x74, 0x20, 0x62, 0x65, 0x20, 0x72, 0x75, 0x6E, 0x20, 0x69, 0x6E, 0x20, 0x44, 0x4F, 0x53, 0x20,
            0x6D, 0x6F, 0x64, 0x65, 0x2E, 0x0D, 0x0D, 0x0A, 0x24, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            /*0xF1, 0x2F, 0xAD, 0xC7, 0xB5, 0x4E, 0xC3, 0x94, 0xB5, 0x4E, 0xC3, 0x94, 0xB5, 0x4E, 0xC3, 0x94,
            0x68, 0xB1, 0x08, 0x94, 0xB6, 0x4E, 0xC3, 0x94, 0xB5, 0x4E, 0xC2, 0x94, 0xB6, 0x4E, 0xC3, 0x94,
            0x47, 0x17, 0xC7, 0x95, 0xB4, 0x4E, 0xC3, 0x94, 0x47, 0x17, 0x3C, 0x94, 0xB4, 0x4E, 0xC3, 0x94,
            0x47, 0x17, 0xC1, 0x95, 0xB4, 0x4E, 0xC3, 0x94, 0x52, 0x69, 0x63, 0x68, 0xB5, 0x4E, 0xC3, 0x94,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00*/
        };

        public PeWriter(CompilerSettings settings)
            : base(settings)
        {

        }

        protected override void WriteHeaders(FinalProgram<byte> final)
        {
            Writer.Write(DosHeader);
            WritePeHeader(final);
            WriteOptionalHeader(final);
            foreach (var section in final.Sections)
                WriteSectionHeader(section);
        }

        protected override void WriteSection(Section<byte> section)
        {
            PadTo(section.PhysicalAddress);
            Writer.Write(section.Data.ToArray());
        }

        #region Headers

        private void WritePeHeader(FinalProgram<byte> final)
        {
            Writer.Write(0x00004550); //PE\0\0
            switch (Settings.ISA)
            {
                case ISA.x86:
                    Writer.Write((ushort)0x14c);
                    break;
                case ISA.x86_64:
                    Writer.Write((ushort)0x8664);
                    break;
                case ISA.ARM7:
                    Writer.Write((ushort)0x1c4);
                    break;
            }

            Writer.Write((ushort)final.Sections.Count);
            Writer.Write((uint)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
            Writer.Write(0);
            Writer.Write(0);
            Writer.Write((ushort)0xF0);
            Writer.Write((ushort)(CoffCharacteristics.Executable | CoffCharacteristics.LargeAddress));
        }

        private void WriteOptionalHeader(FinalProgram<byte> final)
        {
            if (Settings.Is64Bit)
                Writer.Write((ushort)0x20b);
            else
                Writer.Write((ushort)0x10b);

            // linker version
            Writer.Write((byte)14);
            Writer.Write((byte)0);

            // size of code (uint32)
            Writer.Write(SumSection(final.Sections, ESectionFlags.Code));
            // size of init (uint32)
            Writer.Write(SumSection(final.Sections, ESectionFlags.InitializedData));
            // size of uninit (uint32)
            Writer.Write(SumSection(final.Sections, ~ESectionFlags.InitializedData));
            // relative entry point (uint32)
            Writer.Write((uint)final.Sections[0].VirtualAddress);
            // relative base of code (uint32)
            Writer.Write((uint)final.Sections[0].VirtualAddress);
            // image base (arch)
            if (Settings.Is64Bit)
                Writer.Write(final.VirtualBaseAddress);
            else
                Writer.Write((uint)final.VirtualBaseAddress);

            // section alignment (uint32) virtual chunk size
            Writer.Write((uint)final.VirtualAlignment);
            // file alignment (uint32) physical chunk size
            Writer.Write((uint)final.PhysicalAlignment);
            // os version (uint32) major / minor
            Writer.Write(0x00000006);
            // image version (uint32) major / minor
            Writer.Write(0x00000000);
            // subsystem (uint32) major / minor
            Writer.Write(0x00000006);
            // win32 version (uint32)
            Writer.Write(0);
            // size of image (uint32) ?
            Writer.Write(SumSection(final.Sections, ESectionFlags.Read, true) + (uint)final.VirtualAlignment);
            // size of headers (uint32)
            Writer.Write((uint)final.PhysicalBaseAddress);
            // checksum (uint32) 0?
            Writer.Write(0);
            // subsystem (short) 3 for console
            Writer.Write((ushort)3);
            // flags (short)
            Writer.Write((ushort)0x8160);
            // stack reserve (arch) ?
            ArchWrite(0x100000);
            // stack commit (arch) ?
            ArchWrite(0x1000);
            // heap reserve (arch) ?
            ArchWrite(0x100000);
            // heap commit (arch) ?
            ArchWrite(0x1000);
            // loader flags (uint32) 0
            Writer.Write(0);
            // number of dirs (uint32)
            Writer.Write(0x10);
            // n dirs? uint32 rva and uint32 size
            // Export Directory
            Writer.Write(new byte[8]);
            // Import Descriptor Directory
            Writer.Write((uint)final.ImportDescriptorTableAddress);
            Writer.Write((uint)final.ImportDescriptorTableSize);
            // Resource Directory
            /*Writer.Write((uint)final.ResourcesAddress);
            Writer.Write((uint)final.ResourcesSize);*/
            Writer.Write(new byte[8]);
            // Exception Directory
            Writer.Write(new byte[8]);
            // Certificates directory
            Writer.Write(new byte[8]);
            // Base relocation
            Writer.Write(new byte[8]);
            // Debug
            Writer.Write(new byte[8]);
            // Architecture
            Writer.Write(new byte[8]);
            // Global Pointer
            Writer.Write(new byte[8]);
            // Thread Local Storage
            Writer.Write(new byte[8]);
            // Load Configuration
            Writer.Write(new byte[8]);
            // Bound Import
            Writer.Write(new byte[8]);
            // Import Address
            Writer.Write((uint)final.ImportAddressTableAddress);
            Writer.Write((uint)final.ImportAddressTableSize);
            // Delay Import
            Writer.Write(new byte[8]);
            // COM descriptor
            Writer.Write(new byte[8]);
            // Reserved
            Writer.Write(new byte[8]);
        }

        private void ArchWrite(long value)
        {
            if (Settings.Is64Bit)
                Writer.Write(value);
            else
                Writer.Write((uint)value);
        }

        private uint SumSection(IList<Section<byte>> sections, ESectionFlags flags, bool @virtual = false)
        {
            long ret = 0;

            foreach (var section in sections)
                if (section.Flags.HasFlag(flags))
                    ret += @virtual ? section.VirtualSize.RoundUpTo(0x1000) : section.PhysicalSize;

            return (uint)ret;
        }

        private void WriteSectionHeader(Section<byte> section)
        {
            /*
            struct IMAGE_SECTION_HEADER // size 40 bytes
	            char[8]  mName;
	            uint32_t mVirtualSize;
	            uint32_t mVirtualAddress;
	            uint32_t mSizeOfRawData;
	            uint32_t mPointerToRawData;
	            uint32_t mPointerToRawData;
	            uint32_t mPointerToRealocations;
	            uint32_t mPointerToLinenumbers;
	            uint16_t mNumberOfRealocations;
	            uint16_t mNumberOfLinenumbers;
	            uint32_t mCharacteristics;
                */
            var b = Encoding.ASCII.GetBytes(section.Name).ToList();
            if (b.Count < 8)
                do
                    b.Add(0x00);
                while (b.Count < 8);
            else if (b.Count > 8)
                b.RemoveRange(8, b.Count - 8);

            Writer.Write(b.ToArray());
            Writer.Write((uint)section.VirtualSize);
            Writer.Write((uint)section.VirtualAddress);
            Writer.Write((uint)section.PhysicalSize);
            Writer.Write((uint)section.PhysicalAddress);
            Writer.Write(0);
            Writer.Write(0);
            Writer.Write((ushort)0);
            Writer.Write((ushort)0);
            Writer.Write((uint)section.Flags);
        }

        #endregion

        private void PadTo(long value)
        {
            while (Writer.BaseStream.Position < value)
                Writer.Write((byte)0x00);
        }
    }
}
