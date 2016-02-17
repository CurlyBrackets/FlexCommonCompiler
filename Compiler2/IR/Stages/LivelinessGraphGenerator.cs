using Compiler2.IR.Structure;
using Compiler2.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.IR.Stages
{
    class LivelinessGraphVisitor : IRVisitor<IList<Expression>, IList<Expression>>
    {
        private static readonly IList<Expression> EmptyList = new List<Expression>();

        public Expression AssignmentDest
        {
            get; set;
        } = null;

        public override IList<Expression> Visit(Constant constant)
        {
            return EmptyList;
        }

        public override IList<Expression> Visit(Label label)
        {
            return EmptyList;
        }

        public override IList<Expression> Visit(StringConstant constant)
        {
            return EmptyList;
        }

        public override IList<Expression> Visit(UnaryExpression expression)
        {
            return expression.Expression.Accept(this);
        }

        public override IList<Expression> Visit(Memory memory)
        {
            return memory.Target.Accept(this);
        }

        public override IList<Expression> Visit(Pop pop)
        {
            return EmptyList;
        }

        public override IList<Expression> Visit(ExpressionStatement statement)
        {
            return statement.Expression.Accept(this);
        }

        public override IList<Expression> Visit(ReturnStatement statement)
        {
            if (statement.Value != null)
                return statement.Value.Accept(this);
            return EmptyList;
        }

        public override IList<Expression> Visit(AssignStatement statement)
        {
            if(statement.Destination is Temporary || statement.Destination is Parameter)
                AssignmentDest = statement.Destination;
            return statement.Source.Accept(this);
        }

        public override IList<Expression> Visit(Offset offset)
        {
            var temp = offset.Base.Accept(this);
            temp.AddRange(offset.Displacement.Accept(this));
            return temp;
        }

        public override IList<Expression> Visit(Register register)
        {
            return EmptyList;
        }

        public override IList<Expression> Visit(Temporary temporary)
        {
            return new List<Expression>() { temporary };
        }

        public override IList<Expression> Visit(Parameter parameter)
        {
            return new List<Expression>() { parameter };
        }

        public override IList<Expression> Visit(FloatConstant constant)
        {
            return EmptyList;
        }

        public override IList<Expression> Visit(Call call)
        {
            var ret = new List<Expression>();

            foreach (var arg in call.Arguments)
                ret.AddRange(arg.Accept(this));

            return ret;
        }

        public override IList<Expression> Visit(BinaryExpression expression)
        {
            var temp = expression.Left.Accept(this);
            temp.AddRange(expression.Right.Accept(this));
            return temp;
        }
    }

    class LivelinessGraphGenerator : CompileStage<IRProgram, IRProgram>
    {
        public LivelinessGraphGenerator(CompilerSettings settings)
            : base(settings)
        {

        }

        protected override IRProgram ProcessCore(IRProgram input)
        {
            var visitor = new LivelinessGraphVisitor();
            var formatVisitor = new IRPrinterVisitor();

            foreach(var kvp in input.Functions)
            {
                Console.WriteLine(kvp.Key);
                var func = kvp.Value;
                var graph = new LivenessGraph();

                for(int i = 0; i < func.Body.Count; i++)
                {
                    visitor.AssignmentDest = null;
                    var active = func.Body[i].Accept(visitor);
                    if (visitor.AssignmentDest != null)
                        graph.New(visitor.AssignmentDest, i);

                    foreach (var item in active)
                        graph.RecordUse(item, i);

                    /*
                    Console.Write("\t{0} | [", i.ToString().PadLeft(3));
                    if (visitor.AssignmentDest != null)
                        Console.Write(visitor.AssignmentDest.Accept(formatVisitor).PadLeft(3));
                    else
                        Console.Write("   ");
                    Console.Write("] ");

                    foreach (var item in active)
                        Console.Write(item.Accept(formatVisitor).PadLeft(3) + " ");
                    Console.WriteLine();*/
                }
            }

            return input;
        }
    }
}
