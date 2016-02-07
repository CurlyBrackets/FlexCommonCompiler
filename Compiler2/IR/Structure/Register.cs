using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.IR.Structure
{
    class Register : AssignableExpression
    {
        public RegisterType Type { get; private set; }
        public int Index { get; private set; }

        public Register(RegisterType type, int index = 0)
        {
            Type = type;
            Index = index;
        }

        public override T Accept<T>(IRExpressionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
