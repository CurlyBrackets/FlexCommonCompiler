using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.Program
{
    [Flags]
    enum EMethodFlags
    {
        HasThis = 0x20,
        ExplicitThis = 0x40,
        Default = 0x00,
        VarArg = 0x05,
        Generic = 0x10
    }
}
