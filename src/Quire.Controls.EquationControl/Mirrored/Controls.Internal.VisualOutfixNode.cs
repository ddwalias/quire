using System.Windows;
using Quire.EquationEditor.View;

namespace Quire.Controls.Internal;

internal abstract partial class VisualOutfixNode : VisualMathNode, IVisualOutfixNode, IVisualMathNode, IVisualNode
{
    public abstract IVisualMathNode LeftFence { get; }

    public abstract IVisualMathNode RightFence { get; }

    public override IVisualMathNode[] NavigatableParts => new IVisualMathNode[2] { LeftFence, RightFence };

    public abstract VisualMathNode[] NodeChildren { get; }

    public abstract Size DesiredSizeWithoutFences { get; }

    public abstract bool Horizontal { get; }

    public abstract void SetParensVisibility(Visibility visibility);

    protected VisualOutfixNode()
    {
    }
}
