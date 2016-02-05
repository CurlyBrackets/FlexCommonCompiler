using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.Compiler
{
    /// <summary>
    /// Common operand size enum between various compiler states
    /// </summary>
    enum OperandSize
    {
        Byte    = 0,
        Word    = 1,
        DWord   = 2,
        QWord   = 3,
        OWord   = 4,

        S8      = 0,
        S16     = 1,
        S32     = 2,
        S64     = 3,
        S128    = 4
    }
}
