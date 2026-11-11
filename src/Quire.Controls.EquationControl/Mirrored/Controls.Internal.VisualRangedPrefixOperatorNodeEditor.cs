using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Quire.EquationEditor.View;
using Quire.Wpf;
using Point = System.Windows.Point;
using Size = System.Windows.Size;
using Color = System.Drawing.Color;

namespace Quire.Controls.Internal;

internal partial class VisualRangedPrefixOperatorNodeEditor : VisualMathNodeEditor<VisualRangedPrefixNode>, IVisualUnaryEditor, IVisualMathNodeEditor
{
    public static readonly DependencyProperty CaretOpacityProperty;

    public static readonly DependencyProperty OperatorSelectedProperty;

    private readonly VisualSymbolNode _rangedPrefixSymbol;

    private readonly List<RubberBandHighlighter> _operandHighlighters = new List<RubberBandHighlighter>();

    private bool OperatorSelected
    {
        get => (bool)GetValue(OperatorSelectedProperty);
        set => SetValue(OperatorSelectedProperty, value);
    }

    private static void OnOperatorSelectedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((VisualRangedPrefixOperatorNodeEditor)d).OnOperatorSelectedChanged();
    }

    private static void OnCaretOpacityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((VisualRangedPrefixOperatorNodeEditor)d).OnCaretOpacityChanged((double)e.NewValue);
    }

    static VisualRangedPrefixOperatorNodeEditor()
    {
        FrameworkPropertyMetadata frameworkPropertyMetadata = new FrameworkPropertyMetadata();
        frameworkPropertyMetadata.AffectsMeasure = true;
        frameworkPropertyMetadata.AffectsRender = true;
        frameworkPropertyMetadata.PropertyChangedCallback += OnOperatorSelectedPropertyChanged;
        frameworkPropertyMetadata.DefaultValue = false;
        OperatorSelectedProperty = DependencyProperty.Register("OperatorSelected", typeof(bool), typeof(VisualRangedPrefixOperatorNodeEditor), frameworkPropertyMetadata);
        FrameworkPropertyMetadata frameworkPropertyMetadata2 = new FrameworkPropertyMetadata();
        frameworkPropertyMetadata2.AffectsRender = true;
        frameworkPropertyMetadata2.PropertyChangedCallback += OnCaretOpacityPropertyChanged;
        CaretOpacityProperty = DependencyProperty.Register("CaretOpacity", typeof(double), typeof(VisualRangedPrefixOperatorNodeEditor), frameworkPropertyMetadata2);
    }

    protected override void OnCaretColorChanged(Color color)
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

    public VisualRangedPrefixOperatorNodeEditor(VisualRangedPrefixNode editNode, VisualMathRegionNode region)
        : base((VisualMathNode)editNode, region)
    {
        _rangedPrefixSymbol = editNode.OperatorSymbol.Clone();
        AddVisualChild(_rangedPrefixSymbol);
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
        EditNode.SetVisibilityOfOperatorSymbol(Visibility.Visible);
        RemoveHighlighters();
        base.Remove();
    }

    protected override Size MeasureOverride(Size constraint)
    {
        Size desiredSize = AdornedElement.DesiredSize;
        if (!(AdornedElement is VisualRangedPrefixNode))
        {
            return desiredSize;
        }
        _rangedPrefixSymbol.Measure(WpfUtils.SizeInfinity);
        return desiredSize;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        if (!(AdornedElement is VisualRangedPrefixNode visualRangedPrefixNode))
        {
            return finalSize;
        }
        _rangedPrefixSymbol.Arrange(new Rect(visualRangedPrefixNode.OperatorSymbolTopLeft, _rangedPrefixSymbol.DesiredSize));
        return finalSize;
    }

    private void UpdateOperatorSymbol()
    {
        if (OperatorSelected)
        {
            SetVisibilityOfEditorSymbols(Visibility.Visible);
            System.Windows.Media.Color value = WpfUtils.GdiToWpf(GetCaretColor());
            _rangedPrefixSymbol.MathFormat.FontColor = value;
            if (_operandHighlighters.Count == 0)
            {
                VisualMathNode[] nodeChildren = ((VisualRangedPrefixNode)AdornedElement).NodeChildren;
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
        WpfUtils.Blink(_rangedPrefixSymbol, period);
    }

    private void SetVisibilityOfEditorSymbols(Visibility visibility)
    {
        EditNode.SetVisibilityOfOperatorSymbol((visibility == Visibility.Visible) ? Visibility.Hidden : Visibility.Visible);
        _rangedPrefixSymbol.Visibility = visibility;
    }
}
