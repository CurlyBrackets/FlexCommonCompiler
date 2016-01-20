using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.AST.Statements
{
    class Branch : Statement
    {
        public Expression Condition { get; set; }
        public Block True { get; set; }
        public Block False { get; set; }

        public Branch()
        {

        }
    }
}
