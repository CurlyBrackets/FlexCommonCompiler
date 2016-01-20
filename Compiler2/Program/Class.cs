using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.Program
{
    class Class
    {
        public string Name { get; private set; }

        public IDictionary<string, Method> Methods
        {
            get; private set;
        }

        public IDictionary<string, Field> InstanceFields
        {
            get; private set;
        }

        public IDictionary<string, Field> StaticFields
        {
            get; private set;
        }

        public Class(string fqName)
        {
            Name = fqName;
            Methods = new Dictionary<string, Method>();
            InstanceFields = new Dictionary<string, Field>();
            StaticFields = new Dictionary<string, Field>();
        }
    }
}
