using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.IR.Structure
{
    class Constant : Expression
    {
        public long Value { get; private set; }

        public Constant(long value)
        {
            Value = value;
        }
    }
}
