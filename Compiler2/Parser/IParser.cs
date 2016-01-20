using Compiler2.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P = Compiler2.Program;

namespace Compiler2.Parser
{
    interface IParser
    {
        void Parse(P.Program program, string filename);
    }
}
