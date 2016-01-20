using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.Dummy
{
    class GenericIsaGenerator : CompileStage<object, object>
    {
        public GenericIsaGenerator(CompilerSettings settings)
            : base(settings)
        {

        }

        protected override object ProcessCore(object input)
        {
            throw new NotImplementedException();
        }
    }
}
