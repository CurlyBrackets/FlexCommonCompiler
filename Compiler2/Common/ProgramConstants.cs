using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.Common
{
    class ProgramConstants
    {
        public Dictionary<string, string> StringConstants { get; private set; }
        public Dictionary<string, long> IntegerConstants { get; private set; }
        public Dictionary<string, double> FloatConstants { get; private set; }

        public ProgramConstants()
        {
            StringConstants = new Dictionary<string, string>();
            IntegerConstants = new Dictionary<string, long>();
            FloatConstants = new Dictionary<string, double>();
        }

        public int Add(string key, string val)
        {
            StringConstants.Add(key, val);
            return StringConstants.Count - 1;
        }

        public int Add(string key, long val)
        {
            IntegerConstants.Add(key, val);
            return IntegerConstants.Count - 1;
        }

        public int Add(string key, double val)
        {
            FloatConstants.Add(key, val);
            return FloatConstants.Count - 1;
        }
    }
}
