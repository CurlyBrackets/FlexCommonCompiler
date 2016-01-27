using Compiler2.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.Compiler
{
    class AIF : SingletonFactoryBase<AIF>
    {
        private Dictionary<byte, AddressIndependentByte> m_byteMap;
        private TwoKeyDictionary<string, string, AddressIndependentExternalCall> m_externalMap;
        private ThreeKeyDictionary<bool, bool, string, AddressIndependentAddress> m_addressMap;
        private Dictionary<string, AddressIndependentLabel> m_labelMap;

        public AIF()
        {
            m_byteMap = new Dictionary<byte, AddressIndependentByte>();
            m_externalMap = new TwoKeyDictionary<string, string, AddressIndependentExternalCall>();
            m_addressMap = new ThreeKeyDictionary<bool, bool, string, AddressIndependentAddress>();
            m_labelMap = new Dictionary<string, AddressIndependentLabel>();
        }

        public AddressIndependentByte Byte(byte b)
        {
            if (m_byteMap.ContainsKey(b))
                return m_byteMap[b];

            var ret = new AddressIndependentByte(b);
            m_byteMap.Add(b, ret);
            return ret;
        }

        public AddressIndependentLabel Label(string label)
        {
            if (m_labelMap.ContainsKey(label))
                return m_labelMap[label];

            var ret = new AddressIndependentLabel(label);
            m_labelMap.Add(label, ret);
            return ret;
        }

        public AddressIndependentExternalCall ExternalCall(string function, string module = "")
        {
            if (m_externalMap.ContainsKey(module, function))
                return m_externalMap[module, function];

            var ret = new AddressIndependentExternalCall(function, module);
            m_externalMap[module, function] = ret;
            return ret;
        }

        public AddressIndependentAddress Address(string name, bool is64 = false, bool isAbsolute = false)
        {
            if (m_addressMap.ContainsKey(is64, isAbsolute, name))
                return m_addressMap[is64, isAbsolute, name];

            var ret = new AddressIndependentAddress(name) { Is64Bit = is64, Absolute = isAbsolute };
            m_addressMap[is64, isAbsolute, name] = ret;
            return ret;
        }
    }
}
