using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compiler2.IR.Structure;

namespace Compiler2.IR.Stages
{
    class IRPrinterVisitor : IRVisitor<string, string>
    {
        public override string Visit(Constant constant)
        {
            return $"{constant.Value}";
        }

        public override string Visit(Label label)
        {
            return $"@{label.Key}";
        }

        public override string Visit(StringConstant constant)
        {
            return $"\"{constant.Value}\"";
        }

        public override string Visit(UnaryExpression expression)
        {
            switch (expression.Operation)
            {
                case Operation.AddressOf:
                    return "&" + expression.Expression.Accept(this);
                case Operation.Push:
                    return "push " + expression.Expression.Accept(this);
                default:
                    throw new Exception("bad unary expression op");
            }
        }

        public override string Visit(Memory memory)
        {
            return $"[ { memory.Target.Accept(this) } ]";
        }

        public override string Visit(Pop pop)
        {
            return "pop";
        }

        public override string Visit(Offset offset)
        {
            return offset.Base.Accept(this) + "+" + offset.Displacement.Accept(this);
        }

        private static readonly string[] GeneralNames = new string[] { "rbx", "rsi", "rdi", "r10", "r11", "r12", "r13", "r14", "r15" };
        private static readonly string[] ArgumentNames = new string[] { "rcx", "rdx", "r8", "r9" };

        public override string Visit(Register register)
        {
            switch (register.Type)
            {
                case RegisterType.Parameter:
                case RegisterType.Argument:
                    return ArgumentNames[register.Index];
                case RegisterType.General:
                    return GeneralNames[register.Index];
                case RegisterType.Base:
                    return "rbp";
                case RegisterType.Return:
                    return "rax";
                case RegisterType.Stack:
                    return "rsp";
                default:
                    return string.Empty;
            }
        }

        public override string Visit(Temporary temporary)
        {
            return $"t{ temporary.Index }";
        }

        public override string Visit(Parameter parameter)
        {
            return $"p{ parameter.Index }";
        }

        public override string Visit(FloatConstant constant)
        {
            return $"{constant.Value}f";
        }

        public override string Visit(Call call)
        {
            var sb = new StringBuilder();

            sb.Append(call.Target);
            sb.Append("(");

            for (int i = 0; i < call.Arguments.Count; i++)
            {
                sb.Append(call.Arguments[i].Accept(this));
                if (i < call.Arguments.Count - 1)
                    sb.Append(", ");
            }

            sb.Append(")");
            return sb.ToString();
        }

        public override string Visit(BinaryExpression expression)
        {
            var left = expression.Left.Accept(this);
            var right = expression.Right.Accept(this);
            string op = null;

            switch (expression.Operation)
            {
                case Operation.Add:
                    op = "+";
                    break;
                case Operation.AddCarry:
                    op = "+c";
                    break;
                case Operation.Subtract:
                    op = "-";
                    break;
                case Operation.SubtractBorrow:
                    op = "-c";
                    break;
                case Operation.Multiply:
                    op = "*";
                    break;
                case Operation.Divide:
                    op = "÷";
                    break;
                case Operation.Remainder:
                    op = "%";
                    break;
                case Operation.Compare:
                    op = "cmp";
                    break;
                case Operation.ExclusiveOr:
                    op = "^";
                    break;
                case Operation.And:
                    op = "&";
                    break;
                case Operation.Or:
                    op = "|";
                    break;
                case Operation.ShiftLeft:
                    op = "<<";
                    break;
                case Operation.ShiftRight:
                    op = ">>";
                    break;
            }

            return "(" + left + " " + op + " " + right + ")";
        }

        public override string Visit(ExpressionStatement statement)
        {
            return statement.Expression.Accept(this) + Environment.NewLine;
        }

        public override string Visit(ReturnStatement statement)
        {
            if (statement.Value != null)
                return "return " + statement.Value.Accept(this) + Environment.NewLine;
            else
                return "return" + Environment.NewLine;
        }

        public override string Visit(AssignStatement statement)
        {
            return
                statement.Destination.Accept(this) + " = " + statement.Source.Accept(this) + Environment.NewLine;
        }
    }

    class IRPrinter : CompileStage<IRProgram, string>
    {
        public IRPrinter(CompilerSettings settings)
            : base(settings)
        {

        }

        protected override string ProcessCore(IRProgram input)
        {
            var visitor = new IRPrinterVisitor();
            var sb = new StringBuilder();

            foreach(var kvp in input.Functions)
            {
                sb.AppendLine(kvp.Key);
                foreach(var statement in kvp.Value.Body)
                {
                    sb.Append('\t');
                    sb.Append(statement.Accept(visitor));
                }
            }

            return sb.ToString();
        }
    }
}
