using Compiler2.IR.Structure;
using Compiler2.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.IR
{
    class StatementFactory : SingletonFactoryBase<StatementFactory>
    {
        private Dictionary<Expression, ReturnStatement> m_returnMap;
        private TwoKeyDictionary<AssignableExpression, Expression, AssignStatement> m_assignMap;

        public StatementFactory()
        {
            m_returnMap = new Dictionary<Expression, ReturnStatement>();
            m_assignMap = new TwoKeyDictionary<AssignableExpression, Expression, AssignStatement>();
        }

        public ReturnStatement Return(Expression exp)
        {
            if (m_returnMap.ContainsKey(exp))
                return m_returnMap[exp];

            var ret = new ReturnStatement(exp);
            m_returnMap.Add(exp, ret);
            return ret;
        }

        public AssignStatement Assignment(AssignableExpression dest, Expression source)
        {
            if (m_assignMap.ContainsKey(dest, source))
                return m_assignMap[dest, source];

            var ret = new AssignStatement(dest, source);
            m_assignMap[dest, source] = ret;
            return ret;
        }
    }
}
