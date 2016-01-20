using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.Compiler
{
    interface ISection
    {
        string Name { get; set; }
        long VirtualAddress { get; set; }
        long VirtualSize { get; set; }
        long PhysicalAddress { get; set; }
        long PhysicalSize { get; set; }
        ESectionFlags Flags { get; set; }
    }

    class Section<T> : ISection
    {
        public string Name { get; set; }
        public long VirtualAddress { get; set; }
        public long VirtualSize { get; set; }
        public long PhysicalAddress { get; set; }
        public long PhysicalSize { get; set; }
        public ESectionFlags Flags { get; set; }

        public IList<T> Data { get; set; }

        public Section() { }

        public Section(ISection other)
        {
            Name = other.Name;
            VirtualAddress = other.VirtualAddress;
            VirtualSize = other.VirtualSize;
            PhysicalAddress = other.PhysicalAddress;
            PhysicalSize = other.PhysicalSize;
            Flags = other.Flags;
        }
    }
}
