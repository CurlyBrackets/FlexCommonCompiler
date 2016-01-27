using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.Compiler.RepresentationalISA
{
    [Flags]
    enum Amd64Operation : uint
    {
        Subtract =      0x00000001,
        Add =           0x00000002,
        Multiply =      0x00000004,
        Divide =        0x00000008,

        Move =          0x00000010,
        LoadAddress =   0x00000020,
        Call =          0x00000040,
        Return =        0x00000080,
        Jump =          0x00000100,

        ExclusiveOr =   0x00000200,
        Or =            0x00000400,
        And =           0x00000800,
        AddCarry =      (Add | Carry),
        SubtractBorrow= (Subtract | Carry),
        Compare =       0x00004000,

        RotateLeft =    0x00010000,
        RotateRight =   0x00020000,
        ShiftLeft =     0x00040000,
        ShiftRight =    0x00080000,
        RotateCarryLeft = (RotateLeft | Carry),
        RotateCarryRight = (RotateRight | Carry),
        ArithmeticShiftRight = (Arithmetic | ShiftRight),

        LargeOperand =  0x01000000,
        Carry =         0x02000000,
        Arithmetic =    0x02000000,  
        Special =       0x80000000,

        BasicArithmetic = (Subtract | Add | ExclusiveOr | Or | And | Compare),
        Rotate = (RotateLeft | RotateRight | ShiftLeft | ShiftRight),
    }
}
