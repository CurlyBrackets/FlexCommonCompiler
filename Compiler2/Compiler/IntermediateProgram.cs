using Compiler2.Compiler.RepresentationalISA;
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
        
        public Common.ProgramConstants Constants { get; private set; }

        public IntermediateProgram(Common.ProgramConstants constants = null)
        {
            Functions = new Dictionary<string, IList<RepresentationalBase<T>>>();
            Constants = constants ?? new Common.ProgramConstants();
        }
    }
}
