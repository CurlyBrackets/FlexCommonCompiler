using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compiler2.IR.Structure;

namespace Compiler2.IR.Stages
{
    class MemoryAssignmentExpanderVisitor : IRDefaultVisitor
    {
        public IRFunction Func { get; set; }
        public Statement Before { get; set; }

        public override Statement Visit(AssignStatement statement)
        {
            if(statement.Destination is Memory && statement.Source is Memory)
            {
                int index = Func.GetTempIndex();
                Before =
                    StatementFactory.Instance.Assignment(
                        ExpressionFactory.Instance.Temporary(index),
                        statement.Source);
                return
                    StatementFactory.Instance.Assignment(
                        statement.Destination,
                        ExpressionFactory.Instance.Temporary(index));
            }
            else
            {
                return statement;
            }
        }
    }

    /// <summary>
    /// Expands assignments where memory is both the left and right side
    /// </summary>
    class MemoryAssignmentExpander : CompileStage<IRProgram, IRProgram>
    {
        public MemoryAssignmentExpander(CompilerSettings settings)
            : base(settings)
        {

        }

        protected override IRProgram ProcessCore(IRProgram input)
        {
            var visitor = new MemoryAssignmentExpanderVisitor();

            foreach(var function in input.Functions.Values)
            {
                visitor.Func = function;

                for(int i = 0; i < function.Body.Count; i++)
                {
                    visitor.Before = null;
                    function.Body[i] = function.Body[i].Accept(visitor);
                    if (visitor.Before != null)
                        function.Body.Insert(i, visitor.Before);
                }
            }

            return input;
        }
    }
}
