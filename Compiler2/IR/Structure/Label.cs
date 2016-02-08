using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.IR.Structure
{
    class Label : Expression, IMemoryExpression
    {
        public string Key { get; private set; }

        public Label(string key)
        {
            Key = key;
        }

        public T Accept<T>(IRExpressionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
