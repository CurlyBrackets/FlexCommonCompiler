using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.Program
{
    class GenericSignature
    {
        private List<ElementType> m_internal;

        public ElementType this[int index]
        {
            get
            {
                return m_internal[index];
            }
        }

        public GenericSignature()
        {
            m_internal = new List<ElementType>();
        }

        public void Add(ElementType type)
        {
            m_internal.Add(type);
        }
    }
}
