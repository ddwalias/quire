namespace Quire.Controls.Internal;

internal abstract partial class VisualUnaryOperatorNodeForEditor
{
    protected VisualUnaryOperatorNodeForEditor()
    {
    }

    public abstract VisualMathNode Child { get; set; }

    public abstract VisualMathNode OperatorElement { get; }
}
