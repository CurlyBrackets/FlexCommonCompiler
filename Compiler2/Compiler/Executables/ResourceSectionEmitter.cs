using Compiler2.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.Compiler.Executables
{
    class ResourceSectionEmitter : CompileStage<FinalProgram<AddressIndependentThing>, FinalProgram<AddressIndependentThing>>
    {
        private static readonly byte[] Tree1 = new byte[]
        {
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00,
            0x18, 0x00, 0x00, 0x00, 0x18, 0x00, 0x00, 0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x01, 0x00, 0x00, 0x00, 0x30, 0x00, 0x00, 0x80,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00,
            0x09, 0x04, 0x00, 0x00, 0x48, 0x00, 0x00, 0x00 
        };

        private static readonly byte[] Tree2 = new byte[]
        {
            0x7D, 0x01, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
        };

        private const string XmlManifest =
            "<?xml version='1.0' encoding='UTF-8' standalone='yes'?>" +
            "<assembly xmlns='urn:schemas-microsoft-com:asm.v1' manifestVersion='1.0'>" +
                @"<trustInfo xmlns=""urn:schemas-microsoft-com:asm.v3"">" +
                    "<security>" +
                        "<requestedPriviledges>" +
                            "<requestedExecutionLevel level='asInvoker' uiAccess='false' />" +
                        "</requestedPriviledges>" +
                    "</security>" +
                "</trustInfo>" +
              "</assembly>";

        public ResourceSectionEmitter(CompilerSettings settings)
            : base(settings)
        {
            
        }

        protected override FinalProgram<AddressIndependentThing> ProcessCore(FinalProgram<AddressIndependentThing> input)
        {
            var rsrc = new Section<AddressIndependentThing>()
            {
                Name = ".rsrc",
                Flags = ESectionFlags.Read | ESectionFlags.InitializedData,
                Data = new List<AddressIndependentThing>()
            };

            rsrc.Data.Add(new AddressIndependentLabel(Addresser.ResourcesLabel));
            rsrc.Data.AddRange(Tree1.Convert());
            rsrc.Data.Add(new AddressIndependentAddress("Rsrc::XmlManifest"));
            rsrc.Data.AddRange(Tree2.Convert());
            rsrc.Data.Add(new AddressIndependentLabel("Rsrc::XmlManifest"));
            rsrc.Data.AddMany(Encoding.ASCII.GetBytes(XmlManifest).Convert(), new byte[3].Convert());

            input.Sections.Add(rsrc);
            input.ResourcesSize = rsrc.Data.GetOffset();

            return input;
        }
    }
}
