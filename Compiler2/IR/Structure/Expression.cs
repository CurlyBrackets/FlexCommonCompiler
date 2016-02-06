using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.IR.Structure
{
    abstract class Expression
    {
        public abstract T Accept<T>(IRExpressionVisitor<T> visitor);
    }
}
