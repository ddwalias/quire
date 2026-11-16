using Quire.Calculation.Core;
using Quire.EquationEditor.Tree;
using Quire.EquationEditor.Tree.Transform;

namespace Quire.Native.PtcEquationEditor;

internal partial class aa : t, IFunctionResultNode, IResultNode, IVisitable<IResultNodeVisitor>
{
    private string a;

    public string Description
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
        return new aa { Description = Description };
    }

    public override void Accept(IResultNodeVisitor visitor)
    {
        visitor.Visit(this);
    }

    public aa()
    {
    }
}
