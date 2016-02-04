using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.Compiler
{
    struct ProcessArgs
    {
        public CompilerSettings Settings;

        public BinaryWriter Writer;
        public IList<AddressIndependentThing> Intermediate;

        public long Base;
        public IDictionary<string, long> Symbols;
        public bool FinalPass;

        public long GetOffset()
        {
            if (FinalPass)
                return Writer.BaseStream.Position;
            else
                return Intermediate.GetOffset();
        }
    }

    abstract class AddressIndependentThing
    {
        public abstract bool Process(ProcessArgs args);
    }

    class AddressIndependentByte : AddressIndependentThing
    {
        private byte m_val;

        public AddressIndependentByte(byte val)
        {
            m_val = val;
        }

        public override bool Process(ProcessArgs args)
        {
            if(args.FinalPass)
                args.Writer.Write(m_val);
            else
                args.Intermediate.Add(this);

            return true;
        }

        public static implicit operator AddressIndependentByte(byte b)
        {
            return AIF.Instance.Byte(b);
        }
    }

    class AddressIndependentLabel : AddressIndependentThing
    {
        private string m_val;

        public AddressIndependentLabel(string val)
        {
            m_val = val;
        }

        public override bool Process(ProcessArgs args)
        {
            args.Symbols.Add(m_val, args.GetOffset() + args.Base);
            return true;
        }
    }

    class AddressIndependentAddress : AddressIndependentThing
    {
        private string m_val;

        public bool Is64Bit
        {
            get; set;
        }

        public bool Absolute { get; set; }

        public AddressIndependentAddress(string label)
        {
            m_val = label;
            Is64Bit = false;
            Absolute = false;
        }

        public override bool Process(ProcessArgs args)
        {
            if (!args.Symbols.ContainsKey(m_val))
            {
                args.Intermediate.Add(this);
                return false;
            }

            long targetAddress = args.Symbols[m_val];
            long localAddress = args.GetOffset() + args.Base;

            dynamic resolved = 0;
            if (Is64Bit)
            {
                localAddress += sizeof(long);
                resolved = targetAddress - (Absolute ? 0L : localAddress);
            }
            else
            {
                localAddress += sizeof(int);
                resolved = (int)(targetAddress - (Absolute ? 0L : localAddress));
            }

            if (args.FinalPass)
                args.Writer.Write(resolved);
            else
            {
                byte[] val = BitConverter.GetBytes(resolved);
                foreach (var b in val)
                    args.Intermediate.Add(new AddressIndependentByte(b));
            }

            return true;
        }
    }

    class AddressIndependentExternalCall : AddressIndependentThing
    {
        public string Name { get; set; }
        public string Module { get; set; }

        public AddressIndependentExternalCall(string name, string module = "")
        {
            Name = name;
            Module = module;
        }

        public override bool Process(ProcessArgs args)
        {
            throw new Exception("This shouldn't exist at the point where process is called");
        }
    }

    static class Extenstions
    {
        public static AddressIndependentByte[] Convert(this byte[] raw)
        {
            var ret = new AddressIndependentByte[raw.Length];

            for (int i = 0; i < raw.Length; i++)
                ret[i] = new AddressIndependentByte(raw[i]);

            return ret;
        }

        public static long GetOffset(this IEnumerable<AddressIndependentThing> things)
        {
            long ret = 0;
            foreach (var ait in things)
            {
                if (ait is AddressIndependentByte)
                    ret++;
                else if (ait is AddressIndependentAddress)
                {
                    if (((AddressIndependentAddress)ait).Is64Bit)
                        ret += sizeof(long);
                    else
                        ret += sizeof(int);
                }
            }

            return ret;
        }

        
    }
}
