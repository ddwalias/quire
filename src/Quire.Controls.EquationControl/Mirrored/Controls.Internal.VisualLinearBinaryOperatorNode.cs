using System;
using System.Windows;
using Quire.Wpf;
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

    public override double EffectiveBaselineY => _effectiveBaseline;

    public override double EffectiveScriptBaselineY => _effectiveScriptBaseline;

    protected override Size ArrangeStaticLayout(Size finalSize)
    {
        double separation = 0.0;
        separation += ArrangeChild(FirstMathNode, separation);
        _currentSeparation = separation;
        separation += ArrangeChild(_operator, separation);
        _currentSeparation = separation - _currentSeparation;
        ArrangeChild(SecondMathNode, separation);
        return DesiredSize;
    }

    protected override Size MeasureStaticLayout(Size availableSize)
    {
        _topLineBaseline = 0.0;
        _effectiveBaseline = 0.0;
        _effectiveScriptBaseline = 0.0;
        double maxDepth = 0.0;
        double totalWidth = 0.0;
        foreach (VisualMathNode child in Children)
        {
            child.MeasureMathNode(WpfUtils.SizeInfinity);
            if (child == OperatorElement)
            {
                totalWidth += child.DesiredSize.Width;
            }
            else
            {
                totalWidth += child.DesiredSize.Width;
                _topLineBaseline = Math.Max(_topLineBaseline, child.TextBaselineY);
                _effectiveBaseline = Math.Max(_effectiveBaseline, child.EffectiveBaselineY);
                _effectiveScriptBaseline = Math.Min(_effectiveScriptBaseline, child.EffectiveScriptBaselineY);
                maxDepth = Math.Max(maxDepth, child.DepthBelowBaseline);
            }
        }

        double height = _topLineBaseline + maxDepth;
        return new Size(totalWidth, height);
    }
}
