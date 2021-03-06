﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.Compiler.Dummy
{
    class IsaGenerator : CompileStage<object, FinalProgram<AddressIndependentThing>>
    {
        #region TextBlocks

        private static readonly byte[] _startA = new byte[]
        {
            0x48, 0x83, 0xEC, 0x38, //sub rsp,38h
            0x48, 0xC7, 0x44, 0x24, 0x20, 0x00, 0x00, 0x00, 0x00,
            0x48, 0x8D, 0x44, 0x24, 0x20,
            0x48, 0x89, 0x44, 0x24, 0x28,
            0x48, 0x8B, 0x4C, 0x24, 0x28,
            0xE8
        },
        _startB = new byte[]
        {
            0x8B, 0xC8, 0xE8
        },
        _startC = new byte[]
        {
            0x48,0x83, 0xC4, 0x38, 0xC3, 0xCC, 0xCC, 0xCC
        },
        mainA = new byte[]
        {
            0x48, 0x89, 0x4C, 0x24,0x08,
            0x48, 0x83, 0xEC, 0x48,
            0xB9, 0xF5, 0xFF, 0xFF, 0xFF,
            0xE8
        },
        mainB = new byte[]
        {
            0x48, 0x89, 0x44,0x24, 0x30,
            0x48, 0xC7, 0x44,0x24, 0x20, 0x00, 0x00, 0x00, 0x00,
            0x45, 0x33, 0xc9,
            0x41, 0xb8, 0x0d, 0x00, 0x00, 0x00,
            0x48, 0x8D, 0x15
        },
        mainC = new byte[]
        {
            0x48, 0x8b, 0x4c, 0x24, 0x30,
            0xe8
        },
        mainD = new byte[]
        {
            0x33, 0xc0,
            0x48, 0x83, 0xc4, 0x48,
            0xC3
        },
        data = new byte[]
        {
            0x48, 0x65, 0x6c, 0x6c, 0x6f, 0x20, 0x77, 0x6f, 0x72, 0x6c, 0x64, 0x21, 0x0a, 0x00
        };

        #endregion

        public IsaGenerator(CompilerSettings settings)
            : base(settings)
        {

        }

        protected override FinalProgram<AddressIndependentThing> ProcessCore(object ignore)
        {
            return new FinalProgram<AddressIndependentThing>() {
                Sections = new List<Section<AddressIndependentThing>>()
                {
                    GenerateText(),
                    //GenerateReadonlyData(),
                    GenerateData()
                }
            };
        }

        private Section<AddressIndependentThing> GenerateText()
        {
            var data = new List<AddressIndependentThing>();
            data.Add(new AddressIndependentLabel(Addresser.FunctionStartPrefix + "0"));
            data.AddRange(_startA.Convert());
            data.Add(new AddressIndependentAddress("main"));
            data.AddRange(_startB.Convert());
            data.Add(new AddressIndependentAddress("ExitProcess"));
            data.AddRange(_startC.Convert());
            data.Add(new AddressIndependentLabel(Addresser.FunctionEndPrefix + "0"));

            data.Add(new AddressIndependentLabel(Addresser.FunctionStartPrefix + "1")); // generated by sequencer
            data.Add(new AddressIndependentLabel("main"));
            data.AddRange(mainA.Convert());
            data.Add(new AddressIndependentAddress("GetStdHandle"));
            data.AddRange(mainB.Convert());
            data.Add(new AddressIndependentAddress("hello_string"));
            data.AddRange(mainC.Convert());
            data.Add(new AddressIndependentAddress("WriteFile"));
            data.AddRange(mainD.Convert());
            data.Add(new AddressIndependentLabel(Addresser.FunctionEndPrefix + "1"));

            data.Add(new AddressIndependentLabel("ExitProcess"));
            data.Add(new AddressIndependentExternalCall("ExitProcess", "KERNEL32.dll"));
            data.Add(new AddressIndependentLabel("GetStdHandle"));
            data.Add(new AddressIndependentExternalCall("GetStdHandle", "KERNEL32.dll"));
            data.Add(new AddressIndependentLabel("WriteFile"));
            data.Add(new AddressIndependentExternalCall("WriteFile", "KERNEL32.dll"));

            return new Section<AddressIndependentThing>()
            {
                Name = ".text",
                Flags = ESectionFlags.Code | ESectionFlags.Execute | ESectionFlags.Read,
                Data = data
            };
        }

        /*private Section<AddressIndependentThing> GenerateReadonlyData()
        {
            
        }*/

        private Section<AddressIndependentThing> GenerateData()
        {
            var data = new List<AddressIndependentThing>();

            data.Add(new AddressIndependentLabel("hello_string"));
            data.AddRange(IsaGenerator.data.Convert());

            return new Section<AddressIndependentThing>()
            {
                Name = ".data",
                Flags = ESectionFlags.InitializedData | ESectionFlags.Read | ESectionFlags.Write,
                Data = data
            };
        }

    }
}
