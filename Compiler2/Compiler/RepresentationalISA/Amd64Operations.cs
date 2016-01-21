using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.Compiler.RepresentationalISA
{
    [Flags]
    enum Amd64Operation
    {
        Subtract =      0x00000001,
        Add =           0x00000002,
        Multiply =      0x00000004,
        Divide =        0x00000008,
        Move =          0x00000010,
        LoadAddress =   0x00000020,
        LargeOperand =  0x01000000
    }
}
