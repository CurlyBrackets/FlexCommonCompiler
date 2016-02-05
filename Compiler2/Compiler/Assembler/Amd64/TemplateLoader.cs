using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compiler2.Utils;
using Compiler2.Compiler.RepresentationalISA;
using System.Reflection;
using System.Xml;

namespace Compiler2.Compiler.Assembler.Amd64
{
    class TemplateLoader
    {
        public FiveKeyDictionary<Amd64Operation, OperandType, OperandSize, OperandType, OperandSize, BinaryInstruction> BinaryInstructions { get; private set; }
        public ThreeKeyDictionary<Amd64Operation, OperandType, OperandSize, UnaryInstruction> UnaryInstructions { get; private set; }

        public TemplateLoader()
        {
            BinaryInstructions = new FiveKeyDictionary<Amd64Operation, OperandType, OperandSize, OperandType, OperandSize, BinaryInstruction>();
            UnaryInstructions = new ThreeKeyDictionary<Amd64Operation, OperandType, OperandSize, UnaryInstruction>();
        }

        public void Load(string name)
        {
            var assembly = Assembly.GetExecutingAssembly();

            using (var stream = assembly.GetManifestResourceStream(name))
            {
                var xml = new XmlDocument();
                xml.Load(stream);

                var root = xml.LastChild;
                ProcessList(root.ChildNodes);                
            }
        }

        public void ProcessList(XmlNodeList list)
        {
            foreach(XmlNode node in list)
            {
                var el = node as XmlElement;
                if (el != null)
                {
                    switch (el.Name)
                    {
                        case "Set":
                            ProcessList(el.ChildNodes);
                            break;
                        case "BinaryInstruction":
                            ProcessBinaryOperation(el.ChildNodes);
                            break;
                        case "UnaryInstruction":
                            ProcessUnaryOperation(el.ChildNodes);
                            break;
                    }
                }
            }
        }

        private void ProcessBinaryOperation(XmlNodeList list)
        {
            Amd64Operation operation = Amd64Operation.Move;
            Operand[] left = null, right = null;
            byte[] opcode = null;
            IList<StaticField> fields = new List<StaticField>();

            foreach(XmlNode node in list)
            {
                var el = node as XmlElement;
                if(el != null)
                {
                    switch (el.Name)
                    {
                        case "Operation":
                            operation = (Amd64Operation) Enum.Parse(typeof(Amd64Operation), el.InnerText);
                            break;
                        case "OpCode":
                            var parts = el.InnerText.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            opcode = new byte[parts.Length];
                            for(int i = 0; i < parts.Length; i++)
                            {
                                opcode[i] = byte.Parse(parts[i], System.Globalization.NumberStyles.HexNumber);
                            }
                            break;
                        case "Operand":
                            var position = (OperandPosition)Enum.Parse(typeof(OperandPosition), el.GetAttribute("Position"));
                            var type = (OperandType)Enum.Parse(typeof(OperandType), el.GetAttribute("Type"));
                            var sizes = ParseSizes(el.GetAttribute("Size"));
                            var epos = !el.HasAttribute("EncodingPosition") ? EncodingPosition.None : (EncodingPosition)Enum.Parse(typeof(EncodingPosition), el.GetAttribute("EncodingPosition"));

                            var ops = new Operand[sizes.Length];
                            for(int i=0;i<sizes.Length;i++)
                                ops[i] = new Operand(type, sizes[i], epos);

                            if (position == OperandPosition.Left)
                                left = ops;
                            else
                                right = ops;
                            break;
                        case "StaticField":
                            var field = new StaticField(
                                byte.Parse(el.GetAttribute("Value"), System.Globalization.NumberStyles.HexNumber),
                                (EncodingPosition)Enum.Parse(typeof(EncodingPosition), el.GetAttribute("EncodingPosition")));
                            fields.Add(field);
                            break;
                    }
                }
            }

            foreach(var lop in left)
            {
                foreach(var rop in right)
                {
                    var inst = new BinaryInstruction(opcode, lop, rop) { Fields = fields };
                    BinaryInstructions[operation, lop.Type, lop.Size, rop.Type, rop.Size] = inst;
                }
            }
        }

        private void ProcessUnaryOperation(XmlNodeList list)
        {
            Amd64Operation operation = Amd64Operation.Move;
            byte[] opcode = null;
            Operand[] ops = null;

            foreach (XmlNode node in list)
            {
                var el = node as XmlElement;
                if (el != null)
                {
                    switch (el.Name)
                    {
                        case "Operation":
                            operation = (Amd64Operation)Enum.Parse(typeof(Amd64Operation), el.InnerText);
                            break;
                        case "OpCode":
                            var parts = el.InnerText.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            opcode = new byte[parts.Length];
                            for (int i = 0; i < parts.Length; i++)
                            {
                                opcode[i] = byte.Parse(parts[i], System.Globalization.NumberStyles.HexNumber);
                            }
                            break;
                        case "Operand":
                            var type = (OperandType)Enum.Parse(typeof(OperandType), el.GetAttribute("Type"));
                            var sizes = ParseSizes(el.GetAttribute("Size"));
                            var epos = !el.HasAttribute("EncodingPosition") ? EncodingPosition.None : (EncodingPosition)Enum.Parse(typeof(EncodingPosition), el.GetAttribute("EncodingPosition"));

                            ops = new Operand[sizes.Length];
                            for (int i = 0; i < sizes.Length; i++)
                                ops[i] = new Operand(type, sizes[i], epos);
                            
                            break;
                        case "StaticField":
                            throw new Exception("Invalid tag for unary instruction");
                    }
                }
            }

            foreach (var op in ops)
            {
                UnaryInstructions[operation, op.Type, op.Size] = new UnaryInstruction(opcode, op);
            }
        }

        public OperandSize[] ParseSizes(string sizes)
        {
            var ret = new List<OperandSize>();

            var parts = sizes.Split('|');
            foreach(var p in parts)
            {
                ret.Add((OperandSize)Enum.Parse(typeof(OperandSize), "S" + p));
            }

            return ret.ToArray();
        }
    }
}
