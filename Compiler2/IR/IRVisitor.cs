using Compiler2.IR.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.IR
{
    interface IRExpressionVisitor<T>
    {
        T Visit(BinaryExpression expression);
        T Visit(Call call);
        T Visit(Constant constant);
        T Visit(FloatConstant constant);
        T Visit(Label label);
        T Visit(Parameter parameter);
        T Visit(StringConstant constant);
        T Visit(Temporary temporary);
        T Visit(UnaryExpression expression);

        T Visit(Register register);
    }

    interface IRStatementVisitor<T>
    {
        T Visit(AssignStatement statement);
        T Visit(ExpressionStatement statement);
        T Visit(ReturnStatement statement);
    }
}
