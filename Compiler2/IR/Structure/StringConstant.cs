﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.IR.Structure
{
    class StringConstant : Expression
    {
        public string Value { get; private set; }

        public StringConstant(string val)
        {
            Value = val;
        }

        public E Accept<E, S>(IRVisitor<E, S> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
