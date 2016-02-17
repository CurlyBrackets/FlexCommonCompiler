using Compiler2.IR.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.IR
{
    struct LivenessNode
    {
        public Expression Exp;
        public int Start;
        public int End;
    }

    class LivenessGraph
    {
        private Dictionary<Expression, LivenessNode> m_nodes;

        public LivenessGraph()
        {
            m_nodes = new Dictionary<Expression, LivenessNode>();
        }

        public void New(Expression exp, int index)
        {
            if (m_nodes.ContainsKey(exp))
                throw new Exception("Bad things");

            var node = new LivenessNode()
            {
                Exp = exp,
                Start = index,
                End = index
            };

            m_nodes.Add(exp, node);
        }

        public void RecordUse(Expression exp, int index)
        {
            if (!m_nodes.ContainsKey(exp))
            {
                if (exp is Parameter)
                {
                    New(exp, -1);
                }
                else
                    throw new Exception("Bad things 2");
            }

            var node = m_nodes[exp];
            if (node.End < index)
                node.End = index;
            //possibly need to update map
        }
    }
}
