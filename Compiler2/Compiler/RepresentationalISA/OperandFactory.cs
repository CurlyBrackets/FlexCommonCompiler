using Compiler2.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.Compiler.RepresentationalISA
{
    class OperandFactory : SingletonFactoryBase<OperandFactory>
    {
        private ThreeKeyDictionary<OperandType, OperandSize, int, RegisterOperand> m_registerMap;
        private TwoKeyDictionary<OperandSize, long, ImmediateOperand> m_immediateMap;
        private TwoKeyDictionary<OperandSize, Operand, MemoryOperand> m_memoryMap;
        private TwoKeyDictionary<RegisterOperand, ImmediateOperand, OffsetOperand> m_offsetMap;
        private TwoKeyDictionary<OperandSize, string, AddressOperand> m_addressMap;

        public OperandFactory()
        {
            m_registerMap = new ThreeKeyDictionary<OperandType, OperandSize, int, RegisterOperand>();
            m_immediateMap = new TwoKeyDictionary<OperandSize, long, ImmediateOperand>();
            m_memoryMap = new TwoKeyDictionary<OperandSize, Operand, MemoryOperand>();
            m_offsetMap = new TwoKeyDictionary<RegisterOperand, ImmediateOperand, OffsetOperand>();
            m_addressMap = new TwoKeyDictionary<OperandSize, string, AddressOperand>();
        }

        public RegisterOperand Register(OperandType type, OperandSize size, int index = 0)
        {
            if (m_registerMap.ContainsKey(type, size, index))
                return m_registerMap[type, size, index];

            var ret = new RegisterOperand(type, size, index);
            m_registerMap[type, size, index] = ret;
            return ret;
        }

        public ImmediateOperand Immediate(OperandSize size, long value)
        {
            if (m_immediateMap.ContainsKey(size, value))
                return m_immediateMap[size, value];

            var ret = new ImmediateOperand(size, value);
            m_immediateMap[size, value] = ret;
            return ret;
        }

        public MemoryOperand Memory(OperandSize size, Operand offset)
        {
            if (m_memoryMap.ContainsKey(size, offset))
                return m_memoryMap[size, offset];

            var ret = new MemoryOperand(size, offset);
            m_memoryMap[size, offset] = ret;
            return ret;
        }

        public OffsetOperand Offset(RegisterOperand register, ImmediateOperand offset)
        {
            if (m_offsetMap.ContainsKey(register, offset))
                return m_offsetMap[register, offset];

            var ret = new OffsetOperand(register, offset);
            m_offsetMap[register, offset] = ret;
            return ret;
        }

        public AddressOperand Address(OperandSize size, string label)
        {
            if (m_addressMap.ContainsKey(size, label))
                return m_addressMap[size, label];

            var ret = new AddressOperand(size, label);
            m_addressMap[size, label] = ret;
            return ret;
        }
    }
}
