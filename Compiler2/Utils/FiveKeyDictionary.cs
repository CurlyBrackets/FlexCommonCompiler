using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.Utils
{
    class FiveKeyDictionary<K1, K2, K3, K4, K5, V>
    {
        public Dictionary<K1, Dictionary<K2, Dictionary<K3, Dictionary<K4, Dictionary<K5, V>>>>> m_core;

        public FiveKeyDictionary()
        {
            m_core = new Dictionary<K1, Dictionary<K2, Dictionary<K3, Dictionary<K4, Dictionary<K5, V>>>>>();
        }

        public bool ContainsKey(K1 k1, K2 k2, K3 k3, K4 k4, K5 k5)
        {
            if (!m_core.ContainsKey(k1))
                return false;

            if (!m_core[k1].ContainsKey(k2))
                return false;

            if (!m_core[k1][k2].ContainsKey(k3))
                return false;

            if (!m_core[k1][k2][k3].ContainsKey(k4))
                return false;

            return m_core[k1][k2][k3][k4].ContainsKey(k5);
        }

        public V this[K1 k1, K2 k2, K3 k3, K4 k4, K5 k5]
        {
            get
            {
                return m_core[k1][k2][k3][k4][k5];
            }
            set
            {
                if (!m_core.ContainsKey(k1))
                    m_core.Add(k1, new Dictionary<K2, Dictionary<K3, Dictionary<K4, Dictionary<K5, V>>>>());

                if (!m_core[k1].ContainsKey(k2))
                    m_core[k1].Add(k2, new Dictionary<K3, Dictionary<K4, Dictionary<K5, V>>>());

                if (!m_core[k1][k2].ContainsKey(k3))
                    m_core[k1][k2].Add(k3, new Dictionary<K4, Dictionary<K5, V>>());

                if (!m_core[k1][k2][k3].ContainsKey(k4))
                    m_core[k1][k2][k3].Add(k4, new Dictionary<K5, V>());

                if (!m_core[k1][k2][k3][k4].ContainsKey(k5))
                    m_core[k1][k2][k3][k4].Add(k5, value);
                else
                    m_core[k1][k2][k3][k4][k5] = value;
            }
        }
    }
}
