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

        public Expression Visit(Constant constant)
        {
            throw new NotImplementedException();
        }

        public Expression Visit(Parameter parameter)
        {
            throw new NotImplementedException();
        }

        public Expression Visit(UnaryExpression expression)
        {
            throw new NotImplementedException();
        }

        public Statement Visit(ExpressionStatement statement)
        {
            throw new NotImplementedException();
        }

        public Statement Visit(ReturnStatement statement)
        {
            throw new NotImplementedException();
        }

        public Statement Visit(AssignStatement statement)
        {
            throw new NotImplementedException();
        }

        public Expression Visit(StringConstant constant)
        {
            throw new NotImplementedException();
        }

        public Expression Visit(FloatConstant constant)
        {
            throw new NotImplementedException();
        }

        public Expression Visit(Call call)
        {
            throw new NotImplementedException();
        }

        public Expression Visit(BinaryExpression expression)
        {
            throw new NotImplementedException();
        }

        protected override IRProgram ProcessCore(IRProgram input)
        {


            return input;
        }
    }
}
