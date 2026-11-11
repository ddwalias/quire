using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Quire.EquationEditor.View;
using Quire.Wpf;
using Color = System.Drawing.Color;
using Size = System.Windows.Size;
using Point = System.Windows.Point;

namespace Quire.Controls.Internal;

internal partial class VisualLimitNodeEditor : VisualMathNodeEditor<VisualLimitOperatorNode>, IVisualUnaryEditor, IVisualMathNodeEditor
{
    public static readonly DependencyProperty CaretOpacityProperty;

    public static readonly DependencyProperty OperatorSelectedProperty;

    private readonly List<RubberBandHighlighter> _operandHighlighters = new List<RubberBandHighlighter>();

    private readonly VisualSymbolNode _limitSymbol;

    private readonly VisualSymbolNode _limitArrow;

    private bool OperatorSelected
    {
        get => (bool)GetValue(OperatorSelectedProperty);
        set => SetValue(OperatorSelectedProperty, value);
    }

    private static void OnOperatorSelectedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((VisualLimitNodeEditor)d).OnOperatorSelectedChanged();
    }

    private static void OnCaretOpacityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((VisualLimitNodeEditor)d).OnCaretOpacityChanged((double)e.NewValue);
    }

    static VisualLimitNodeEditor()
    {
        FrameworkPropertyMetadata frameworkPropertyMetadata = new FrameworkPropertyMetadata
        {
            AffectsMeasure = true,
            AffectsRender = true
        };
        frameworkPropertyMetadata.PropertyChangedCallback += OnOperatorSelectedPropertyChanged;
        frameworkPropertyMetadata.DefaultValue = false;
        OperatorSelectedProperty = DependencyProperty.Register("OperatorSelected", typeof(bool), typeof(VisualLimitNodeEditor), frameworkPropertyMetadata);
        FrameworkPropertyMetadata frameworkPropertyMetadata2 = new FrameworkPropertyMetadata
        {
            AffectsRender = true
        };
        frameworkPropertyMetadata2.PropertyChangedCallback += OnCaretOpacityPropertyChanged;
        CaretOpacityProperty = DependencyProperty.Register("CaretOpacity", typeof(double), typeof(VisualLimitNodeEditor), frameworkPropertyMetadata2);
    }

    protected override void OnCaretColorChanged(System.Drawing.Color color)
    {
        UpdateOperatorSymbol();
    }

    private void OnOperatorSelectedChanged()
    {
        UpdateOperatorSymbol();
    }

    private void OnCaretOpacityChanged(double d)
    {
        UpdateOperatorSymbol();
    }

    public VisualLimitNodeEditor(VisualMathNode editNode, VisualMathRegionNode region)
        : base(editNode, region)
    {
        _limitSymbol = EditNode.LimitSymbol.Clone();
        _limitArrow = EditNode.LimitArrow.Clone();
        AddVisualChild(_limitSymbol);
        AddVisualChild(_limitArrow);
        BindToSettings();
    }

    public void SelectOperator(bool selected)
    {
        OperatorSelected = selected;
    }

    public override void GetFocus()
    {
        base.GetFocus();
        SetVisibilityOfEditorSymbols(Visibility.Visible);
    }

    public override void Remove()
    {
        EditNode.LimitSymbol.Visibility = Visibility.Visible;
        EditNode.LimitArrow.Visibility = Visibility.Visible;
        RemoveHighlighters();
        base.Remove();
    }

    private void UpdateOperatorSymbol()
    {
        if (OperatorSelected)
        {
            SetVisibilityOfEditorSymbols(Visibility.Visible);
            System.Windows.Media.Color color = WpfUtils.GdiToWpf(GetCaretColor());
            _limitSymbol.MathFormat.FontColor = color;
            _limitArrow.MathFormat.FontColor = color;
            if (_operandHighlighters.Count == 0)
            {
                VisualMathNode[] nodeChildren = ((VisualLimitOperatorNode)AdornedElement).NodeChildren;
                foreach (VisualMathNode visualMathNode in nodeChildren)
                {
                    _operandHighlighters.Add(CreateNodeHighlighter(visualMathNode, HighlightCornerStyle.Rounded, visualMathNode));
                }
            }
        }
        else
        {
            SetVisibilityOfEditorSymbols(Visibility.Hidden);
            RemoveHighlighters();
        }
    }

    protected override Size MeasureOverride(Size constraint)
    {
        _limitSymbol.Measure(constraint);
        _limitArrow.Measure(constraint);
        return EditNode.DesiredSize;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        if (!(AdornedElement is VisualLimitOperatorNode visualLimitOperatorNode))
        {
            return finalSize;
        }
        _limitSymbol.Arrange(new Rect(visualLimitOperatorNode.LimitSymbolTopLeft, _limitSymbol.DesiredSize));
        _limitArrow.Arrange(new Rect(visualLimitOperatorNode.LimitArrowTopLeft, _limitArrow.DesiredSize));
        return finalSize;
    }

    private new void BindToSettings()
    {
        base.BindToSettings();
        WpfUtils.BindToSetting(MathcadSettings.Instance, this, CaretOpacityProperty, "CaretHighlightOpacity");
    }

    private void RemoveHighlighters()
    {
        foreach (RubberBandHighlighter operandHighlighter in _operandHighlighters)
        {
            operandHighlighter.Remove();
        }
        _operandHighlighters.Clear();
    }

    protected override void UpdateCaretBlinking()
    {
        int period = 0;
        if (Blink)
        {
            period = GetCaretBlinkPeriod();
        }
        WpfUtils.Blink(this, period);
    }

    private void SetVisibilityOfEditorSymbols(Visibility visibility)
    {
        Visibility visibility2 = ((visibility == Visibility.Visible) ? Visibility.Hidden : Visibility.Visible);
        EditNode.LimitSymbol.Visibility = visibility2;
        EditNode.LimitArrow.Visibility = visibility2;
        _limitSymbol.Visibility = visibility;
        _limitArrow.Visibility = visibility;
    }
}
