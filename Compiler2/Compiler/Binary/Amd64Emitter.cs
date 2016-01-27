using Compiler2.Compiler.RepresentationalISA;
using Compiler2.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.Compiler.Binary
{
    class Amd64Emitter : InstructionEmitter<Amd64Operation>
    {
        public override IList<AddressIndependentThing> FarJump(JumpTarget target)
        {
            switch (target.Type)
            {
                case JumpTargetType.Relative:
                    return new List<AddressIndependentThing>()
                    {
                        new AddressIndependentByte(0xFF),
                        new AddressIndependentByte(0x25),
                        new AddressIndependentAddress(target.FullLabel)
                    };
                case JumpTargetType.External:
                    return new List<AddressIndependentThing>()
                    {
                        new AddressIndependentExternalCall(target.Label, target.Module)
                    };
                default:
                    throw new Exception("blur");
            }
        }

        private IList<AddressIndependentThing> m_ret;

        private void Setup()
        {
            m_ret = new List<AddressIndependentThing>();
        }

        private IList<AddressIndependentThing> Cleanup()
        {
            var ret = m_ret;
            m_ret = null;
            return ret;
        }

        #region Visits

        public override IList<AddressIndependentThing> Visit(NullaryOperation<Amd64Operation> op)
        {
            Setup();

            if (op.Operation.IsCombo(Amd64Operation.Return))
                m_ret.Add(AIF.Instance.Byte(0xC3));
            else
            {
                throw new NotImplementedException();
            }

            return Cleanup();
        }

        public override IList<AddressIndependentThing> Visit(UnaryOperation<Amd64Operation> op)
        {
            throw new NotImplementedException();
        }

        

        #region Binary Operations

        public override IList<AddressIndependentThing> Visit(BinaryOperation<Amd64Operation> op)
        {
            Setup();

            throw new NotImplementedException();

            //return Cleanup();
        }

        /// <summary>
        /// Extended op for modrm byte, used for 0x80, 0x81,0x83, 0xc0, 0xc1, 0xD0, D1, D2, D3
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        private byte GetModRMExOp(Amd64Operation op)
        {
            if (op.IsCombo(Amd64Operation.Add) || op.IsCombo(Amd64Operation.RotateLeft))
                return 0;
            else if (op.IsCombo(Amd64Operation.Or) || op.IsCombo(Amd64Operation.RotateRight))
                return 1 << 3;
            else if (op.IsCombo(Amd64Operation.AddCarry) || op.IsCombo(Amd64Operation.RotateCarryLeft))
                return 2 << 3;
            else if (op.IsCombo(Amd64Operation.SubtractBorrow) || op.IsCombo(Amd64Operation.RotateCarryRight))
                return 3 << 3;
            else if (op.IsCombo(Amd64Operation.And) || op.IsCombo(Amd64Operation.ShiftLeft))
                return 4 << 3;
            else if (op.IsCombo(Amd64Operation.Subtract) || op.IsCombo(Amd64Operation.ShiftRight))
                return 5 << 3;
            else if (op.IsCombo(Amd64Operation.ExclusiveOr) || op.IsCombo(Amd64Operation.ShiftLeft))
                return 6 << 3;
            else if (op.IsCombo(Amd64Operation.Compare) || op.IsCombo(Amd64Operation.ArithmeticShiftRight))
                return 7 << 3;

            return 0;
        }

        private byte GetModRMRm(Operand baseOperand)
        {
            var operand = baseOperand as RegisterOperand;
            if (operand == null)
                throw new Exception("ModRM.rm not register");

            if (operand.Type.IsCombo(OperandType.ReturnRegister))
                return 0; // rax | xmm0
            else if (operand.Type.IsCombo(OperandType.StackRegister))
                return 4;
            else if (operand.Type.IsCombo(OperandType.ReturnRegister))
                return 5;
            else if (operand.Type.IsCombo(OperandType.ArgumentRegister))
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
            else if (operand.Type.IsCombo(OperandType.GeneralRegister))
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

        #endregion

        public override IList<AddressIndependentThing> Visit(SpecialOperation<Amd64Operation> op)
        {
            throw new NotImplementedException();
        }

        public override IList<AddressIndependentThing> Visit(RepresentationalBase<Amd64Operation> op)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
