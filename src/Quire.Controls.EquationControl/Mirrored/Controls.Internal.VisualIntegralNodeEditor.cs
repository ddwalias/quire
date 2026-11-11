#nullable disable
using System;
using System.Collections.Generic;
using System.Windows;
using Quire.EquationEditor.View;
using Quire.Wpf;
using Color = System.Drawing.Color;
using Size = System.Windows.Size;

namespace Quire.Controls.Internal;

internal partial class VisualIntegralNodeEditor : VisualMathNodeEditor<VisualIntegralOperatorNode>, IVisualOutfixNodeEditor, IVisualMathNodeEditor
{
    public static readonly DependencyProperty NodeSideProperty;
    public static readonly DependencyProperty CaretOpacityProperty;
    private readonly Fence _integralSymbol;
    private readonly VisualSymbolNode _variableOfIntegrationPrefix;
    private readonly List<RubberBandHighlighter> _operandHighlighters = new List<RubberBandHighlighter>();

    public Direction NodeSide
    {
        get => (Direction)GetValue(NodeSideProperty);
        set => SetValue(NodeSideProperty, value);
    }

    private double CaretOpacity => (double)GetValue(CaretOpacityProperty);

    private static void OnCaretOpacityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((VisualIntegralNodeEditor)d).OnCaretOpacityChanged((double)e.NewValue);
    }

    private static void OnNodeSidePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((VisualIntegralNodeEditor)d).OnNodeSideChanged((Direction)e.NewValue);
    }

    static VisualIntegralNodeEditor()
    {
        FrameworkPropertyMetadata frameworkPropertyMetadata = new FrameworkPropertyMetadata();
        frameworkPropertyMetadata.DefaultValue = Direction.Indeterminate;
        frameworkPropertyMetadata.AffectsArrange = true;
        frameworkPropertyMetadata.PropertyChangedCallback += OnNodeSidePropertyChanged;
        NodeSideProperty = DependencyProperty.Register("NodeSide", typeof(Direction), typeof(VisualIntegralNodeEditor), frameworkPropertyMetadata);

        FrameworkPropertyMetadata frameworkPropertyMetadata2 = new FrameworkPropertyMetadata();
        frameworkPropertyMetadata2.AffectsRender = true;
        frameworkPropertyMetadata2.PropertyChangedCallback += OnCaretOpacityPropertyChanged;
        CaretOpacityProperty = DependencyProperty.Register("CaretOpacity", typeof(double), typeof(VisualIntegralNodeEditor), frameworkPropertyMetadata2);
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

    public VisualIntegralNodeEditor(VisualIntegralOperatorNode editNode, VisualMathRegionNode region)
        : base(editNode, region)
    {
        _integralSymbol = editNode.IntegralSymbol.Clone() as Fence;
        AddVisualChild(_integralSymbol);
        _variableOfIntegrationPrefix = editNode.VariableOfIntegrationPrefix.Clone();
        AddVisualChild(_variableOfIntegrationPrefix);
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
        Size desiredSize = AdornedElement.DesiredSize;
        if (!(AdornedElement is VisualIntegralOperatorNode visualIntegralOperatorNode))
        {
            return desiredSize;
        }
        _integralSymbol.ContentSize = visualIntegralOperatorNode.IntegralSymbol.ContentSize;
        _integralSymbol.Measure(WpfUtils.SizeInfinity);
        _variableOfIntegrationPrefix.Measure(WpfUtils.SizeInfinity);
        return desiredSize;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        if (!(AdornedElement is VisualIntegralOperatorNode visualIntegralOperatorNode))
        {
            return finalSize;
        }
        _integralSymbol.Arrange(new Rect(visualIntegralOperatorNode.IntegralSymbolTopLeft, _integralSymbol.DesiredSize));
        _variableOfIntegrationPrefix.Arrange(new Rect(visualIntegralOperatorNode.VariablePrefixTopLeft, _variableOfIntegrationPrefix.DesiredSize));
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
            VisualMathNode[] nodeChildren = ((VisualIntegralOperatorNode)AdornedElement).NodeChildren;
            for (int i = 0; i < nodeChildren.Length; i++)
            {
                VisualMathNode visualMathNode = nodeChildren[i];
                _operandHighlighters.Add(CreateNodeHighlighter(visualMathNode, HighlightCornerStyle.Rounded, visualMathNode));
            }
        }
        SetVisibilityOfEditorSymbols(Visibility.Visible);
        System.Windows.Media.Color color = WpfUtils.GdiToWpf(GetCaretColor());
        if (NodeSide == Direction.Left)
        {
            _integralSymbol.MathFormat.FontColor = color;
            _integralSymbol.Opacity = 1.0;
            _variableOfIntegrationPrefix.MathFormat.FontColor = color;
            _variableOfIntegrationPrefix.Opacity = CaretOpacity;
        }
        else if (NodeSide == Direction.Right)
        {
            _integralSymbol.MathFormat.FontColor = color;
            _integralSymbol.Opacity = CaretOpacity;
            _variableOfIntegrationPrefix.MathFormat.FontColor = color;
            _variableOfIntegrationPrefix.Opacity = 1.0;
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

    private VisualMathNode GetMathNode()
    {
        VisualMathNode result = null;
        if (NodeSide == Direction.Left)
        {
            result = _integralSymbol;
        }
        else if (NodeSide == Direction.Right)
        {
            result = _variableOfIntegrationPrefix;
        }
        return result;
    }

    protected override void UpdateCaretBlinking()
    {
        VisualMathNode mathNode = GetMathNode();
        if (mathNode != null)
        {
            int period = 0;
            if (Blink)
            {
                period = GetCaretBlinkPeriod();
            }
            WpfUtils.Blink(mathNode, period);
        }
    }

    private void SetVisibilityOfEditorSymbols(Visibility visibility)
    {
        EditNode.SetVisibilityOfOperatorSymbol((visibility == Visibility.Visible) ? Visibility.Hidden : Visibility.Visible);
        _integralSymbol.Visibility = visibility;
        _variableOfIntegrationPrefix.Visibility = visibility;
    }
}
