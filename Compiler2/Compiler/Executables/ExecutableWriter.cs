using Compiler2.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.Compiler.Executables
{
    abstract class ExecutableWriter : CompileStage<FinalProgram<byte>, object>, IDisposable
    {
        protected BinaryWriter Writer { get; private set; }
        private FileStream m_coreStream;

        protected ExecutableWriter(CompilerSettings settings)
            : base(settings)
        {
            m_coreStream = File.OpenWrite(settings.Output);
            Writer = new BinaryWriter(m_coreStream);
        }

        public void Dispose()
        {
            Writer.Close();

            Writer.Dispose();
            m_coreStream.Dispose();
        }

        protected abstract void WriteHeaders(FinalProgram<byte> final);
        protected abstract void WriteSection(Section<byte> section);

        protected override object ProcessCore(FinalProgram<byte> final)
        {
            WriteHeaders(final);

            foreach(var section in final.Sections)
            {
                WriteSection(section);
            }

            long target = Writer.BaseStream.Position.RoundUpTo(0x200);
            while (Writer.BaseStream.Position < target)
                Writer.Write((byte)0x00);

            return null;
        }
    }
}
