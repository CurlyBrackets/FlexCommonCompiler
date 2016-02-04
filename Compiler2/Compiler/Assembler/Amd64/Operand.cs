using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.Compiler.Assembler.Amd64
{
    class Operand
    {
        public OperandType Type { get; private set; }
        public OperandSize Size { get; private set; }
        public EncodingPosition EncodingPosition { get; private set; }

        public Operand(OperandType type, OperandSize size, EncodingPosition eposition)
        {
            Type = type;
            Size = size;
            EncodingPosition = eposition;
        }
    }
}
