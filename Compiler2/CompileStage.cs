using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2
{
    interface ICompileStage<In>
    {
        void Process(In input);
    }

    abstract class CompileStage<In, Out> : ICompileStage<In>
    {
        protected CompilerSettings Settings { get; private set; }
        private ICompileStage<Out> m_next;

        protected CompileStage(CompilerSettings settings)
        {
            Settings = settings;
        }

        protected abstract Out ProcessCore(In input);

        public void Process(In input)
        {
            var o = ProcessCore(input);
            if (m_next != null)
                m_next.Process(o);
        }

        public void Next(ICompileStage<Out> next)
        {
            m_next = next;
        }
    }
}
