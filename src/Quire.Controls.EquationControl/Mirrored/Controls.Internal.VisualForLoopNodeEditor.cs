using System;
using System.Collections.Generic;
using System.Windows;
using Quire.Core;
using Quire.EquationEditor.View;
using Quire.Wpf;
using Point = System.Windows.Point;
using Size = System.Windows.Size;
using Color = System.Drawing.Color;

namespace Quire.Controls.Internal;

internal partial class VisualForLoopNodeEditor
{
    public static readonly DependencyProperty NodeSideProperty;
    public static readonly DependencyProperty CaretOpacityProperty;

    private VisualSymbolNode _forOperator;
    private VisualSymbolNode _inOperator;
    private readonly List<RubberBandHighlighter> _operandHighlighters = new List<RubberBandHighlighter>();

    public Direction NodeSide
    {
        get => (Direction)GetValue(NodeSideProperty);
        set => SetValue(NodeSideProperty, value);
    }

    private double CaretOpacity => (double)GetValue(CaretOpacityProperty);

    private static void OnCaretOpacityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((VisualForLoopNodeEditor)d).OnCaretOpacityChanged((double)e.NewValue);
    }

    private static void OnNodeSidePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((VisualForLoopNodeEditor)d).OnNodeSideChanged((Direction)e.NewValue);
    }

    static VisualForLoopNodeEditor()
    {
        FrameworkPropertyMetadata frameworkPropertyMetadata = new FrameworkPropertyMetadata();
        frameworkPropertyMetadata.DefaultValue = Direction.Indeterminate;
        frameworkPropertyMetadata.AffectsArrange = true;
        frameworkPropertyMetadata.PropertyChangedCallback += OnNodeSidePropertyChanged;
        NodeSideProperty = DependencyProperty.Register("NodeSide", typeof(Direction), typeof(VisualForLoopNodeEditor), frameworkPropertyMetadata);

        FrameworkPropertyMetadata frameworkPropertyMetadata2 = new FrameworkPropertyMetadata();
        frameworkPropertyMetadata2.AffectsRender = true;
        frameworkPropertyMetadata2.PropertyChangedCallback += OnCaretOpacityPropertyChanged;
        CaretOpacityProperty = DependencyProperty.Register("CaretOpacity", typeof(double), typeof(VisualForLoopNodeEditor), frameworkPropertyMetadata2);
    }

    private void OnNodeSideChanged(Direction direction)
    {
        UpdateOperatorSymbols();
    }

    protected override void OnCaretColorChanged(Color color)
    {
        UpdateOperatorSymbols();
    }

    private void OnCaretOpacityChanged(double d)
    {
        UpdateOperatorSymbols();
    }

    public VisualForLoopNodeEditor(VisualForLoopNode editNode, VisualMathRegionNode region)
        : base((VisualMathNode)editNode, region)
    {
        _forOperator = editNode.ForOperator.Clone();
        AddVisualChild(_forOperator);
        _inOperator = editNode.InOperator.Clone();
        AddVisualChild(_inOperator);
        BindToSettings();
        Blink = true;
    }

    public override void GetFocus()
    {
        base.GetFocus();
        SetVisibilityOfEditorSymbols(Visibility.Visible);
    }

    public override void Remove()
    {
        EditNode.SetVisibilityOfOperatorSymbol(Visibility.Visible);
        RemoveHighlighters();
        base.Remove();
    }

    protected override Size MeasureOverride(Size constraint)
    {
        _forOperator.Measure(WpfUtils.SizeInfinity);
        _inOperator.Measure(WpfUtils.SizeInfinity);
        return AdornedElement.DesiredSize;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        if (!(AdornedElement is VisualForLoopNode visualForLoopNode))
        {
            return finalSize;
        }
        _forOperator.Arrange(new Rect(visualForLoopNode.ForOperatorTopLeft, _forOperator.DesiredSize));
        _inOperator.Arrange(new Rect(visualForLoopNode.InOperatorTopLeft, _inOperator.DesiredSize));
        return finalSize;
    }

    private new void BindToSettings()
    {
        base.BindToSettings();
        WpfUtils.BindToSetting(MathcadSettings.Instance, this, CaretOpacityProperty, "CaretHighlightOpacity");
    }

    private void UpdateOperatorSymbols()
    {
        if (NodeSide == Direction.Indeterminate)
        {
            RemoveHighlighters();
        }
        else if (_operandHighlighters.Count == 0)
        {
            VisualMathNode[] nodeChildren = ((VisualForLoopNode)AdornedElement).NodeChildren;
            foreach (VisualMathNode visualMathNode in nodeChildren)
            {
                _operandHighlighters.Add(CreateNodeHighlighter(visualMathNode, HighlightCornerStyle.Rounded, visualMathNode));
            }
        }
        SetVisibilityOfEditorSymbols(Visibility.Visible);
        if (NodeSide != Direction.Indeterminate)
        {
            System.Windows.Media.Color value = WpfUtils.GdiToWpf(GetCaretColor());
            _forOperator.MathFormat.FontColor = value;
            _inOperator.MathFormat.FontColor = value;
            if (NodeSide == Direction.Left)
            {
                _forOperator.Opacity = 1.0;
                _inOperator.Opacity = CaretOpacity;
            }
            else if (NodeSide == Direction.Right)
            {
                _forOperator.Opacity = CaretOpacity;
                _inOperator.Opacity = 1.0;
            }
        }
        else
        {
            SetVisibilityOfEditorSymbols(Visibility.Hidden);
        }
        if (IsCaretBlinkEnabled())
        {
            UpdateCaretBlinking();
        }
    }

    private void RemoveHighlighters()
    {
        foreach (RubberBandHighlighter operandHighlighter in _operandHighlighters)
        {
            operandHighlighter.Remove();
        }
        _operandHighlighters.Clear();
    }

    private VisualSymbolNode GetVisualSymbolNode()
    {
        VisualSymbolNode result = null;
        if (NodeSide == Direction.Left)
        {
            result = _forOperator;
        }
        else if (NodeSide == Direction.Right)
        {
            result = _inOperator;
        }
        return result;
    }

    protected override void UpdateCaretBlinking()
    {
        VisualSymbolNode visualSymbolNode = GetVisualSymbolNode();
        if (visualSymbolNode != null)
        {
            int period = 0;
            if (Blink)
            {
                period = GetCaretBlinkPeriod();
            }
            WpfUtils.Blink(visualSymbolNode, period);
        }
    }

    private void SetVisibilityOfEditorSymbols(Visibility visibility)
    {
        EditNode.SetVisibilityOfOperatorSymbol((visibility == Visibility.Visible) ? Visibility.Hidden : Visibility.Visible);
        _forOperator.Visibility = visibility;
        _inOperator.Visibility = visibility;
    }
}
