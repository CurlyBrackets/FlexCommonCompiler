using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.AST.Statements
{
    class Try : Statement
    {
        public Statement Body { get; set; }
        public Catch Catch { get; set; }
        public Statement Finally { get; set; }
    }
}
