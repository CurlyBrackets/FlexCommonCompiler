using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.IR
{
    class IRProgram
    {
        public IDictionary<string, IList<Structure.Statement>> Functions { get; private set; }
        public Common.ProgramConstants Constants { get; private set; }

        public IRProgram()
        {
            Functions = new Dictionary<string, IList<Structure.Statement>>();
            Constants = new Common.ProgramConstants();
        }

        public void AddFunction(string name, IList<Structure.Statement> body)
        {
            Functions.Add(name, body);
        }
    }
}
