using Compiler2.AST;
using Compiler2.AST.Expressions;
using Compiler2.AST.Statements;
using Compiler2.Program;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.Parser.CIL
{
    class ILReader
    {
        public static INode Read(InspectableAssembly ia, MethodSignature signature, MethodBodyBlock rawBody)
        {
            /*Console.WriteLine("=== IL Bytes ===");
            foreach (var b in ilBytes)
                Console.Write("{0:X2} ", b);
            Console.WriteLine();*/

            var parser = new ILMethodParser(ia, signature, rawBody);
            return parser.Parse();
        }

        #region ArgumentLength

        private const int MetadataHandleSize = 4;
        public static int ArgumentLength(OpCode opcode)
        {
            switch (opcode)
            {
                case OpCode.beq:
                case OpCode.bge:
                case OpCode.bge_un:
                case OpCode.bgt:
                case OpCode.bgt_un:
                case OpCode.ble:
                case OpCode.ble_un:
                case OpCode.blt:
                case OpCode.blt_un:
                case OpCode.bne_un:
                case OpCode.br:
                case OpCode.brfalse:
                case OpCode.brtrue:
                    return 4;
                case OpCode.beq_s:
                case OpCode.bge_s:
                case OpCode.bge_un_s:
                case OpCode.bgt_s:
                case OpCode.bgt_un_s:
                case OpCode.ble_s:
                case OpCode.ble_un_s:
                case OpCode.blt_s:
                case OpCode.blt_un_s:
                case OpCode.bne_un_s:
                case OpCode.br_s:
                case OpCode.brfalse_s:
                case OpCode.brtrue_s:
                    return 1;

                case OpCode.ldarg_s:
                case OpCode.ldarga_s:
                    return 1;

                case OpCode.ldc_i4:
                case OpCode.ldc_r4:
                    return 4;

                case OpCode.ldc_i8:
                case OpCode.ldc_r8:
                    return 8;

                case OpCode.ldc_i4_s:
                    return 1;

                case OpCode.ldloc_s:
                case OpCode.ldloca_s:
                    return 1;

                case OpCode.leave:
                    return 4;
                case OpCode.leave_s:
                    return 1;

                case OpCode.starg_s:
                    return 1;

                case OpCode.stloc_s:
                    return 1;

                case OpCode.isinst:
                case OpCode.ldstr:
                case OpCode.call:
                case OpCode.callvirt:
                case OpCode.ldsfld:
                case OpCode.ldsflda:
                case OpCode.newobj:
                case OpCode.newarr:
                    return MetadataHandleSize;
                    
                default:
                    return 0;
            }
        }

        public static int ArgumentLength(ExtendedOpCode opcode)
        {
            switch (opcode)
            {
                case ExtendedOpCode.ldarg:
                case ExtendedOpCode.ldarga:
                    return 2;

                case ExtendedOpCode.ldloc:
                case ExtendedOpCode.ldloca:
                    return 2;

                case ExtendedOpCode.starg:
                    return 2;

                case ExtendedOpCode.stloc:
                    return 2;

                default:
                    return 0;
            }
        }

        #endregion

        #region Signature Parsing

        private const string VolatileClassName = "System.Runtime.CompilerServices.IsVolatile";

        public static FieldSignature ParseFieldSignature(InspectableAssembly ia, byte[] blob)
        {
            var ret = new FieldSignature();

            int index = 0;
            if (blob[index++] != 0x06)
                throw new Exception("Invalid field signature");

            while(index < blob.Length)
            {
                var type = ReadType(ia, blob, ref index);

                if (type.Type.IsCustomMod()) {
                    if (type.Target == ia.Context.Program.Classes[VolatileClassName])
                        ret.IsVolatile = true;

                    // other custom mods
                }
                else
                    ret.Type = type;
            }

            return ret;
        }

        public static MethodSignature ParseMethodSignature(InspectableAssembly ia, byte[] blob)
        {
            var ret = new MethodSignature();

            int index = 0;

            ret.Flags = (EMethodFlags)blob[index++];
            if (ret.Flags.HasFlag(EMethodFlags.Generic))
                ret.NumGeneric = UncompressInt(blob, ref index);
     
            ret.NumNormal = UncompressInt(blob, ref index);

            ret.ReturnType = ReadType(ia, blob, ref index);
            if (ret.Flags.HasFlag(EMethodFlags.ExplicitThis))
            {
                ret.ThisType = ReadType(ia, blob, ref index);
                ret.NumNormal--;
            }

            for(int i = 0; i < ret.NumNormal; i++)
            {
                ret.ParameterTypes.Add(ReadType(ia, blob, ref index));
            }

            return ret;
        }

        public static GenericSignature ParseGenericSignature(InspectableAssembly ia, byte[] specBloc)
        {
            int index = 0;
            if (specBloc[index++] != 10)
                throw new Exception("Poop generic signature");

            int count = specBloc[index++];

            var ret = new GenericSignature();
            for (int i = 0; i < count; i++)
                ret.Add(ReadType(ia, specBloc, ref index));

            return ret;
        }

        /*public static MethodSignature SetGenerics(InspectableAssembly ia, byte[] specBloc, MethodSignature @base)
        {
            

            var ret = @base.Clone();
            if (ret.NumGeneric != count)
                throw new Exception("Mismatched generic signature");

            // do parameters
            for(int i = 0; i < ret.ParameterTypes.Count; i++)
            {
                if(ret.ParameterTypes[i].Type == EElementType.MVar)
                    ret.ParameterTypes[i] = types[ret.ParameterTypes[i].Number];
                else if(ret.ParameterTypes[i].HasSecondary && ret.ParameterTypes[i].Secondary.Type == EElementType.Mvar)
                {

                }
            }
            ret.NumGeneric = 0;

            // do return type
            if (ret.ReturnType.Type == EElementType.MVar)
                ret.ReturnType = types[ret.ReturnType.Number];
            
            return ret;
        }*/

        public static IList<ElementType> ParseLocalSignature(InspectableAssembly ia, byte[] blob)
        {
            var ret = new List<ElementType>();

            int index = 0;
            if (blob[index++] != 0x07)
                throw new Exception("Bad locals signature");

            int numberOfEntries = blob[index++];
            for(; numberOfEntries > 0; numberOfEntries--)
            {
                ret.Add(ReadType(ia, blob, ref index));
            }
                
            return ret;
        }

        private static ElementType ReadType(InspectableAssembly ia, byte[] blob, ref int index)
        {
            var ret = new ElementType((EElementType)blob[index++]);
            // class or struct
            if (ret.Type == EElementType.Class || ret.Type == EElementType.ValueType || ret.Type == EElementType.CMod_Required || ret.Type == EElementType.CMod_Optional)
            {
                //next is metadata token?
                var tokenRaw = UncompressInt(blob, ref index);
                int token = (tokenRaw & ~0x03) >> 2;
                switch(tokenRaw & 0x03)
                {
                    case 0: //typedef 
                        token |= 0x02000000;
                        break;
                    case 1: //typeref
                        token |= 0x01000000;
                        break;
                    case 2: //typespec
                        token |= 0x20000000;
                        break;
                }

                var handle = MetadataTokens.Handle(token);
                switch (handle.Kind)
                {
                    case HandleKind.TypeDefinition:
                        ret.Target = ia.GetClassImmediate(token);
                        break;
                    default:
                        throw new Exception("Go fuck yourself handle");
                }
            }
            else if(ret.Type == EElementType.SzArray || ret.Type == EElementType.Reference)
            {
                ElementType next;
                do
                {
                    next = ReadType(ia, blob, ref index);
                    if (next.Type.IsCustomMod())
                        Console.WriteLine("?? SzArray custom mod ??");
                } while (next.Type.IsCustomMod());

                ret.Secondary = next;
            }
            else if(ret.Type == EElementType.MVar)
            {
                ret.Number = UncompressInt(blob, ref index);
            }

            return ret;
        }

        private static void DumpBlob(byte[] blob)
        {
            Console.WriteLine("=== Signature Blob ===");
            foreach (var b in blob)
                Console.Write("{0:X2} ", b);
            Console.WriteLine();
        }

        private const byte SingleByteMask = 0x7F;
        private const byte TwoByteDetect = 0xBF, TwoByteMask = 0x3F;
        private const byte FourByteDetect = 0xDF, FourByteMask = 0x1F;

        private static int UncompressInt(byte[] blob, ref int index)
        {
            var first = blob[index++];
            if ((first & SingleByteMask) == first)
                return first;
            else if ((first & TwoByteDetect) == first)
                return ((first & TwoByteMask) << 8) | blob[index++];
            else if ((first & FourByteDetect) == first)
                return ((first & FourByteMask) << 24) |
                        (blob[index++] << 16) |
                        (blob[index++] << 8) |
                        blob[index++];

            return 0;
        }

        #endregion
    }
}
