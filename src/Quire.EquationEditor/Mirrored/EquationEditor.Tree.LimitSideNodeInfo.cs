using System.Collections.Generic;
using System.Linq;
using Quire.Undo;

namespace Quire.EquationEditor.Tree;

public partial class LimitSideNodeInfo
{
    private static readonly IDictionary<LimitSide, string> b;
    private static IDictionary<string, LimitSide> c;

    public override string ValueSExpression => b[Value];

    public new LimitSide Value
    {
        get => (LimitSide)base.Value;
        set => base.Value = value;
    }

    public static bool IsSExpressionOf(string sexp)
    {
        return b.Values.Contains(sexp);
    }

    public static bool IsValidInitializerString(string s)
    {
        return c.ContainsKey(s);
    }

    public LimitSideNodeInfo()
    {
        using (new CommandScope(new CompoundCommand()))
        {
            Value = LimitSide.TwoSided;
        }
    }

    public LimitSideNodeInfo(string limitSide)
    {
        using (new CommandScope(new CompoundCommand()))
        {
            Value = c[limitSide];
        }
    }

    public LimitSideNodeInfo(LimitSideNodeInfo other)
    {
        using (new CommandScope(new CompoundCommand()))
        {
            Value = other.Value;
        }
    }

    protected override object bk()
    {
        return new LimitSideNodeInfo(this);
    }

    public override bool IsEquivalentTo(INodeInfo other)
    {
        if (!(other is LimitSideNodeInfo))
        {
            return false;
        }

        return IsEquivalentTo((LimitSideNodeInfo)other);
    }

    public bool IsEquivalentTo(LimitSideNodeInfo other)
    {
        if (other == null)
        {
            return false;
        }

        return Value == other.Value;
    }
}
