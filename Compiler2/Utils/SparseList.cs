using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.Utils
{
    class SparseList<T> : IList<T>
    {
        private List<T> m_core;

        public T this[int index]
        {
            get
            {
                return m_core[index];
            }

            set
            {
                if (index > m_core.Count - 1)
                {
                    while (index > m_core.Count)
                        m_core.Add(default(T));
                    m_core.Add(value);
                }
                else
                {
                    m_core[index] = value;
                }
            }
        }

        public int Count
        {
            get
            {
                return m_core.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public SparseList()
        {
            m_core = new List<T>();
        }

        public void Add(T item)
        {
            m_core.Add(item);
        }

        public void Clear()
        {
            m_core.Clear();
        }

        public bool Contains(T item)
        {
            return m_core.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            m_core.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return m_core.GetEnumerator();
        }

        public int IndexOf(T item)
        {
            return m_core.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            m_core.Insert(index, item);
        }

        public bool Remove(T item)
        {
            return m_core.Remove(item);
        }

        public void RemoveAt(int index)
        {
            m_core.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_core.GetEnumerator();
        }
    }
}
