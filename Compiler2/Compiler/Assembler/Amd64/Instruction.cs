using Compiler2.Compiler.RepresentationalISA;
using Compiler2.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RISA = Compiler2.Compiler.RepresentationalISA;

namespace Compiler2.Compiler.Assembler.Amd64
{
    abstract class Instruction
    {
        public byte[] OpCode { get; private set; }
        public IList<StaticField> Fields { get; set; }

        public Instruction(byte[] opcode)
        {
            OpCode = opcode;
            //Fields = new List<StaticField>();
        }

        public virtual byte[] Encode(BinaryOperation<Amd64Operation> op)
        {
            throw new InvalidOperationException();
        }

        public virtual byte[] Encode(UnaryOperation<Amd64Operation> op)
        {
            throw new InvalidOperationException();
        }

        public virtual byte[] Encode(NullaryOperation<Amd64Operation> op)
        {
            throw new InvalidOperationException();
        }

        protected byte RegisterIndex(RISA.Operand baseOperand)
        {
            var operand = baseOperand as RegisterOperand;
            if (operand == null)
                throw new Exception("ModRM.rm not register");

            if (operand.Type.IsCombo(RISA.OperandType.ReturnRegister))
                return 0; // rax | xmm0
            else if (operand.Type.IsCombo(RISA.OperandType.StackRegister))
                return 4;
            else if (operand.Type.IsCombo(RISA.OperandType.ReturnRegister))
                return 5;
            else if (operand.Type.IsCombo(RISA.OperandType.ArgumentRegister))
            {
                switch (operand.Index)
                {
                    case 0:
                        return 1; //TODO: windows only
                    case 1:
                        return 2; //TODO: windows only
                    case 2:
                        return 0;
                    case 3:
                        return 1;
                }
            }
            else if (operand.Type.IsCombo(RISA.OperandType.GeneralRegister))
            {
                switch (operand.Index)
                {
                    case 0:
                        return 3;
                    case 1:
                        return 6;
                    case 2:
                        return 7;
                    default:
                        return (byte)(operand.Index - 2);
                }
            }

            throw new Exception("Unexpected register");
        }
    }
}
