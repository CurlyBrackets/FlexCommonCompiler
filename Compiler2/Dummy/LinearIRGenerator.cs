using Compiler2.IR;
using Compiler2.IR.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.Dummy
{
    class LinearIRGenerator : CompileStage<object, IRProgram>
    {
        public LinearIRGenerator(CompilerSettings settings)
            : base(settings)
        {
        }

        protected override IRProgram ProcessCore(object ignore)
        {
            var ret = new IRProgram();

            ret.AddFunction("_start", GenerateStart());
            ret.AddFunction("Program::main", GenerateMain());
            ret.AddFunction("Program::exit", GenerateExternalCall("ExitProcess", "KERNEL32.dll", 1));
            ret.AddFunction("Program::GetStdHandle", GenerateExternalCall("GetStdHandle", "KERNEL32.dll", 1));
            ret.AddFunction("Program::WriteFile", GenerateExternalCall("WriteFile", "KERNEL32.dll", 5));

            return ret;
        }

        private List<Statement> GenerateStart()
        {
            return new List<Statement>()
            {
                // t0 = 0 (NULL)
                StatementFactory.Instance.Assignment(
                    ExpressionFactory.Instance.Temporary(0),
                    ExpressionFactory.Instance.Constant(0)),
                // t1 = &t0
                StatementFactory.Instance.Assignment(
                    ExpressionFactory.Instance.Temporary(1),
                    ExpressionFactory.Instance.Unary(
                        Operation.AddressOf,
                        ExpressionFactory.Instance.Temporary(0))),
                // t2 = main(t1)
                StatementFactory.Instance.Assignment(
                    ExpressionFactory.Instance.Temporary(2),
                    ExpressionFactory.Instance.Call(
                        "Program::main",
                        ExpressionFactory.Instance.Temporary(1))),
                // exit(t2)
                StatementFactory.Instance.Expression(
                    ExpressionFactory.Instance.Call(
                        "Program::exit",
                        ExpressionFactory.Instance.Temporary(2))),
                // return
                StatementFactory.Instance.Return()
            };
        }

        private List<Statement> GenerateExternalCall(string name, string module, int n)
        {
            var args = new Expression[n];
            for (int i = 0; i < n; i++)
                args[i] = ExpressionFactory.Instance.Parameter(i);

            return new List<Statement>()
            {
                StatementFactory.Instance.Assignment(
                    ExpressionFactory.Instance.Temporary(0),
                    ExpressionFactory.Instance.ExternalCall(
                        name, module, args)),

                StatementFactory.Instance.Return(
                    ExpressionFactory.Instance.Temporary(0))
            };
        }

        private List<Statement> GenerateMain()
        {
            return new List<Statement>()
            {
                // t0 = -11
                StatementFactory.Instance.Assignment(
                    ExpressionFactory.Instance.Temporary(0),
                    ExpressionFactory.Instance.Constant(-11)),
                // t1 = GetStdHandle(t0)
                StatementFactory.Instance.Assignment(
                    ExpressionFactory.Instance.Temporary(1),
                    ExpressionFactory.Instance.Call(
                        "Program::GetStdHandle",
                        ExpressionFactory.Instance.Temporary(0))),
                // t2 = 0
                StatementFactory.Instance.Assignment(
                    ExpressionFactory.Instance.Temporary(2),
                    ExpressionFactory.Instance.Constant(0)),
                // t3 = 13
                StatementFactory.Instance.Assignment(
                    ExpressionFactory.Instance.Temporary(3),
                    ExpressionFactory.Instance.Constant(13)),
                // t4 = "Hello World\n"
                StatementFactory.Instance.Assignment(
                    ExpressionFactory.Instance.Temporary(4),
                    ExpressionFactory.Instance.Constant("Hello world\n")),
                // t5 = WriteFile(t1, t4, t3, t2, t2)
                StatementFactory.Instance.Assignment(
                    ExpressionFactory.Instance.Temporary(5),
                    ExpressionFactory.Instance.Call(
                        "Program::WriteFile",
                        ExpressionFactory.Instance.Temporary(1),
                        ExpressionFactory.Instance.Temporary(4),
                        ExpressionFactory.Instance.Temporary(3),
                        ExpressionFactory.Instance.Temporary(2),
                        ExpressionFactory.Instance.Temporary(2))),
                // t6 = 0
                StatementFactory.Instance.Assignment(
                    ExpressionFactory.Instance.Temporary(6),
                    ExpressionFactory.Instance.Constant(0)),
                // return t6
                StatementFactory.Instance.Return(
                    ExpressionFactory.Instance.Temporary(6))
            };
        }
    }
}
