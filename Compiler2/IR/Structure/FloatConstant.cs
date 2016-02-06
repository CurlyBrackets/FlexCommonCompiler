using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.IR.Structure
{
    class FloatConstant
    {
        public double Value { get; private set; }

        public FloatConstant(double val)
        {
            Value = val;
        }
    }
}
