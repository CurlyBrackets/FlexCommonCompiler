﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.IR.Structure
{
    /// <summary>
    /// Parameter to the method being processed
    /// </summary>
    class Parameter : Expression
    {
        public int Index { get; private set; }

        public Parameter(int index)
        {
            Index = index;
        }

        public E Accept<E, S>(IRVisitor<E, S> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
