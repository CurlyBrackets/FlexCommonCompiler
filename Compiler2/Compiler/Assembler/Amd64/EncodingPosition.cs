using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.Compiler.Assembler.Amd64
{
    enum EncodingPosition
    {
        None,
        Mod, // modrm 1100 0000
        Reg, // modrm 0011 1000
        RM,  // modrm 0000 0111
        Index, // sib 0011 1000
        Base   // sib 0000 0111
    }
}
