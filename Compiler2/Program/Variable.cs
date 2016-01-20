using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.Program
{
    enum VariableType
    {
        Parameter,
        Local
    }

    class Variable : Element
    {
        public int Id { get; private set; }

        public VariableType VarType { get; private set; }
        public int Index { get; private set; }

        private static int s_id = 0;
        private static object s_idLock = new object();

        public Variable(ElementType type, VariableType vtype, int index)
            : this(type)
        {
            Index = index;
            VarType = vtype;
        }

        private Variable(ElementType type)
            : base(type)
        {
            lock (s_idLock)
            {
                Id = s_id;
            }
        }
    }
}
