using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.AST.Statements
{
    class Throw : Statement
    {
        public Expression Exception { get; set; }
    }
}
