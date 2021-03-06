﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.IR.Structure
{
    class AssignStatement : Statement
    {
        public AssignableExpression Destination { get; private set; }
        public Expression Source { get; private set; }

        public AssignStatement(AssignableExpression dest, Expression src)
        {
            Destination = dest;
            Source = src;
        }

        public override S Accept<E, S>(IRVisitor<E, S> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
