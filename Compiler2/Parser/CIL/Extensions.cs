using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.Parser.CIL
{
    static class Extensions
    {
        public static TypeDefinitionHandle GetTypeDefintionByName(this MetadataReader reader, string fqName)
        {
            foreach(var typeHandle in reader.TypeDefinitions)
            {
                var type = reader.GetTypeDefinition(typeHandle);

                var name = reader.GetString(type.Name);
                var ns = reader.GetString(type.Namespace);

                var fullname = string.Join(".", ns, name);
                if (fullname == fqName)
                    return typeHandle;
            }

            return new TypeDefinitionHandle();
        }
    }
}
