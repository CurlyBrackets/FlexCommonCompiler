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
        private Dictionary<string, StringConstant> m_stringConstantMap;

        private ThreeKeyDictionary<Operation, Expression, Expression, BinaryExpression> m_binaryMap;
        private Dictionary<int, Parameter> m_parameters;
        private Dictionary<int, Temporary> m_temporary;
        private TwoKeyDictionary<Operation, Expression, UnaryExpression> m_unaryMap;

        private Dictionary<string, List<Call>> m_calls;
        private TwoKeyDictionary<string, string, List<ExternalCall>> m_externalCalls;

        public ExpressionFactory()
        {

            m_stringConstantMap = new Dictionary<string, StringConstant>();
            m_constantMap = new Dictionary<long, Constant>();
            m_binaryMap = new ThreeKeyDictionary<Operation, Expression, Expression, BinaryExpression>();
            m_parameters = new Dictionary<int, Parameter>();
            m_temporary = new Dictionary<int, Temporary>();
            m_unaryMap = new TwoKeyDictionary<Operation, Expression, UnaryExpression>();
            m_calls = new Dictionary<string, List<Call>>();
            m_externalCalls = new TwoKeyDictionary<string, string, List<Structure.ExternalCall>>();
        }

        public Constant Constant(long value)
        {
            if (m_constantMap.ContainsKey(value))
                return m_constantMap[value];

            var ret = new Constant(value);
            m_constantMap[value] = ret;
            return ret;
        }

        public StringConstant Constant(string val)
        {
            if (m_stringConstantMap.ContainsKey(val))
                return m_stringConstantMap[val];

            var ret = new StringConstant(val);
            m_stringConstantMap.Add(val, ret);
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

        public Parameter Parameter(int index)
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

        public Call Call(string name, params Expression[] args)
        {
            if (m_calls.ContainsKey(name))
            {
                foreach(var call in m_calls[name])
                {
                    if (ArgsSame(args, call.Arguments)) {
                        return call;
                    }    
                }
            }

            var ret = new Call(name, args);
            if (!m_calls.ContainsKey(name))
                m_calls.Add(name, new List<Call>());

            m_calls[name].Add(ret);

            return ret;
        }

        public ExternalCall ExternalCall(string name, string module, params Expression[] args)
        {
            if (m_externalCalls.ContainsKey(name, module))
            {
                foreach (var call in m_externalCalls[name, module])
                {
                    if (ArgsSame(args, call.Arguments))
                    {
                        return call;
                    }
                }
            }

            var ret = new ExternalCall(name, module, args);
            if (!m_externalCalls.ContainsKey(name, module))
                m_externalCalls[name, module] = new List<ExternalCall>();

            m_externalCalls[name, module].Add(ret);

            return ret;
        }

        private bool ArgsSame(Expression[] args, IList<Expression> args2)
        {
            if (args.Length != args2.Count)
                return false;

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] != args2[i])
                    return false;
            }

            return true;
        }
    }
}
