using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.IR.Structure
{
    abstract class Statement
    {
        public abstract S Accept<E, S>(IRVisitor<E, S> visitor);
    }
}
