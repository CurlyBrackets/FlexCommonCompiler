using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.Compiler
{
    [Flags]
    enum ESectionFlags : uint
    {
        Code = 0x20,
        InitializedData = 0x40,
        Execute = 0x20000000,
        Read = 0x40000000,
        Write = 0x80000000  
    }
}
