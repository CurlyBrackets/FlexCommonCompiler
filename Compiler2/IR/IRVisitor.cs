using Compiler2.IR.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.IR
{
    abstract class IRVisitor<T, T2>
    {
        public abstract T Visit(BinaryExpression expression);
        public abstract T Visit(Call call);
        public abstract T Visit(Constant constant);
        public abstract T Visit(FloatConstant constant);
        public abstract T Visit(Label label);
        public abstract T Visit(Parameter parameter);
        public abstract T Visit(StringConstant constant);
        public abstract T Visit(Temporary temporary);
        public abstract T Visit(UnaryExpression expression);

        public abstract T Visit(Register register);
        public abstract T Visit(Memory memory);
        public abstract T Visit(Offset offset);

        public abstract T Visit(Pop pop);

        public abstract T2 Visit(AssignStatement statement);
        public abstract T2 Visit(ExpressionStatement statement);
        public abstract T2 Visit(ReturnStatement statement);
    }
}
