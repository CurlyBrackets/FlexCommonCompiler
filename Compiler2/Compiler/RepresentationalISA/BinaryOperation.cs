using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.Compiler.RepresentationalISA
{
    class BinaryOperation<T> : RepresentationalBase<T>
    {
        public Operand Left { get; set; }
        public Operand Right { get; set; }

        public BinaryOperation(T op)
            : base(op)
        {
            Left = null;
            Right = null;
        }

        public BinaryOperation(T op, Operand left, Operand right)
            : base(op)
        {
            Left = left;
            Right = right;
        }
    }
}
