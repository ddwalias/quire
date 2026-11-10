using System.Windows;
using Quire.Controls.Core;

namespace Quire.Controls.Internal;

internal partial class VisualTransposeNode
{
    private double _pushFirstOpDown;

    public override double TextBaselineY => Child.TextBaselineY + _pushFirstOpDown;

    public override double EffectiveBaselineY => Child.EffectiveBaselineY + _pushFirstOpDown;

    public override double EffectiveScriptBaselineY => OperatorElement.EffectiveScriptBaselineY;

    public VisualTransposeNode()
        : base(new VisualSymbolNode('T'), isPrefix: false)
    {
        SetOperatorFontProperties(OperatorElement.MathFormat);
        SetOperatorElementFontSize(MathFormat);
    }

    private void SetOperatorElementFontSize(IMathFormat mathFormat)
    {
        OperatorElement.MathFormat.FontSize = FontManager.GetScaledFontSize(mathFormat, FontConstants.ScriptFontSizeRatio);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        VisualMathNode child = Child;
        VisualMathNode operatorElement = OperatorElement;
        child.Measure(availableSize);
        operatorElement.Measure(availableSize);
        _pushFirstOpDown = operatorElement.TextBaselineY - child.EffectiveScriptBaselineY;
        if (_pushFirstOpDown < 0.0)
        {
            _pushFirstOpDown = 0.0;
        }

        double height;
        ref Point alignmentPoint = ref _alignmentPoint;
        Point alignmentPoint2 = child.AlignmentPoint;
        alignmentPoint.Y = alignmentPoint2.Y + _pushFirstOpDown;
        height = child.DesiredSize.Height + _pushFirstOpDown;
        double width = child.DesiredSize.Width + operatorElement.DesiredSize.Width;
        FireMeasureOverideCompleted();
        return new Size(width, height);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        VisualMathNode child = Child;
        VisualMathNode operatorElement = OperatorElement;
        child.Arrange(new Rect(new Point(0.0, _pushFirstOpDown), child.DesiredSize));
        operatorElement.Arrange(new Rect(new Point(child.DesiredSize.Width, 0.0), operatorElement.DesiredSize));
        return DesiredSize;
    }

    public override void HandleMathFormatChanged(DependencyPropertyChangedEventArgs e)
    {
        base.HandleMathFormatChanged(e);
        SetOperatorElementFontSize(MathFormat);
    }
}
