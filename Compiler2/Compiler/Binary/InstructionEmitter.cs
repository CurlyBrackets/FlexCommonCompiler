using Compiler2.Compiler.RepresentationalISA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RetType = System.Collections.Generic.IList<Compiler2.Compiler.AddressIndependentThing>;

namespace Compiler2.Compiler.Binary
{
    abstract class InstructionEmitter<T> : IRepresentationalVisitor<T, RetType>
    {
        protected InstructionEmitter()
        {

        }

        public abstract RetType FarJump(JumpTarget label);

        public abstract RetType Visit(RepresentationalBase<T> op);
        public abstract RetType Visit(SpecialOperation<T> op);
        public abstract RetType Visit(BinaryOperation<T> op);
        public abstract RetType Visit(UnaryOperation<T> op);
        public abstract RetType Visit(NullaryOperation<T> op);
    }
}
