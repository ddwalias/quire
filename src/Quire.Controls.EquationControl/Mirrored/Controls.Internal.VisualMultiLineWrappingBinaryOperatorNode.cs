using System.Windows;
using Quire.EquationEditor.View;

namespace Quire.Controls.Internal;

internal partial class VisualMultiLineWrappingBinaryOperatorNode
{
    private double _firstNodeArrangeVoffset;
    private double _secondNodeArrangeVoffset;
    private double _firstNodeArrangeHoffset;
    private double _secondNodeArrangeHOffset;
    private readonly VisualWrappingSign _wrappingSign;
    private Rect _wrappingSignHitRectangle;
    private Point _firstOperandTopLeft;
    private Point _secondOperandTopLeft;
    private Point _operatorTopLeft;
    private Point _wrappingSignTopLeft;

    public override bool IsMultiline => true;

    public VisualWrappingSign WrappingSign => _wrappingSign;

    protected override bool HasFirstOperandAbove => true;

    protected override bool HasFirstOperandAtLeft => false;

    private Rect ComputeOperatorRectangle()
    {
        Point val = new Point(_operatorTopLeft.X, _firstNodeArrangeVoffset + FirstMathNode.DesiredSize.Height);
        Point bottomRight = new Point(_operatorTopLeft.X + _operator.DesiredSize.Width, _secondOperandTopLeft.Y + SecondVariantLineNode.TopLineHeight);
        return new Rect(val, bottomRight);
    }
}
