using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compiler2.AST;
using P = Compiler2.Program;
using System.IO;
using System.Reflection.PortableExecutable;
using System.Reflection.Metadata;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;

namespace Compiler2.Parser.CIL
{
    class CilParser : IParser, IDisposable
    {
        internal static readonly IList<string> IgnoreClassList = new List<string>()
        {
            "<Module>"
        };

        public CilParser()
        {
        }

        public void Parse(P.Program program, string filename)
        {
            var context = new ParseContext(program);

            var ia = new InspectableAssembly(filename, context);
            EntryPointQueue(ia, context);

            while(context.Todo.Count > 0)
            {
                context.Todo.Dequeue().Process();
            }
        }

        public void Dispose()
        {
            
        }

        private void EntryPointQueue(InspectableAssembly ia, ParseContext context)
        {
            foreach (var typeHandle in ia.Reader.TypeDefinitions)
            {
                var parseTarget = ia.CreateParseTarget(typeHandle);
                parseTarget.MethodName = "Main";

                if (IgnoreClassList.Contains(parseTarget.ClassName))
                    continue;

                context.AddTodo(parseTarget);
            }
        }      

               
    }
}
