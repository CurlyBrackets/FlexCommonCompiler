using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.Compiler.RepresentationalISA
{
    /// <summary>
    /// A base of a representational systemn that will accept a visitor that traverses things
    /// </summary>
    /// <typeparam name="T">The operations that the core allows</typeparam>
    abstract class RepresentationalBase<T>
    {
        public T Operation { get; private set; }

        protected RepresentationalBase(T op)
        {
            Operation = op;
        }
    }
}
