using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.IR.Structure
{
    enum Operation
    {
        Move,

        Add,
        AddCarry,
        Subtract,
        SubtractBorrow,
        Multiply,
        Divide,
        Remainder,
        Compare,

        ExclusiveOr,
        Or,
        And,

        ShiftLeft,
        ShiftRight,

        Call,
        AddressOf,
        Push,
        Pop
    }
}
