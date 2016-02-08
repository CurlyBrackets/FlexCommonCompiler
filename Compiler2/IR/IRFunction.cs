using Compiler2.IR.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.IR
{
    class IRFunction
    {
        public IRProgram Parent { get; private set; }
        public IList<Statement> Body { get; private set; }

        private int m_stack;
        public int StackSpace
        {
            get
            {
                return Parent.ShadowSpace + m_stack;
            }
        }

        public IRFunction(IRProgram program, IList<Statement> body)
        {
            Parent = program;
            Body = body;

            m_stack = 0;
        }

        public void IncreaseStack(int amount)
        {
            m_stack += amount;
        }

        public void AddToStart(params Statement[] statements)
        {
            for(int i = statements.Length-1; i >= 0; i--)
            {
                Body.Insert(0, statements[i]);
            }
        }

        public void AddToEnd(params Statement[] statements)
        {
            int insertIndex = Body.Count - 1;
            for (int i = statements.Length - 1; i >= 0; i--)
            {
                Body.Insert(insertIndex, statements[i]);
            }
        }

        public void Transform(Func<Statement, Statement> f)
        {
            for (int i = 0; i < Body.Count; i++)
                Body[i] = f(Body[i]);
        }
    }
}
