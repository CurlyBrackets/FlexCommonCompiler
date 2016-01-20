using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.Program
{
    class Field : LValue
    {
        public string Name { get; set; }
        public FieldSignature Signature { get; set; }
        public Constant DefaultValue { get; set; }

        public Field()
        {

        }

        public override ElementType ResolveType()
        {
            return Signature.Type;
        }
    }
}
