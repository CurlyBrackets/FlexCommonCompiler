using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.IR.Structure
{
    enum RegisterType
    {
        Return,
        Stack,
        Base,
        Argument,
        Parameter,
        General
    }
}
