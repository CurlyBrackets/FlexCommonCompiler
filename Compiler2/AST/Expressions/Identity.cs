using Compiler2.Program;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.AST.Expressions
{
    enum IdentityType
    {
        LValue,
        Expression
    }

    class Identity : Expression
    {
        public IdentityType Type { get; private set; }
        public LValue LValue { get; private set; }
        public Expression Expression { get; private set; }

        public Identity(LValue lval)
        {
            Type = IdentityType.LValue;
            LValue = lval;
        }

        public Identity(Expression exp)
        {
            Type = IdentityType.Expression;
            Expression = exp;
        }

        public override ElementType EvaluateType(out bool indeterminate)
        {
            switch (Type)
            {
                case IdentityType.Expression:
                    return Expression.EvaluateType(out indeterminate);
                case IdentityType.LValue:
                    if (LValue is Field)
                    {
                        var f = LValue as Field;
                        indeterminate = f.Signature.Type.HasTarget && f.Signature.Type.ConcreteTargets.Count < 1;
                    }
                    else
                        indeterminate = false;
                    return LValue.ResolveType();                    
            }

            return base.EvaluateType(out indeterminate);
        }
    }
}
