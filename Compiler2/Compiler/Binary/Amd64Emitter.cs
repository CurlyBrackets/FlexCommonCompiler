using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.Compiler.Binary
{
    class Amd64Emitter : InstructionEmitter
    {
        public override IList<AddressIndependentThing> FarJump(JumpTarget target)
        {
            switch (target.Type)
            {
                case JumpTargetType.Relative:
                    return new List<AddressIndependentThing>()
                    {
                        new AddressIndependentByte(0xFF),
                        new AddressIndependentByte(0x25),
                        new AddressIndependentAddress(target.FullLabel)
                    };
                case JumpTargetType.External:
                    return new List<AddressIndependentThing>()
                    {
                        new AddressIndependentExternalCall(target.Label, target.Module)
                    };
                default:
                    throw new Exception("blur");
            }
        }
    }
}
