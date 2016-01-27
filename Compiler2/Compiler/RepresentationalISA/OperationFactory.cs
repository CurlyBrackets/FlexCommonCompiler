using Compiler2.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.Compiler.RepresentationalISA
{
    /// <summary>
    /// Implements a flyweight (?) pattern to save on the 'new's
    /// </summary>
    class OperationFactory<T> : SingletonFactoryBase<OperationFactory<T>>
    {
        private IDictionary<T, NullaryOperation<T>> m_nullaryMap;
        private TwoKeyDictionary<T, Operand, UnaryOperation<T>> m_unaryMap;
        private ThreeKeyDictionary<T, Operand, Operand, BinaryOperation<T>> m_binaryMap;

        public OperationFactory()
        {
            m_nullaryMap = new Dictionary<T, NullaryOperation<T>>();
            m_unaryMap = new TwoKeyDictionary<T, Operand, UnaryOperation<T>>();
            m_binaryMap = new ThreeKeyDictionary<T, Operand, Operand, BinaryOperation<T>>();
        }

        public NullaryOperation<T> Nullary(T operation)
        {
            if (m_nullaryMap.ContainsKey(operation))
                return m_nullaryMap[operation];

            var ret = new NullaryOperation<T>(operation);
            m_nullaryMap.Add(operation, ret);
            return ret;
        }

        public UnaryOperation<T> Unary(T operation, Operand operand)
        {
            if (m_unaryMap.ContainsKey(operation, operand))
                return m_unaryMap[operation, operand];

            var ret = new UnaryOperation<T>(operation, operand);
            m_unaryMap[operation, operand] = ret;
            return ret;
        }

        public BinaryOperation<T> Binary(T operation, Operand left, Operand right)
        {
            if (m_binaryMap.ContainsKey(operation, left, right))
                return m_binaryMap[operation, left, right];

            var ret = new BinaryOperation<T>(operation, left, right);
            m_binaryMap[operation, left, right] = ret;
            return ret;
        }

        public SpecialOperation<T> Special(T operation, params object[] args)
        {
            // Do this at some point


            throw new Exception("Special operation not matched");
        }
    }
}
