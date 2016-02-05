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

        private IList<AddressIndependentThing> delayAdd = null;


        public BinaryInstruction(byte[] opcode, Operand left, Operand right)
            : base(opcode)
        {
            Left = left;
            Right = right;
        }

        public override IList<AddressIndependentThing> Encode(BinaryOperation<Amd64Operation> op)
        {
            delayAdd = null;
            var ret = new List<AddressIndependentThing>();

            if (RequiresRex(op))
                ret.Add(AIF.Instance.Byte(GenerateRex(op)));

            ret.AddRange(OpCode.Convert());

            byte modrm = 0, sib = 0;
            foreach (var field in Fields)
                PutValue(ref modrm, ref sib, field.Position, field.Value);

            ProcessOp(ref modrm, ref sib, Left, op.Left);
            ProcessOp(ref modrm, ref sib, Right, op.Right);

            if (Right.Type == OperandType.Register)
                PutValue(ref modrm, ref sib, Right.EncodingPosition, RegisterIndex(op.Right));

            ret.Add(AIF.Instance.Byte(modrm));
            if ((modrm & 0xC0) != 0xC0 && (modrm & 7) == 4)
                ret.Add(AIF.Instance.Byte(sib));

            if (delayAdd != null)
                ret.AddRange(delayAdd);

            if(Right.Type == OperandType.Immediate)
            {
                ret.AddRange(EncodeImmediate(op.Right));
            }

            return ret;
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
            return op.Size == OperandSize.QWord || op.Size == OperandSize.OWord;
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

        private void ProcessOp(ref byte modrm, ref byte sib, Operand localOperand, RISA.Operand op)
        {
            if (localOperand.Type == OperandType.Register)
                PutValue(ref modrm, ref sib, localOperand.EncodingPosition, RegisterIndex(op));
            else if (localOperand.Type == OperandType.Memory)
            {
                var mop = op as MemoryOperand;
                var registerOffset = mop.Address as RegisterOperand;
                var offsetOffset = mop.Address as OffsetOperand;

                if (registerOffset != null)
                {
                    PutValue(ref modrm, ref sib, EncodingPosition.Mod, 0);
                    byte regIndex = RegisterIndex(registerOffset);

                    switch (regIndex & 7)
                    {
                        case 4: //SP/R12
                            PutValue(ref modrm, ref sib, EncodingPosition.RM, regIndex);
                            PutValue(ref modrm, ref sib, EncodingPosition.Index, regIndex);
                            PutValue(ref modrm, ref sib, EncodingPosition.Base, regIndex);
                            break;
                        case 5: // BP/R13
                            throw new Exception("this is weird");
                        default:
                            // check for scale, indexer, etc
                            PutValue(ref modrm, ref sib, EncodingPosition.RM, regIndex);
                            break;
                    }
                }
                else if (offsetOffset != null)
                {
                    byte regIndex = RegisterIndex(offsetOffset.Register);
                    switch (regIndex & 7)
                    {
                        case 4: //SP/R12
                            PutValue(ref modrm, ref sib, EncodingPosition.RM, regIndex);
                            PutValue(ref modrm, ref sib, EncodingPosition.Index, (byte)(regIndex & 7));
                            PutValue(ref modrm, ref sib, EncodingPosition.Base, regIndex);
                            break;
                        case 5: // BP/R13
                            throw new Exception("this is weird");
                        default:
                            // check for scale, indexer, etc
                            PutValue(ref modrm, ref sib, EncodingPosition.RM, regIndex);
                            break;
                    }

                    if (offsetOffset.Immediate.Size == OperandSize.S8)
                    {
                        PutValue(ref modrm, ref sib, EncodingPosition.Mod, 1);
                        delayAdd = EncodeImmediate(offsetOffset.Immediate);
                    }
                    else if (offsetOffset.Immediate.Size == OperandSize.S32)
                    {
                        PutValue(ref modrm, ref sib, EncodingPosition.Mod, 2);
                        delayAdd = EncodeImmediate(offsetOffset.Immediate);
                    }
                    else
                        throw new Exception("Invalid memory offset size");
                }
            }
        }

        
    }
}
