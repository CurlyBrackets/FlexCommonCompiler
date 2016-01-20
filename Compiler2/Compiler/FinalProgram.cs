using System.Collections.Generic;

namespace Compiler2.Compiler
{
    interface IFinalProgram
    {
        long VirtualBaseAddress { get; set; }
        long VirtualAlignment { get; set; }
        long PhysicalBaseAddress { get; set; }
        long PhysicalAlignment { get; set; }

        long ImportAddressTableAddress { get; set; }
        long ImportAddressTableSize { get; set; }
        long ImportDescriptorTableAddress { get; set; }
        long ImportDescriptorTableSize { get; set; }

        long ResourcesAddress { get; set; }
        long ResourcesSize { get; set; }
    }

    class FinalProgram<T> : IFinalProgram
    {
        public long VirtualBaseAddress { get; set; }
        public long VirtualAlignment { get; set; }
        public long PhysicalBaseAddress { get; set; }
        public long PhysicalAlignment { get; set; }

        public long ImportAddressTableAddress { get; set; }
        public long ImportAddressTableSize { get; set; }
        public long ImportDescriptorTableAddress { get; set; }
        public long ImportDescriptorTableSize { get; set; }

        public long ResourcesAddress { get; set; }
        public long ResourcesSize { get; set; }

        public IList<Section<T>> Sections { get; set; }

        public FinalProgram()
        {

        }

        public FinalProgram(IFinalProgram other){
            VirtualBaseAddress = other.VirtualBaseAddress;
            VirtualAlignment = other.VirtualAlignment;
            PhysicalBaseAddress = other.PhysicalBaseAddress;
            PhysicalAlignment = other.PhysicalAlignment;

            ImportDescriptorTableAddress = other.ImportDescriptorTableAddress;
            ImportDescriptorTableSize = other.ImportDescriptorTableSize;
            ImportAddressTableAddress = other.ImportAddressTableAddress;
            ImportAddressTableSize = other.ImportAddressTableSize;

            ResourcesAddress = other.ResourcesAddress;
            ResourcesSize = other.ResourcesSize;
        }
    }
}
