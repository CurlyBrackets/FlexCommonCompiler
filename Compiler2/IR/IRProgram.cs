using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.IR
{
    class IRProgram
    {
        public IDictionary<string, IRFunction> Functions { get; private set; }
        public Common.ProgramConstants Constants { get; private set; }

        public int ShadowSpace { get; set; }
        public int ArgCount { get; set; }
        public int SlotSize { get; set; }

        public IRProgram()
        {
            Functions = new Dictionary<string, IRFunction>();
            Constants = new Common.ProgramConstants();
        }

        public void AddFunction(string name, IList<Structure.Statement> body)
        {
            Functions.Add(name, new IRFunction(this, body));
        }
    }
}
