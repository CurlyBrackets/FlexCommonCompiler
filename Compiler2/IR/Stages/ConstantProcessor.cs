using Compiler2.IR.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.IR.Stages
{
    class ConstantProcessor : CompileStage<IRProgram, IRProgram>, IRExpressionVisitor<Expression>, IRStatementVisitor<Statement>
    {
        public ConstantProcessor(CompilerSettings settings)
            : base(settings)
        {

        }

        private Common.ProgramConstants m_constants;

        protected override IRProgram ProcessCore(IRProgram input)
        {
            m_constants = input.Constants;

            foreach(var function in input.Functions)
            {
                function.Value.Transform((s) => s.Accept(this));
            }

            return input;
        }

        #region Visits
        #region Expression

        public Expression Visit(Constant constant)
        {
            if(constant.Value > int.MaxValue || constant.Value < int.MinValue)
            {
                var label = m_constants.Add(constant.Value);
                return ExpressionFactory.Instance.Unary(
                    Operation.AddressOf,
                    ExpressionFactory.Instance.Label(label));
            }
            else
            {
                return constant;
            }
        }

        public Expression Visit(StringConstant constant)
        {
            var label = m_constants.Add(constant.Value);
            return ExpressionFactory.Instance.Unary(
                Operation.AddressOf,
                ExpressionFactory.Instance.Label(label));
        }

        public Expression Visit(FloatConstant constant)
        {
            var label = m_constants.Add(constant.Value);
            return ExpressionFactory.Instance.Unary(
                Operation.AddressOf,
                ExpressionFactory.Instance.Label(label));
        }

        public Expression Visit(Parameter parameter)
        {
            return parameter;
        }

        public Expression Visit(Temporary temporary)
        {
            return temporary;
        }

        public Expression Visit(Label label)
        {
            return label;
        }

        public Expression Visit(Register register)
        {
            return register;
        }

        public Expression Visit(Call call)
        {
            var args = new List<Expression>();

            foreach (var arg in call.Arguments)
                args.Add(arg.Accept(this));

            return ExpressionFactory.Instance.Call(
                call.Target,
                args);
        }

        public Expression Visit(BinaryExpression expression)
        {
            return ExpressionFactory.Instance.Binary(
                expression.Operation,
                expression.Left.Accept(this),
                expression.Right.Accept(this));
        }

        public Expression Visit(UnaryExpression expression)
        {
            return ExpressionFactory.Instance.Unary(
                expression.Operation,
                expression.Expression.Accept(this));
        }

        #endregion
        #region Statements

        public Statement Visit(ReturnStatement statement)
        {
            if(statement.Value != null)
            {
                return StatementFactory.Instance.Return(
                    statement.Value.Accept(this));
            }
            else
            {
                return statement;
            }
        }

        public Statement Visit(AssignStatement statement)
        {
            return StatementFactory.Instance.Assignment(
                statement.Destination,
                statement.Source.Accept(this));
        }

        public Statement Visit(ExpressionStatement statement)
        {
            return StatementFactory.Instance.Expression(
                statement.Expression.Accept(this));
        }

        

        #endregion
        #endregion
    }
}
