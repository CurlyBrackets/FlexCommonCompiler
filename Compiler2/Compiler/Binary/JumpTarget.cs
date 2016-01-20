using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.Compiler.Binary
{
    enum JumpTargetType
    {
        External,
        Absolute,
        Relative
    }

    class JumpTarget
    {
        public JumpTargetType Type { get; set; }
        public string Module { get; set; }
        public string Label { get; set; }

        public const string ImportedJoin = "@";

        public string FullLabel
        {
            get
            {
                if (!string.IsNullOrEmpty(Module))
                    return string.Join(ImportedJoin, Module, Label);
                return Label;
            }
        }

        public JumpTarget(JumpTargetType type, string label)
        {
            Type = type;
            Label = label;
        }
    }
}
