using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.Utils
{
    class TwoKeyDictionary<K1, K2, V>
    {
        private Dictionary<K1, Dictionary<K2, V>> m_core;

        public TwoKeyDictionary()
        {
            m_core = new Dictionary<K1, Dictionary<K2, V>>();
        }

        public bool ContainsKey(K1 k1, K2 k2)
        {
            if (!m_core.ContainsKey(k1))
                return false;

            return m_core[k1].ContainsKey(k2);
        }

        public V this[K1 k1, K2 k2]
        {
            get
            {
                if (!m_core.ContainsKey(k1))
                    throw new KeyNotFoundException();

                var inner = m_core[k1];
                if (!inner.ContainsKey(k2))
                    throw new KeyNotFoundException();

                return inner[k2];
            }
            set
            {
                Dictionary<K2, V> inner;
                if (!m_core.ContainsKey(k1))
                {
                    inner = new Dictionary<K2, V>();
                    m_core.Add(k1, inner);
                }
                else
                    inner = m_core[k1];

                if (inner.ContainsKey(k2))
                    inner[k2] = value;
                else
                    inner.Add(k2, value);
            }
        }
    }

    class ThreeKeyDictionary<K1, K2, K3, V>
    {
        private Dictionary<K1, TwoKeyDictionary<K2, K3, V>> m_core;

        public ThreeKeyDictionary()
        {
            m_core = new Dictionary<K1, TwoKeyDictionary<K2, K3, V>>();
        }

        public bool ContainsKey(K1 k1, K2 k2, K3 k3)
        {
            if (!m_core.ContainsKey(k1))
                return false;

            return m_core[k1].ContainsKey(k2, k3);
        }

        public V this[K1 k1, K2 k2, K3 k3]
        {
            get
            {
                if (!m_core.ContainsKey(k1))
                    throw new KeyNotFoundException();

                return m_core[k1][k2, k3];
            }
            set
            {
                TwoKeyDictionary<K2, K3, V> inner;
                if (!m_core.ContainsKey(k1))
                {
                    inner = new TwoKeyDictionary<K2, K3, V>();
                    m_core.Add(k1, inner);
                }
                else
                    inner = m_core[k1];

                inner[k2, k3] = value;
            }
        }
    }
}
