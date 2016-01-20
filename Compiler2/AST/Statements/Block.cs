using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.AST.Statements
{
    class Block : Statement
    {
        public IList<Statement> Statements { get; private set; }

        public Block()
        {
            Statements = new List<Statement>();
        }
    }
}
