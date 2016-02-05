using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compiler2.Compiler.RepresentationalISA;

namespace Compiler2.Compiler.Assembler.Amd64
{
    class UnaryInstruction : Instruction
    {
        public Operand Operand { get; private set; }

        public UnaryInstruction(byte[] opcode, Operand operand) 
            : base(opcode)
        {
            Operand = operand;
        }

        public override IList<AddressIndependentThing> Encode(UnaryOperation<Amd64Operation> op)
        {
            var ret = new List<AddressIndependentThing>();

            // any rex op
            ret.AddRange(OpCode.Convert());

            switch (Operand.Type)
            {
                case OperandType.Immediate:
                    ret.AddRange(EncodeImmediate(op.Target));
                    break;
                default:
                    throw new Exception("bad things");
            }

            return ret;
        }
    }
}
