using Quire.Calculation.Core;
using Quire.EquationEditor.Tree;
using Quire.EquationEditor.Tree.Transform;

namespace Quire.Native.PtcEquationEditor;

internal partial class u : t, IEmptyResultNode, IResultNode, IVisitable<IResultNodeVisitor>
{
    public override MathNode bo(IResultTreeFormatter A_0)
    {
        return A_0.FormatResult(this);
    }

    public override t bp()
    {
        return new u();
    }

    public override void Accept(IResultNodeVisitor visitor)
    {
        visitor.Visit(this);
    }

    public u()
    {
    }
}
