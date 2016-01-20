using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.Program
{
    class FieldSignature
    {
        public ElementType Type { get; set; }
        public bool IsVolatile { get; set; }

        public FieldSignature()
        {

        }
    }
}
