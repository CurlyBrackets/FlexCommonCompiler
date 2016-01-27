using Compiler2.Compiler.RepresentationalISA;
using Compiler2.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.Compiler.Binary
{
    class BinaryConverter<T> : CompileStage<IntermediateProgram<T>, FinalProgram<AddressIndependentThing>>
    {
        private InstructionEmitter<T> m_emitter;

        public BinaryConverter(CompilerSettings settings, InstructionEmitter<T> emitter)
            : base(settings)
        {
            m_emitter = emitter;
        }

        protected override FinalProgram<AddressIndependentThing> ProcessCore(IntermediateProgram<T> input)
        {
            var ret = new FinalProgram<AddressIndependentThing>()
            {
                Sections = new List<Section<AddressIndependentThing>>()
            };

            ret.Sections.Add(GenerateData(input));
            ret.Sections.Add(GenerateText(input));

            return ret;
        }

        private Section<AddressIndependentThing> GenerateData(IntermediateProgram<T> input)
        {
            var ret = new Section<AddressIndependentThing>()
            {
                Name = ".data",
                Flags = ESectionFlags.InitializedData | ESectionFlags.Read | ESectionFlags.Write
            };

            var data = new List<AddressIndependentThing>();

            // do numeric values first, then strings, then classes
            foreach(var kvp in input.StringConstants)
            {
                data.Add(new AddressIndependentLabel(kvp.Key));
                var bytes = Encoding.ASCII.GetBytes(kvp.Value);
                data.AddRange(bytes.Convert());
                data.Add(new AddressIndependentByte(0));// null terminate (maybe?)
            }

            ret.Data = data;

            return ret;
        }

        private Section<AddressIndependentThing> GenerateText(IntermediateProgram<T> input)
        {
            var ret = new Section<AddressIndependentThing>()
            {
                Name=".text",
                Flags = ESectionFlags.Code | ESectionFlags.Execute | ESectionFlags.Read
            };

            var data = new List<AddressIndependentThing>();
            ret.Data = data;

            int functionIndex = 0;
            foreach(var function in input.Functions)
            {
                data.Add(new AddressIndependentLabel(Addresser.FunctionStartPrefix + functionIndex));
                data.Add(new AddressIndependentLabel(function.Key));
                data.AddMany(GenerateFunction(function.Value));
                data.Add(new AddressIndependentLabel(Addresser.FunctionEndPrefix + functionIndex++));
            }

            return ret;
        }

        private IList<IList<AddressIndependentThing>> GenerateFunction(IList<RepresentationalBase<T>> input)
        {
            var ret = new List<IList<AddressIndependentThing>>();

            foreach (var op in input)
                ret.Add(op.Accept(m_emitter));

            return ret;
        }
    }
}
