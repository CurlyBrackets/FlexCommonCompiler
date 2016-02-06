using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.IR.Structure
{
    abstract class Statement
    {
        public abstract T Accept<T>(IRStatementVisitor<T> visitor);
    }
}
