using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compiler2.Program;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.Metadata;

namespace Compiler2.Parser.CIL
{
    class ParseContext
    {
        public IDictionary<string, InspectableAssembly> Files { get; private set; }
        public Program.Program Program { get; private set; }
        public Queue<ParseTarget> Todo { get; private set; }

        private IDictionary<int, Class> m_tokenReference;

        public ParseContext(Program.Program program)
        {
            Program = program;
            m_tokenReference = new Dictionary<int, Class>();
            Files = new Dictionary<string, InspectableAssembly>();
            Todo = new Queue<ParseTarget>();
        }

        public void AddClass(InspectableAssembly ia, TypeDefinitionHandle typeHandle, Class c)
        {
            int token = MetadataTokens.GetToken(ia.Reader, typeHandle);
            m_tokenReference.Add(token, c);
            Program.Classes.Add(c.Name, c);
        }

        public void AddTodo(ParseTarget target)
        {
            if (target !=null && !Todo.Contains(target) && !AlreadyParsed(target))
                Todo.Enqueue(target);
        }

        private bool AlreadyParsed(ParseTarget target)
        {
            if (target.Method != null)
                return target.Method.Body != null;

            if (!Program.Classes.ContainsKey(target.ClassName))
                return false;

            var clazz = Program.Classes[target.ClassName];
            if (!target.SpecificMethod)
                return false;

            var key = target.MethodName + target.MethodSignature;
            if(clazz.Methods.ContainsKey(key))
                return clazz.Methods[key].Body != null;

            return false;
        }

        public Class GetClass(int token)
        {
            if (m_tokenReference.ContainsKey(token))
                return m_tokenReference[token];
            return null;
        }
    }
}
