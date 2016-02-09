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

            GenerateStart(ret.CreateFunction("_start"));
            GenerateMain(ret.CreateFunction("Program::main"));
            GenerateExternalCall(ret.CreateFunction("Program::exit"), "ExitProcess", "KERNEL32.dll", 1);
            GenerateExternalCall(ret.CreateFunction("Program::GetStdHandle"), "GetStdHandle", "KERNEL32.dll", 1);
            GenerateExternalCall(ret.CreateFunction("Program::WriteFile"), "WriteFile", "KERNEL32.dll", 5);

            return ret;
        }

        private void GenerateStart(IRFunction func)
        {
            func.AddToStart(
                // t0 = 0 (NULL)
                StatementFactory.Instance.Assignment(
                    ExpressionFactory.Instance.Temporary(func.GetTempIndex()),
                    ExpressionFactory.Instance.Constant(0)),
                // t1 = &t0
                StatementFactory.Instance.Assignment(
                    ExpressionFactory.Instance.Temporary(func.GetTempIndex()),
                    ExpressionFactory.Instance.Unary(
                        Operation.AddressOf,
                        ExpressionFactory.Instance.Temporary(1))),
                // t2 = main(t1)
                StatementFactory.Instance.Assignment(
                    ExpressionFactory.Instance.Temporary(func.GetTempIndex()),
                    ExpressionFactory.Instance.Call(
                        "Program::main",
                        ExpressionFactory.Instance.Temporary(2))),
                // exit(t2)
                StatementFactory.Instance.Expression(
                    ExpressionFactory.Instance.Call(
                        "Program::exit",
                        ExpressionFactory.Instance.Temporary(3))),
                // return
                StatementFactory.Instance.Return()
            );
        }

        private void GenerateExternalCall(IRFunction func, string name, string module, int n)
        {
            var args = new Expression[n];
            for (int i = 0; i < n; i++)
                args[i] = ExpressionFactory.Instance.Parameter(i);

            func.AddToStart(
                StatementFactory.Instance.Assignment(
                    ExpressionFactory.Instance.Temporary(func.GetTempIndex()),
                    ExpressionFactory.Instance.ExternalCall(
                        name, module, args)),

                StatementFactory.Instance.Return(
                    ExpressionFactory.Instance.Temporary(1))
            );
        }

        private void GenerateMain(IRFunction func)
        {
            func.AddToStart(
                // t0 = -11
                StatementFactory.Instance.Assignment(
                    ExpressionFactory.Instance.Temporary(func.GetTempIndex()),
                    ExpressionFactory.Instance.Constant(-11)),
                // t1 = GetStdHandle(t0)
                StatementFactory.Instance.Assignment(
                    ExpressionFactory.Instance.Temporary(func.GetTempIndex()),
                    ExpressionFactory.Instance.Call(
                        "Program::GetStdHandle",
                        ExpressionFactory.Instance.Temporary(1))),
                // t2 = 0
                StatementFactory.Instance.Assignment(
                    ExpressionFactory.Instance.Temporary(func.GetTempIndex()),
                    ExpressionFactory.Instance.Constant(0)),
                // t3 = 13
                StatementFactory.Instance.Assignment(
                    ExpressionFactory.Instance.Temporary(func.GetTempIndex()),
                    ExpressionFactory.Instance.Constant(13)),
                // t4 = "Hello World\n"
                StatementFactory.Instance.Assignment(
                    ExpressionFactory.Instance.Temporary(func.GetTempIndex()),
                    ExpressionFactory.Instance.Constant("Hello world\n")),
                // t5 = WriteFile(t1, t4, t3, t2, t2)
                StatementFactory.Instance.Assignment(
                    ExpressionFactory.Instance.Temporary(func.GetTempIndex()),
                    ExpressionFactory.Instance.Call(
                        "Program::WriteFile",
                        ExpressionFactory.Instance.Temporary(2),
                        ExpressionFactory.Instance.Temporary(5),
                        ExpressionFactory.Instance.Temporary(4),
                        ExpressionFactory.Instance.Temporary(3),
                        ExpressionFactory.Instance.Temporary(3))),
                // t6 = 0
                StatementFactory.Instance.Assignment(
                    ExpressionFactory.Instance.Temporary(func.GetTempIndex()),
                    ExpressionFactory.Instance.Constant(0)),
                // return t6
                StatementFactory.Instance.Return(
                    ExpressionFactory.Instance.Temporary(7))
            );
        }
    }
}
