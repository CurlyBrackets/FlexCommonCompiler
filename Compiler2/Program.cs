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
using Compiler2.Dummy;
using Compiler2.IR.Stages;

namespace Compiler2
{
    class Entry
    {
        private static ICompileStage<object> GetCompiler(CompilerSettings settings)
        {
            //var instructionEmitter = GetEmitter(settings);
            var generator = new LinearIRGenerator(settings);
            var setupStack = new SetupStackParameters(settings);
            var constanthandler = new ConstantProcessor(settings);
            var argSanitizer = new ArgumentSanitizer(settings);
            var argLifter = new ArgumentLifter(settings);
            var stackAllocator = new StackAllocator(settings);
            var paramLifter = new ParameterLifter(settings);
            var memoryAssExpander = new MemoryAssignmentExpander(settings);

            var instructionEmitter = new Compiler.Assembler.Amd64.Emitter();
            var binaryconverter = new BinaryConverter<Amd64Operation>(settings, instructionEmitter);
            var externalResolver = settings.ExecutableType == ExecutableType.PortableExecutable ? new WindowsResolver<Amd64Operation>(settings, instructionEmitter) : null;
            var orderer = new SectionOrderer(settings);
            var addresser = new Addresser(settings);
            var physAddress = new PhysicalAddresser(settings);
            var exeWriter = settings.ExecutableType == ExecutableType.PortableExecutable ? new PeWriter(settings) : null;

            generator.Next(setupStack);
            setupStack.Next(constanthandler);
            constanthandler.Next(argSanitizer);
            argSanitizer.Next(argLifter);
            argLifter.Next(stackAllocator);
            stackAllocator.Next(paramLifter);
            paramLifter.Next(memoryAssExpander);
            memoryAssExpander.Next(new IRPrinter(settings));
            
            binaryconverter.Next(externalResolver);

            var before = externalResolver;
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

            return generator;
        }

        /*private static InstructionEmitter GetEmitter<T>(CompilerSettings settings)
        {
            switch (settings.ISA)
            {
                case ISA.x86_64:
                case ISA.x86:
                    return new Amd64Emitter();
                default:
                    return null;
            }
        }*/

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
                ISA = ISA.x86,
                Output = "aout.exe"
            };

            var compiler = GetCompiler(settings);
            compiler.Process(null);            
        }

        
    }
}
