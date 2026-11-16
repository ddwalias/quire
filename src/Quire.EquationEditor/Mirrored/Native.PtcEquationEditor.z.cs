using Quire.Calculation.Core;
using Quire.EquationEditor.Tree;
using Quire.EquationEditor.Tree.Transform;

namespace Quire.Native.PtcEquationEditor;

internal partial class z : t, IStringResultNode, IResultNode, IVisitable<IResultNodeVisitor>
{
    private string a;

    public string Value
    {
        get => a;
        set => a = value;
    }

    public override MathNode bo(IResultTreeFormatter A_0)
    {
        return A_0.FormatResult(this);
    }

    public override t bp()
    {
        return new z { Value = Value };
    }

    public override void Accept(IResultNodeVisitor visitor)
    {
        visitor.Visit(this);
    }

    public z()
    {
    }
}
