using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compiler2.IR.Structure;

namespace Compiler2.IR.Stages
{
    class ArgumentSanitizerVisitor : IRDefaultVisitor
    {
        public IList<Statement> Before { get; private set; }
        private int m_argCount;

        public IRFunction Func { get; set; }

        public ArgumentSanitizerVisitor(int argCount)
        {
            m_argCount = argCount;
            Before = new List<Statement>();
        }

        public override Expression Visit(Call call)
        {
            var newArgs = new List<Expression>();

            foreach(var arg in call.Arguments)
            {
                if(newArgs.Count >= m_argCount || !(arg is Temporary))
                {
                    int index = Func.GetTempIndex();

                    Before.Add(
                        StatementFactory.Instance.Assignment(
                            ExpressionFactory.Instance.Temporary(index),
                            arg));

                    newArgs.Add(
                        ExpressionFactory.Instance.Temporary(index));
                }
                else
                {
                    newArgs.Add(arg);
                }
            }

            return ExpressionFactory.Instance.Call(
                call.Target,
                newArgs);
        }
    }

    /// <summary>
    /// Ensures that arguments can be assigned
    /// </summary>
    /// <remarks>Comes before ArgumentLifter</remarks>
    class ArgumentSanitizer : CompileStage<IRProgram, IRProgram>
    {
        public ArgumentSanitizer(CompilerSettings settings)
            : base(settings)
        {

        }

        protected override IRProgram ProcessCore(IRProgram input)
        {
            var visitor = new ArgumentSanitizerVisitor(input.ArgCount);

            foreach (var function in input.Functions.Values)
            {
                visitor.Func = function;

                for (int i = function.Body.Count - 1; i >= 0; i--)
                {
                    visitor.Before.Clear();
                    

                    function.Body[i] = function.Body[i].Accept(visitor);
                    if (visitor.Before.Count > 0)
                    {
                        foreach(var statement in visitor.Before)
                            function.Body.Insert(i, statement);
                    }
                }
            }

            return input;
        }
    }
}
