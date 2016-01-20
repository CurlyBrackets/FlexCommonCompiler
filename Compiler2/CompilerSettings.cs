using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2
{
    enum ExecutableType
    {
        PortableExecutable,
        ExecutableAndLinkableFormat
    }

    enum ISA
    {
        x86,
        x86_64,
        ARM7
    }

    class CompilerSettings
    {
        public bool Is64Bit { get; set; }
        public ExecutableType ExecutableType { get; set; }
        public ISA ISA { get; set; }
        public string Output { get; set; }
    }
}
