using Compiler2.Program;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.AST.Statements
{
    class Declaration : Statement
    {
        public Variable Variable{ get; set; }
        public object InitialValue { get; set; }

        public Declaration()
        {

        }
    }
}
