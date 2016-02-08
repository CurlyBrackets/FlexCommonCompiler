using Compiler2.IR.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.IR.Stages
{
    class ConstantProcessorVisitor : IRDefaultVisitor
    {
        private Common.ProgramConstants m_constants;
        public ConstantProcessorVisitor(Common.ProgramConstants constants)
        {
            m_constants = constants;
        }

        public override Expression Visit(Constant constant)
        {
            if (constant.Value > int.MaxValue || constant.Value < int.MinValue)
            {
                var label = m_constants.Add(constant.Value);
                return ExpressionFactory.Instance.Memory(
                        ExpressionFactory.Instance.Label(label));
            }
            else
            {
                return constant;
            }
        }

        public override Expression Visit(StringConstant constant)
        {
            var label = m_constants.Add(constant.Value);
            return ExpressionFactory.Instance.Unary(
                Operation.AddressOf,
                ExpressionFactory.Instance.Memory(
                        ExpressionFactory.Instance.Label(label)));
        }

        public override Expression Visit(FloatConstant constant)
        {
            var label = m_constants.Add(constant.Value);
            return ExpressionFactory.Instance.Memory(
                        ExpressionFactory.Instance.Label(label));
        }
    }

    class ConstantProcessor : CompileStage<IRProgram, IRProgram>
    {
        public ConstantProcessor(CompilerSettings settings)
            : base(settings)
        {

        }

        protected override IRProgram ProcessCore(IRProgram input)
        {
            var visitor = new ConstantProcessorVisitor(input.Constants);

            foreach(var function in input.Functions)
            {
                function.Value.Transform((s) => s.Accept(visitor));
            }

            return input;
        }
    }
}
