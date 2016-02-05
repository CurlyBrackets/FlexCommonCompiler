using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.IR.Structure
{
    class ExternalCall : Call
    {
        public string Module { get; private set; }

        public ExternalCall(string name, string module, params Expression[] args)
            : base(name, args)
        {
            Module = module;
        }
    }
}
