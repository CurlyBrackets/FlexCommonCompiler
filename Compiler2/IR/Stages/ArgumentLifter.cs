using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compiler2.IR.Structure;

namespace Compiler2.IR.Stages
{
    class ArgumentLifterVisitor : IRDefaultVisitor
    {
        private int m_argCount, m_shadowSpace, m_slotSize;
        private Dictionary<Expression, int> m_extraArgs;
        private Dictionary<Expression, bool> m_used;
        public int MaxStack { get; private set; }

        public Statement After { get; set; }

        public ArgumentLifterVisitor(int argCount, int slotSize, int shadowSpace)
        {
            m_argCount = argCount;
            m_shadowSpace = shadowSpace;
            m_slotSize = slotSize;
            m_extraArgs = new Dictionary<Expression, int>();
            m_used = new Dictionary<Expression, bool>();
            MaxStack = 0;
        }

        public void Reset()
        {
            m_extraArgs.Clear();
            m_used.Clear();
            MaxStack = 0;
            After = null;
        }

        public override Expression Visit(Call call)
        {
            m_extraArgs.Clear();
            m_used.Clear();

            var keepArgs = call.Arguments.Take(m_argCount).ToList();

            for(int i = m_argCount; i < call.Arguments.Count; i++)
            {
                var exp = call.Arguments[i];
                m_extraArgs.Add(exp, i - m_argCount);

                var used = false;
                for(int j = 0; j < call.Arguments.Count; j++)
                {
                    if (!used && i != j && call.Arguments[j] == exp)
                        used = true;
                }

                m_used.Add(exp, used);

                var stackEnd = (i - m_argCount + 1) * m_slotSize;
                if (MaxStack < stackEnd)
                    MaxStack = stackEnd;
            }

            return ExpressionFactory.Instance.Call(
                call.Target,
                keepArgs);
        }

        public override Statement Visit(AssignStatement statement)
        {
            if (m_extraArgs.ContainsKey(statement.Destination))
            {
                var slot = m_extraArgs[statement.Destination];
                var used = m_used[statement.Destination];

                var memoryExp = ExpressionFactory.Instance.Memory(
                    ExpressionFactory.Instance.Offset(
                        ExpressionFactory.Instance.Register(RegisterType.Stack),
                        ExpressionFactory.Instance.Constant((slot * m_slotSize) + m_shadowSpace)));

                if (!used)
                {
                    return StatementFactory.Instance.Assignment(
                        memoryExp,
                        statement.Source.Accept(this));
                }
                else
                {
                    After = StatementFactory.Instance.Assignment(
                        memoryExp,
                        statement.Destination);

                    return StatementFactory.Instance.Assignment(
                        statement.Destination,
                        statement.Source.Accept(this));
                }
            }
            else
            {
                return StatementFactory.Instance.Assignment(
                    statement.Destination,
                    statement.Source.Accept(this));
            }
        }

        public override Expression Visit(Temporary temporary)
        {
            if (m_used.ContainsKey(temporary))
            {
                m_used[temporary] = true;
            }

            return temporary;
        }
    }

    /// <summary>
    /// Lifts excess arguments from calls into the stack
    /// </summary>
    /// <remarks>This should happen before the stack allocator but after the shadow space has been set</remarks>
    class ArgumentLifter : CompileStage<IRProgram, IRProgram>
    {
        public ArgumentLifter(CompilerSettings settings)
            : base(settings)
        {

        }

        protected override IRProgram ProcessCore(IRProgram input)
        {
            var visitor = new ArgumentLifterVisitor(input.ArgCount, input.SlotSize, input.ShadowSpace);

            foreach(var function in input.Functions.Values)
            {
                visitor.Reset();

                for (int i = function.Body.Count - 1; i >= 0; i--)
                {
                    visitor.After = null;
                    function.Body[i] = function.Body[i].Accept(visitor);
                    if (visitor.After != null)
                        function.Body.Insert(i + 1, visitor.After);
                }

                if (visitor.MaxStack > function.StackSpace - input.ShadowSpace)
                    function.IncreaseStack(visitor.MaxStack - (function.StackSpace - input.ShadowSpace));
            }

            return input;
        }
    }
}
