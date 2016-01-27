using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.Compiler.RepresentationalISA
{
    interface IRepresentationalVisitor<OpType, RetType>
    {
        RetType Visit(NullaryOperation<OpType> op);
        RetType Visit(UnaryOperation<OpType> op);
        RetType Visit(BinaryOperation<OpType> op);
        RetType Visit(SpecialOperation<OpType> op);
        RetType Visit(RepresentationalBase<OpType> op);
    }
}
