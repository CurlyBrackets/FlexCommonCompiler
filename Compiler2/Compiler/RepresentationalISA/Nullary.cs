using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.Compiler.RepresentationalISA
{
    class NullaryOperation<T> : RepresentationalBase<T>
    {
        public NullaryOperation(T op)
            : base(op)
        {

        }
    }
}
