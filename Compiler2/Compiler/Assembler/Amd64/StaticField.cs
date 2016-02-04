using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.Compiler.Assembler.Amd64
{
    class StaticField
    {
        public byte Value { get; private set; }
        public EncodingPosition Position { get; private set; }

        public StaticField(byte value, EncodingPosition position)
        {
            Value = value;
            Position = position;
        }
    }
}
