using Compiler2.IR.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.IR.Stages
{
    class ParameterLifterVisitor : IRDefaultVisitor
    {
        private int m_argCount;
        private int m_slotSize;
        private int m_shadowSpace;

        public bool NeedsBase { get; private set; }

        public ParameterLifterVisitor(int argCount, int slotSize, int shadowSpace)
        {
            m_argCount = argCount;
            m_slotSize = slotSize;
            m_shadowSpace = shadowSpace;
        }

        public void Reset()
        {
            NeedsBase = false;
        }

        public override Expression Visit(Parameter parameter)
        {
            if (parameter.Index >= m_argCount)
            {
                NeedsBase = true;
                return ExpressionFactory.Instance.Memory(
                    ExpressionFactory.Instance.Offset(
                        ExpressionFactory.Instance.Register(RegisterType.Base),
                        ExpressionFactory.Instance.Constant(
                            m_shadowSpace + (parameter.Index - m_argCount + 1) * m_slotSize)));
            }
            else
            {
                return ExpressionFactory.Instance.Register(
                    RegisterType.Parameter,
                    parameter.Index);
            }
        }
    }
    /// <summary>
    /// Lifts excess parameters into the stack and managers the base pointers
    /// </summary>
    class ParameterLifter : CompileStage<IRProgram, IRProgram>
    {
        public ParameterLifter(CompilerSettings settings)
            : base(settings)
        {
            
        }

        protected override IRProgram ProcessCore(IRProgram input)
        {
            var visitor = new ParameterLifterVisitor(input.ArgCount, input.SlotSize, input.ShadowSpace);

            foreach(var func in input.Functions.Values)
            {
                visitor.Reset();
                func.Transform((s) => s.Accept(visitor));

                if (visitor.NeedsBase)
                {
                    func.AddToStart(
                        StatementFactory.Instance.Expression(
                            ExpressionFactory.Instance.Unary(
                                Operation.Push,
                                ExpressionFactory.Instance.Register(RegisterType.Base))),
                        StatementFactory.Instance.Assignment(
                            ExpressionFactory.Instance.Register(RegisterType.Base),
                            ExpressionFactory.Instance.Register(RegisterType.Stack)));

                    func.AddToEnd(
                        StatementFactory.Instance.Assignment(
                            ExpressionFactory.Instance.Register(RegisterType.Base),
                            ExpressionFactory.Instance.Pop()));
                }
            }

            return input;
        }
    }
}
