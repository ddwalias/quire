using System.Windows;
using Quire.EquationEditor.View;

namespace Quire.Controls.Internal;

internal partial class VisualLinearBinaryOperatorNode
{
    protected readonly VisualSymbolNode _operator;
    protected double _topLineBaseline;

    public override bool IsMonoLine
    {
        get => VisualSymbolNode.GetIsMonoLine(OperatorElement);
        set => VisualSymbolNode.SetIsMonoLine(OperatorElement, value);
    }

    public override IVisualMathNode Operator => _operator;

    public override VisualMathNode OperatorElement => _operator;

    public override Rect ScreenInkBounds => _operator.ScreenInkBounds;

    public VisualLinearBinaryOperatorNode()
        : this(null)
    {
    }

    public VisualLinearBinaryOperatorNode(VisualSymbolNode opSymbol)
    {
        _topLineBaseline = 0.0;
        _operator = opSymbol;
        Children.Add(_operator);
    }
}
