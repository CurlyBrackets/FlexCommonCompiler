using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P = Compiler2.Program;

namespace Compiler2.Program
{
    class Program
    {
        public IDictionary<string, P.Class> Classes
        {
            get; private set;
        }

        public P.Method EntryPoint
        {
            get; set;
        }

        public Program()
        {
            Classes = new Dictionary<string, P.Class>();
        }
    }
}
