#nullable disable
using System;
using System.Collections.Generic;
using System.Windows;
using Quire.EquationEditor.View;
using Quire.Wpf;
using Color = System.Drawing.Color;
using Size = System.Windows.Size;

namespace Quire.Controls.Internal;

internal partial class VisualTryNodeEditor : VisualMathNodeEditor<VisualTryNode>, IVisualOutfixNodeEditor, IVisualMathNodeEditor
{
    public static readonly DependencyProperty NodeSideProperty;
    public static readonly DependencyProperty CaretOpacityProperty;
    private VisualSymbolNode _tryOperator;
    private VisualSymbolNode _onErrorOperator;
    private readonly List<RubberBandHighlighter> _operandHighlighters = new List<RubberBandHighlighter>();

    public Direction NodeSide
    {
        get => (Direction)GetValue(NodeSideProperty);
        set => SetValue(NodeSideProperty, value);
    }

    private double CaretOpacity => (double)GetValue(CaretOpacityProperty);

    private static void OnCaretOpacityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((VisualTryNodeEditor)d).OnCaretOpacityChanged((double)e.NewValue);
    }

    private static void OnNodeSidePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((VisualTryNodeEditor)d).OnNodeSideChanged((Direction)e.NewValue);
    }

    static VisualTryNodeEditor()
    {
        FrameworkPropertyMetadata frameworkPropertyMetadata = new FrameworkPropertyMetadata();
        frameworkPropertyMetadata.DefaultValue = Direction.Indeterminate;
        frameworkPropertyMetadata.AffectsArrange = true;
        frameworkPropertyMetadata.PropertyChangedCallback += OnNodeSidePropertyChanged;
        NodeSideProperty = DependencyProperty.Register("NodeSide", typeof(Direction), typeof(VisualTryNodeEditor), frameworkPropertyMetadata);

        FrameworkPropertyMetadata frameworkPropertyMetadata2 = new FrameworkPropertyMetadata();
        frameworkPropertyMetadata2.AffectsRender = true;
        frameworkPropertyMetadata2.PropertyChangedCallback += OnCaretOpacityPropertyChanged;
        CaretOpacityProperty = DependencyProperty.Register("CaretOpacity", typeof(double), typeof(VisualTryNodeEditor), frameworkPropertyMetadata2);
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

    public VisualTryNodeEditor(VisualTryNode editNode, VisualMathRegionNode region)
        : base(editNode, region)
    {
        _tryOperator = editNode.TryOperator.Clone();
        AddVisualChild(_tryOperator);
        _onErrorOperator = editNode.OnErrorOperator.Clone();
        AddVisualChild(_onErrorOperator);
        BindToSettings();
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
        _tryOperator.Measure(WpfUtils.SizeInfinity);
        _onErrorOperator.Measure(WpfUtils.SizeInfinity);
        return AdornedElement.DesiredSize;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        if (!(AdornedElement is VisualTryNode visualTryNode))
        {
            return finalSize;
        }
        _tryOperator.Arrange(new Rect(visualTryNode.TryOperatorTopLeft, _tryOperator.DesiredSize));
        _onErrorOperator.Arrange(new Rect(visualTryNode.OnErrorOperatorTopLeft, _onErrorOperator.DesiredSize));
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
            VisualMathNode[] nodeChildren = ((VisualTryNode)AdornedElement).NodeChildren;
            for (int i = 0; i < nodeChildren.Length; i++)
            {
                VisualMathNode visualMathNode = nodeChildren[i];
                _operandHighlighters.Add(CreateNodeHighlighter(visualMathNode, HighlightCornerStyle.Rounded, visualMathNode));
            }
        }
        SetVisibilityOfEditorSymbols(Visibility.Visible);
        if (NodeSide != Direction.Indeterminate)
        {
            System.Windows.Media.Color color = WpfUtils.GdiToWpf(GetCaretColor());
            _tryOperator.MathFormat.FontColor = color;
            _onErrorOperator.MathFormat.FontColor = color;
            if (NodeSide == Direction.Left)
            {
                _tryOperator.Opacity = 1.0;
                _onErrorOperator.Opacity = CaretOpacity;
            }
            else if (NodeSide == Direction.Right)
            {
                _tryOperator.Opacity = CaretOpacity;
                _onErrorOperator.Opacity = 1.0;
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

    private VisualSymbolNode GetSymbolNode()
    {
        VisualSymbolNode result = null;
        if (NodeSide == Direction.Left)
        {
            result = _tryOperator;
        }
        else if (NodeSide == Direction.Right)
        {
            result = _onErrorOperator;
        }
        return result;
    }

    protected override void UpdateCaretBlinking()
    {
        VisualSymbolNode symbolNode = GetSymbolNode();
        if (symbolNode != null)
        {
            int period = 0;
            if (Blink)
            {
                period = GetCaretBlinkPeriod();
            }
            WpfUtils.Blink(symbolNode, period);
        }
    }

    private void SetVisibilityOfEditorSymbols(Visibility visibility)
    {
        EditNode.SetVisibilityOfOperatorSymbol((visibility == Visibility.Visible) ? Visibility.Hidden : Visibility.Visible);
        _tryOperator.Visibility = visibility;
        _onErrorOperator.Visibility = visibility;
    }
}
