using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Compiler2.Parser
{
    class Parser
    {
        private Dictionary<string, IParser> m_internals;

        public Parser()
        {
            m_internals = new Dictionary<string, IParser>();

            var cil = new CIL.CilParser();
            m_internals.Add("exe", cil);
            m_internals.Add("dll", cil);

            var text = new Text.TextParser();
            m_internals.Add("cs", text);
        }

        public void ParseFile(Program.Program program, string filename)
        {
            var ext = Path.GetExtension(filename).Substring(1);
            if (!m_internals.ContainsKey(ext))
                throw new Exception("Unknown file type");

            m_internals[ext].Parse(program, filename);
        }
    }
}
