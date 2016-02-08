using Compiler2.IR.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.IR
{
    abstract class IRDefaultVisitor : IRVisitor<Expression, Statement>
    {
        public override Expression Visit(Constant constant)
        {
            return constant;
        }

        public override Expression Visit(Label label)
        {
            return label;
        }

        public override Expression Visit(StringConstant constant)
        {
            return constant;
        }

        public override Expression Visit(UnaryExpression expression)
        {
            return ExpressionFactory.Instance.Unary(
                expression.Operation,
                expression.Expression.Accept(this));
        }

        public override Expression Visit(Memory memory)
        {
            var transformed = memory.Target.Accept(this);
            if (!(transformed is IMemoryExpression))
                throw new Exception("Bad memory transform");

            return ExpressionFactory.Instance.Memory(
                (IMemoryExpression)transformed);
        }

        public override Expression Visit(Offset offset)
        {
            return ExpressionFactory.Instance.Offset(
                (Register)offset.Base.Accept(this),
                (Constant)offset.Displacement.Accept(this));
        }

        public override Expression Visit(Register register)
        {
            return register;
        }

        public override Expression Visit(Pop pop)
        {
            return pop;
        }

        public override Expression Visit(Temporary temporary)
        {
            return temporary;
        }

        public override Expression Visit(Parameter parameter)
        {
            return parameter;
        }

        public override Expression Visit(FloatConstant constant)
        {
            return constant;
        }

        public override Expression Visit(Call call)
        {
            var args = new List<Expression>();

            foreach (var arg in call.Arguments)
                args.Add(arg.Accept(this));

            return ExpressionFactory.Instance.Call(
                call.Target,
                args);
        }

        public override Expression Visit(BinaryExpression expression)
        {
            return ExpressionFactory.Instance.Binary(
                expression.Operation,
                expression.Left.Accept(this),
                expression.Right.Accept(this));
        }

        public override Statement Visit(AssignStatement statement)
        {
            return StatementFactory.Instance.Assignment(
                statement.Destination,
                statement.Source.Accept(this));
        }

        public override Statement Visit(ReturnStatement statement)
        {
            if (statement.Value != null)
            {
                return StatementFactory.Instance.Return(
                    statement.Value.Accept(this));
            }
            else
            {
                return statement;
            }
        }

        public override Statement Visit(ExpressionStatement statement)
        {
            return StatementFactory.Instance.Expression(
                statement.Expression.Accept(this));
        }
    }
}
