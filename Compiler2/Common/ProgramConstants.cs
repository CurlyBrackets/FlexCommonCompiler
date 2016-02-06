using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.Common
{
    class ProgramConstants
    {
        private const string BasePrefix = "Constant::";
        private const string StringPrefix = "String::";
        private const string IntegerPrefix = "Integer::";
        private const string FloatPrefix = "Float::";

        public Dictionary<string, string> StringConstants { get; private set; }
        public Dictionary<string, long> IntegerConstants { get; private set; }
        public Dictionary<string, double> FloatConstants { get; private set; }

        public ProgramConstants()
        {
            StringConstants = new Dictionary<string, string>();
            IntegerConstants = new Dictionary<string, long>();
            FloatConstants = new Dictionary<string, double>();
        }

        public string Add(string val)
        {
            var key = BasePrefix + StringPrefix + StringConstants.Count;
            StringConstants.Add(key, val);
            return key;
        }

        public string Add(long val)
        {
            var key = BasePrefix + IntegerPrefix + IntegerConstants.Count;
            IntegerConstants.Add(key, val);
            return key;
        }

        public string Add(double val)
        {
            var key = BasePrefix + FloatPrefix + FloatConstants.Count;
            FloatConstants.Add(key, val);
            return key;
        }
    }
}
