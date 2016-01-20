using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.Program
{
    class Constant : Element
    {
        public object Value { get; private set; }

        public Constant(ElementType type, object val)
            : base(type)
        {
            Value = val;
        }
    }
}
