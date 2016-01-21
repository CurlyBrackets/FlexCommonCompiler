using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.Compiler.RepresentationalISA
{
    [Flags]
    enum OperandType
    {
        GeneralRegister =   0x01,
        Memory =            0x02,
        Float32 =           0x04, // i.e. xmmn register, s0-s32 (possibly wr0-16)
        Float64 =           0x08,
        Immediate =         0x10,
        StackRegister =     0x20,
        BaseRegister =      0x40,
        InstructionRegister=0x80,
        Offset =            0x100,
        ArgumentRegister =  0x200,
        ReturnRegister =    0x400
    }

    enum OperandSize
    {
        Byte,
        Word,
        DWord,
        QWord,
        OWord, //oct word, 128bit
    }

    abstract class Operand
    {
        public OperandType Type { get; set; }
        public OperandSize Size { get; set; }

        protected Operand(OperandType type = OperandType.GeneralRegister, OperandSize size = OperandSize.DWord)
        {
            Type = type;
            Size = size;
        }
    }

    class MemoryOperand : Operand
    {
        public Operand Address { get; private set; }

        public MemoryOperand(OperandSize size, Operand address)
            : base(OperandType.Memory, size)
        {
            Address = address;
        }
    }

    class ImmediateOperand : Operand
    {
        public static readonly ImmediateOperand Zero = new ImmediateOperand(OperandSize.Byte, 0);

        public long Value { get; private set; }

        public ImmediateOperand(OperandSize size, long value)
            : base(OperandType.Immediate, size)
        {
            Value = value;
        }
    }

    class OffsetOperand : Operand
    {
        public RegisterOperand Register { get; private set; }
        public ImmediateOperand Immediate { get; private set; }

        public OffsetOperand(RegisterOperand register)
            : this(register, ImmediateOperand.Zero)
        {

        }

        public OffsetOperand(RegisterOperand register, ImmediateOperand immedate)
            : base(OperandType.Offset)
        {
            Register = register;
            Immediate = immedate;

            if (Immediate.Value == 0)
                Type = register.Type;
        }
    }

    class RegisterOperand : Operand
    {
        public int Index { get; set; }

        public RegisterOperand(OperandType type, OperandSize size, int registerIndex = 0)
            : base(type, size)
        {
            Index = registerIndex;
        }
    }
}
