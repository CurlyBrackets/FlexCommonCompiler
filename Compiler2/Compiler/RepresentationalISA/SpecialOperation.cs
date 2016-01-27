using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.Compiler.RepresentationalISA
{
    abstract class SpecialOperation<T> : RepresentationalBase<T>
    {
        protected SpecialOperation(T op)
            : base(op)
        {

        }

        public override T2 Accept<T2>(IRepresentationalVisitor<T, T2> visitor)
        {
            return visitor.Visit(this);
        }
    }

    class ExternalCallOperation<T> : SpecialOperation<T>
    {
        public string Name { get; private set; }
        public string Module { get; private set; }

        public ExternalCallOperation(T op, string name, string module)
            : base(op)
        {
            Name = name;
            Module = module;
        }
    }
}
