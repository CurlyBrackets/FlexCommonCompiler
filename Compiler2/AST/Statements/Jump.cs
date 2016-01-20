using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.AST.Statements
{
    class Jump : Statement
    {
        public int Index { get; set; } // temporary while parsing cil :(
        public Statement Destination { get; set; }
    }
}
