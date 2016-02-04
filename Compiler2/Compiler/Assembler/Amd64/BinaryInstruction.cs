using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compiler2.Compiler.RepresentationalISA;
using RISA = Compiler2.Compiler.RepresentationalISA;
using Compiler2.Utils;

namespace Compiler2.Compiler.Assembler.Amd64
{
    class BinaryInstruction : Instruction
    {
        public Operand Left { get; private set; }
        public Operand Right { get; private set; }

        public BinaryInstruction(byte[] opcode, Operand left, Operand right)
            : base(opcode)
        {
            Left = left;
            Right = right;
        }

        public override byte[] Encode(BinaryOperation<Amd64Operation> op)
        {
            var ret = new List<byte>();

            if (RequiresRex(op))
                ret.Add(GenerateRex(op));

            ret.AddRange(OpCode);

            byte modrm = 0, sib = 0;
            foreach (var field in Fields)
                PutValue(ref modrm, ref sib, field.Position, field.Value);

            if (Left.Type == OperandType.Register)
                PutValue(ref modrm, ref sib, Left.EncodingPosition, RegisterIndex(op.Left));

            if (Right.Type == OperandType.Register)
                PutValue(ref modrm, ref sib, Right.EncodingPosition, RegisterIndex(op.Right));

            ret.Add(modrm);
            if ((modrm & 0xC0) != 0xC0)
                ret.Add(sib);

            if(Right.Type == OperandType.Immediate)
            {
                var immop = op.Right as RISA.ImmediateOperand;
                byte[] bytes = null;
                switch(Right.Size)
                {
                    case OperandSize.S8:
                        bytes = BitConverter.GetBytes((byte)immop.Value);
                        break;
                    case OperandSize.S16:
                        bytes = BitConverter.GetBytes((short)immop.Value);
                        break;
                    case OperandSize.S32:
                        bytes = BitConverter.GetBytes((int)immop.Value);
                        break;
                    case OperandSize.S64:
                        bytes = BitConverter.GetBytes(immop.Value);
                        break;
                }
                ret.AddRange(bytes);
            }

            return ret.ToArray();
        }

        #region Rex

        private bool RequiresRex(BinaryOperation<Amd64Operation> op)
        {
            return
                IsLargeOperand(op.Left) || IsLargeOperand(op.Right) ||
                IsExtendedRegister(op.Left) || IsExtendedRegister(op.Right);
        }

        private bool IsLargeOperand(RISA.Operand op)
        {
            return op.Size == RISA.OperandSize.QWord || op.Size == RISA.OperandSize.OWord;
        }

        private bool IsExtendedRegister(RISA.Operand op)
        {
            var rop = op as RegisterOperand;
            if (rop != null)
            {
                if (rop.Type.IsCombo(RISA.OperandType.ArgumentRegister))
                {
                    return rop.Index == 2 || rop.Index == 3;
                }

                // general register case?
            }

            return false;
        }

        private byte GenerateRex(BinaryOperation<Amd64Operation> op)
        {
            byte toAdd = 0x40;

            if (IsLargeOperand(op.Left) || IsLargeOperand(op.Right))
                toAdd |= 0x08;

            // modrm.reg extension
            if( (Left.EncodingPosition == EncodingPosition.Reg && IsExtendedRegister(op.Left)) ||
                (Right.EncodingPosition == EncodingPosition.Reg && IsExtendedRegister(op.Right)))
            {
                toAdd |= 0x04;
            }

            // sib index extension
            if ((Left.EncodingPosition == EncodingPosition.Index && IsExtendedRegister(op.Left)) ||
                (Right.EncodingPosition == EncodingPosition.Index && IsExtendedRegister(op.Right)))
            {
                toAdd |= 0x02;
            }

            // modrm.rm / sib.base extension
            if (((Left.EncodingPosition == EncodingPosition.RM || Left.EncodingPosition == EncodingPosition.Base) && IsExtendedRegister(op.Left)) ||
                ((Right.EncodingPosition == EncodingPosition.RM || Right.EncodingPosition == EncodingPosition.Base) && IsExtendedRegister(op.Right)))
            {
                toAdd |= 0x01;
            }

            return toAdd;
        }

        #endregion

        #region Put Field

        private void PutValue(ref byte modrm, ref byte sib, EncodingPosition position, byte val)
        {
            switch (position)
            {
                case EncodingPosition.Mod:
                    modrm |= (byte)(val << 6);
                    break;
                case EncodingPosition.Reg:
                    modrm |= (byte)(val << 3);
                    break;
                case EncodingPosition.RM:
                    modrm |= val;
                    break;
                case EncodingPosition.Base:
                    sib |= val;
                    break;
                case EncodingPosition.Index:
                    sib |= (byte)(val << 3);
                    break;
            }
        }

        #endregion
    }
}
