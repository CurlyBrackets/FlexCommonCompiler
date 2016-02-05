using Compiler2.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compiler2.IR.Structure;

namespace Compiler2.IR
{
    class ExpressionFactory : SingletonFactoryBase<ExpressionFactory>
    {
        private Dictionary<long, Constant> m_constantMap;
        private ThreeKeyDictionary<Operation, Expression, Expression, BinaryExpression> m_binaryMap;
        public Dictionary<int, Parameter> m_parameters;
        public Dictionary<int, Temporary> m_temporary;
        public TwoKeyDictionary<Operation, Expression, UnaryExpression> m_unaryMap;

        public ExpressionFactory()
        {
            m_constantMap = new Dictionary<long, Constant>();
            m_binaryMap = new ThreeKeyDictionary<Operation, Expression, Expression, BinaryExpression>();
            m_parameters = new Dictionary<int, Parameter>();
            m_temporary = new Dictionary<int, Temporary>();
            m_unaryMap = new TwoKeyDictionary<Operation, Expression, UnaryExpression>();
        }

        public Constant Constant(long value)
        {
            if (m_constantMap.ContainsKey(value))
                return m_constantMap[value];

            var ret = new Constant(value);
            m_constantMap[value] = ret;
            return ret;
        }

        public BinaryExpression Binary(Operation op, Expression left, Expression right)
        {
            if (m_binaryMap.ContainsKey(op, left, right))
                return m_binaryMap[op, left, right];

            var ret = new BinaryExpression(op, left, right);
            m_binaryMap[op, left, right] = ret;
            return ret;
        }

        public Parameter Paramter(int index)
        {
            if (m_parameters.ContainsKey(index))
                return m_parameters[index];

            var ret = new Parameter(index);
            m_parameters.Add(index, ret);
            return ret;
        }

        public Temporary Temporary(int index)
        {
            if (m_temporary.ContainsKey(index))
                return m_temporary[index];

            var ret = new Temporary(index);
            m_temporary.Add(index, ret);
            return ret;
        }

        public UnaryExpression Unary(Operation op, Expression exp)
        {
            if (m_unaryMap.ContainsKey(op, exp))
                return m_unaryMap[op, exp];

            var ret = new UnaryExpression(op, exp);
            m_unaryMap[op, exp] = ret;
            return ret;
        }
    }
}
