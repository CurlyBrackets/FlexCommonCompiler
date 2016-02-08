using Compiler2.IR.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.IR.Stages
{
    /// <summary>
    /// Reserves the amount of stack space required by a function and adds the esp modifications
    /// </summary>
    /// <remarks>This must occur before the parameter stage</remarks>
    class StackAllocator : CompileStage<IRProgram, IRProgram>
    {
        public StackAllocator(CompilerSettings settings)
            : base(settings)
        {

        }

        protected override IRProgram ProcessCore(IRProgram input)
        {
            foreach(var function in input.Functions.Values)
            {
                function.AddToStart(
                    StatementFactory.Instance.Assignment(
                        ExpressionFactory.Instance.Register(RegisterType.Stack),
                        ExpressionFactory.Instance.Binary(
                            Operation.Subtract,
                            ExpressionFactory.Instance.Register(RegisterType.Stack),
                            ExpressionFactory.Instance.Constant(function.StackSpace))));
                function.AddToEnd(
                    StatementFactory.Instance.Assignment(
                        ExpressionFactory.Instance.Register(RegisterType.Stack),
                        ExpressionFactory.Instance.Binary(
                            Operation.Add,
                            ExpressionFactory.Instance.Register(RegisterType.Stack),
                            ExpressionFactory.Instance.Constant(function.StackSpace))));
            }

            return input;
        }
    }
}
