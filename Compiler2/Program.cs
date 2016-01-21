using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using Compiler2.Compiler;
using Compiler2.Compiler.Executables;
using System.Reflection.PortableExecutable;
using Compiler2.Compiler.ExternalResolver;
using Compiler2.Compiler.Binary;
using Compiler2.Compiler.Dummy;
using Compiler2.Compiler.RepresentationalISA;

namespace Compiler2
{
    class Entry
    {
        private static ICompileStage<object> GetCompiler(CompilerSettings settings)
        {
            var instructionEmitter = GetEmitter(settings);

            var dummyGenerator = new IsaGenerator(settings);
            var exernalResolver = settings.ExecutableType == ExecutableType.PortableExecutable ? new WindowsResolver(settings, instructionEmitter) : null;
            var orderer = new SectionOrderer(settings);
            var addresser = new Addresser(settings);
            var physAddress = new PhysicalAddresser(settings);
            var exeWriter = settings.ExecutableType == ExecutableType.PortableExecutable ? new PeWriter(settings) : null;

            dummyGenerator.Next(exernalResolver);

            var before = exernalResolver;
            var after = orderer;

            /*if(settings.ExecutableType == ExecutableType.PortableExecutable)
            {
                var rsrcEmitter = new ResourceSectionEmitter(settings);

                before.Next(rsrcEmitter);
                rsrcEmitter.Next(after);
            }*/
            before.Next(after);

            orderer.Next(addresser);
            addresser.Next(physAddress);
            physAddress.Next(exeWriter);

            return dummyGenerator;
        }

        private static InstructionEmitter GetEmitter(CompilerSettings settings)
        {
            switch (settings.ISA)
            {
                case ISA.x86_64:
                case ISA.x86:
                    return new Amd64Emitter();
                default:
                    return null;
            }
        }

        static void Main(string[] args)
        {
            /*var prog = new Program.Program();

            var parser = new Parser.Parser();
            parser.ParseFile(prog, "b.exe");

            Console.WriteLine();*/

            var settings = new CompilerSettings()
            {
                Is64Bit = true,
                ExecutableType = ExecutableType.PortableExecutable,
                ISA = ISA.x86_64,
                Output = "aout.exe"
            };

            var test = new Dummy.RepIsaGenerator(settings);
            test.Process(null);

            var stage = OperationFactory<Amd64Operation>.Instance;

            //var compiler = GetCompiler(settings);
            //compiler.Process(null);            
        }

        
    }
}
