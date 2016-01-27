﻿using Compiler2.Compiler.RepresentationalISA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.Compiler
{


    class IntermediateProgram<T>
    {
        public IDictionary<string, IList<RepresentationalBase<T>>> Functions { get; private set; }
        
        // constants?
        public IDictionary<string, string> StringConstants { get; private set; }

        public IntermediateProgram()
        {
            Functions = new Dictionary<string, IList<RepresentationalBase<T>>>();
            StringConstants = new Dictionary<string, string>();
        }
    }
}
