﻿using Compiler2.Compiler.Binary;
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
    class Emitter : InstructionEmitter<Amd64Operation>
    {
        private static readonly string[] Templates = new string[]
        {
            "Compiler2.Compiler.Assembler.Amd64.Template.xml"
        };

        private TemplateLoader m_loader;

        public Emitter()
        {
            m_loader = new TemplateLoader();

            foreach (var template in Templates)
                m_loader.Load(template);
        }

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
            Setup();

            if (!m_loader.UnaryInstructions.ContainsKey(
                op.Operation,
                FromRISA(op.Target.Type),
                op.Target.Size))
                throw new Exception($"Unary instruction not found: {op.Operation} {FromRISA(op.Target.Type)}[{op.Target.Size}]");

            var inst = m_loader.UnaryInstructions[
                op.Operation,
                FromRISA(op.Target.Type),
                op.Target.Size];

            m_ret.AddRange(inst.Encode(op));

            return Cleanup();
        }

        public override IList<AddressIndependentThing> Visit(BinaryOperation<Amd64Operation> op)
        {
            Setup();

            if(!m_loader.BinaryInstructions.ContainsKey(
                op.Operation,
                FromRISA(op.Left.Type),
                op.Left.Size,
                FromRISA(op.Right.Type),
                op.Right.Size))
                throw new Exception($"Binary operation not found: {op.Operation} {FromRISA(op.Left.Type)}[{op.Left.Size}], {FromRISA(op.Right.Type)}[{op.Right.Size}]");

            var inst = m_loader.BinaryInstructions[
                op.Operation,
                FromRISA(op.Left.Type),
                op.Left.Size,
                FromRISA(op.Right.Type),
                op.Right.Size];

            m_ret.AddRange(inst.Encode(op));

            return Cleanup();
        }

        private OperandType FromRISA(RISA.OperandType type)
        {
            switch (type)
            {
                case RISA.OperandType.ArgumentRegister:
                case RISA.OperandType.BaseRegister:
                case RISA.OperandType.GeneralRegister:
                case RISA.OperandType.InstructionRegister:
                case RISA.OperandType.ReturnRegister:
                case RISA.OperandType.StackRegister:
                    return OperandType.Register;

                case RISA.OperandType.Immediate:
                case RISA.OperandType.Address:
                    return OperandType.Immediate;
                
                case RISA.OperandType.Memory:
                    return OperandType.Memory;
                default:
                    throw new Exception("Fuck");
            }
        }


        public override IList<AddressIndependentThing> Visit(SpecialOperation<Amd64Operation> op)
        {
            var operation = op.Operation & ~Amd64Operation.Special;
            switch (operation)
            {
                case Amd64Operation.Call:
                    var temp = op as ExternalCallOperation<Amd64Operation>;
                    return new List<AddressIndependentThing>() {
                        AIF.Instance.ExternalCall(temp.Name, temp.Module)
                    };
                default:
                    throw new Exception("Unhandled special operation");
            }
        }

        public override IList<AddressIndependentThing> Visit(RepresentationalBase<Amd64Operation> op)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
