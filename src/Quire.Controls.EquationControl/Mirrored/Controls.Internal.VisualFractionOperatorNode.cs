#nullable disable
using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Quire.EquationEditor.View;
using Quire.Wpf;

namespace Quire.Controls.Internal;

internal partial class VisualFractionOperatorNode : VisualBinaryOperatorNode, IVisualBinaryOperatorNode, IVisualMathNode, IVisualNode
{
    private const double DEFAULT_SIDE_OVERHANG = 2.4;
    private const double DEFAULT_FRACTION_BAR_VERTICAL_PAD = 2.4;

    internal static readonly DependencyProperty StaticFractionOverhangProperty = DependencyProperty.Register(
        "SideOverhang",
        typeof(double),
        typeof(VisualFractionOperatorNode),
        new FrameworkPropertyMetadata(2.4, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));

    internal static readonly DependencyProperty StaticFractionBarVerticalPadProperty = DependencyProperty.Register(
        "StaticFractionBarVerticalPad",
        typeof(double),
        typeof(VisualFractionOperatorNode),
        new FrameworkPropertyMetadata(2.4, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));

    private readonly HorizontalFence _fractionBar = new HorizontalFence('―');
    private double _textBaselineY;

    public override IVisualMathNode Operator => _fractionBar;

    public override VisualMathNode OperatorElement => _fractionBar;

    public override double TextBaselineY => _textBaselineY;

    public override double EffectiveBaselineY => ((VisualMathNode)SecondOperand).EffectiveBaselineY + DenominatorY();

    public override double EffectiveScriptBaselineY => ((VisualMathNode)FirstOperand).EffectiveScriptBaselineY;

    private double FractionOverhang => (double)GetValue(StaticFractionOverhangProperty);

    private double FractionBarVerticalPad => (double)GetValue(StaticFractionBarVerticalPadProperty);

    public override Rect ScreenInkBounds => _fractionBar.ScreenInkBounds;

    public VisualFractionOperatorNode()
    {
        Children.Add(_fractionBar);
        Binding binding = new Binding("FractionBarVerticalMargin")
        {
            Source = MathcadSettings.Instance,
            Mode = BindingMode.OneWay,
            NotifyOnSourceUpdated = true
        };
        BindingOperations.SetBinding(this, StaticFractionBarVerticalPadProperty, binding);
    }

    protected override Size MeasureStaticLayout(Size availableSize)
    {
        FirstMathNode.MeasureMathNode(WpfUtils.SizeInfinity);
        SecondMathNode.MeasureMathNode(WpfUtils.SizeInfinity);
        _textBaselineY = FirstMathNode.DesiredSize.Height + FractionBarVerticalPad - Font.BaselineAdjustment;
        double height = FirstMathNode.DesiredSize.Height + SecondMathNode.DesiredSize.Height + 2.0 * FractionBarVerticalPad;
        double width = Math.Max(FirstMathNode.DesiredSize.Width, SecondMathNode.DesiredSize.Width) + 2.0 * FractionOverhang;
        _fractionBar.MeasureMathNode(new Size(width, double.PositiveInfinity));
        return new Size(width, height);
    }

    protected override Size ArrangeStaticLayout(Size finalSize)
    {
        double num = DesiredSize.Width / 2.0;
        Point firstPoint = default;
        firstPoint.X = num - FirstMathNode.DesiredSize.Width / 2.0;
        firstPoint.Y = 0.0;
        Point barPoint = default;
        barPoint.X = 0.0;
        barPoint.Y = FirstMathNode.DesiredSize.Height + FractionBarVerticalPad - _fractionBar.DesiredSize.Height / 2.0;
        Point secondPoint = default;
        secondPoint.X = num - SecondMathNode.DesiredSize.Width / 2.0;
        secondPoint.Y = FirstMathNode.DesiredSize.Height + 2.0 * FractionBarVerticalPad;
        FirstMathNode.ArrangeMathNode(new Rect(firstPoint, FirstMathNode.DesiredSize));
        _fractionBar.ArrangeMathNode(new Rect(barPoint, _fractionBar.DesiredSize));
        SecondMathNode.ArrangeMathNode(new Rect(secondPoint, SecondMathNode.DesiredSize));
        return DesiredSize;
    }

    private double DenominatorY()
    {
        VisualMathNode visualMathNode = (VisualMathNode)FirstOperand;
        double height = _fractionBar.DesiredSize.Height;
        return visualMathNode.DesiredSize.Height + height + 2.0 * FractionBarVerticalPad;
    }

    internal override HitTestResult GetNodeAtPoint(Point hitPoint)
    {
        Point val = TransformToDescendant(_fractionBar).Transform(hitPoint);
        if (val.X < 0.0)
        {
            return new VisualMathNodeHitTestResult(this, Direction.Left, Direction.Indeterminate, hitPoint);
        }
        if (val.X > ActualWidth)
        {
            return new VisualMathNodeHitTestResult(this, Direction.Right, Direction.Indeterminate, hitPoint);
        }
        if (val.Y >= 0.0 && val.Y <= _fractionBar.ActualHeight)
        {
            return new VisualMathNodeHitTestResult(this, Direction.Indeterminate, Direction.Indeterminate, hitPoint);
        }
        if (val.Y < 0.0)
        {
            GeneralTransform generalTransform = TransformToDescendant(FirstMathNode);
            return FirstMathNode.GetNodeAtPoint(generalTransform.Transform(hitPoint));
        }
        else
        {
            GeneralTransform generalTransform2 = TransformToDescendant(SecondMathNode);
            return SecondMathNode.GetNodeAtPoint(generalTransform2.Transform(hitPoint));
        }
    }
}
