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
        private Dictionary<double, FloatConstant> m_floatConstantMap;
        private Dictionary<string, StringConstant> m_stringConstantMap;

        private ThreeKeyDictionary<Operation, Expression, Expression, BinaryExpression> m_binaryMap;
        private Dictionary<int, Parameter> m_parameters;
        private Dictionary<int, Temporary> m_temporary;
        private TwoKeyDictionary<Operation, Expression, UnaryExpression> m_unaryMap;

        private Dictionary<string, List<Call>> m_calls;
        private TwoKeyDictionary<string, string, List<ExternalCall>> m_externalCalls;

        private Dictionary<string, Label> m_labels;

        public ExpressionFactory()
        {
            m_floatConstantMap = new Dictionary<double, FloatConstant>();
            m_stringConstantMap = new Dictionary<string, StringConstant>();
            m_constantMap = new Dictionary<long, Constant>();
            m_binaryMap = new ThreeKeyDictionary<Operation, Expression, Expression, BinaryExpression>();
            m_parameters = new Dictionary<int, Parameter>();
            m_temporary = new Dictionary<int, Temporary>();
            m_unaryMap = new TwoKeyDictionary<Operation, Expression, UnaryExpression>();
            m_calls = new Dictionary<string, List<Call>>();
            m_externalCalls = new TwoKeyDictionary<string, string, List<Structure.ExternalCall>>();
            m_labels = new Dictionary<string, Label>();
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

        public FloatConstant Constant(double val)
        {
            if (m_floatConstantMap.ContainsKey(val))
                return m_floatConstantMap[val];

            var ret = new FloatConstant(val);
            m_floatConstantMap.Add(val, ret);
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

        public Label Label(string label)
        {
            if (m_labels.ContainsKey(label))
                return m_labels[label];

            var ret = new Label(label);
            m_labels.Add(label, ret);
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
            return Call(name, args.ToList());
        }

        public Call Call(string name, IList<Expression> args)
        {
            if (m_calls.ContainsKey(name))
            {
                foreach (var call in m_calls[name])
                {
                    if (ArgsSame(args, call.Arguments))
                    {
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

        private bool ArgsSame(IList<Expression> args, IList<Expression> args2)
        {
            if (args.Count != args2.Count)
                return false;

            for (int i = 0; i < args.Count; i++)
            {
                if (args[i] != args2[i])
                    return false;
            }

            return true;
        }
    }
}
