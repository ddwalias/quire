using System;
using System.Collections.Generic;
using System.Windows;
using Quire.EquationEditor.View;
using Quire.Wpf;
using Point = System.Windows.Point;
using Size = System.Windows.Size;
using Color = System.Drawing.Color;

namespace Quire.Controls.Internal;

internal partial class VisualGradientNodeEditor : VisualMathNodeEditor<VisualGradientOperatorNode>, IVisualUnaryEditor, IVisualMathNodeEditor
{
    private static readonly DependencyProperty OperatorSelectedProperty;

    private static readonly DependencyProperty CaretOpacityProperty;

    private VisualSymbolNode OperatorSymbol { get; }

    private readonly List<RubberBandHighlighter> _operandHighlighters = new List<RubberBandHighlighter>();

    private bool OperatorSelected
    {
        get => (bool)GetValue(OperatorSelectedProperty);
        set => SetValue(OperatorSelectedProperty, value);
    }

    private static void OnOperatorSelectedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((VisualGradientNodeEditor)d).OnOperatorSelectedChanged();
    }

    private static void OnCaretOpacityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((VisualGradientNodeEditor)d).OnCaretOpacityChanged((double)e.NewValue);
    }

    static VisualGradientNodeEditor()
    {
        FrameworkPropertyMetadata frameworkPropertyMetadata = new FrameworkPropertyMetadata
        {
            AffectsRender = true,
            DefaultValue = false
        };
        frameworkPropertyMetadata.PropertyChangedCallback += OnOperatorSelectedPropertyChanged;
        OperatorSelectedProperty = DependencyProperty.Register("OperatorSelected", typeof(bool), typeof(VisualGradientNodeEditor), frameworkPropertyMetadata);
        FrameworkPropertyMetadata frameworkPropertyMetadata2 = new FrameworkPropertyMetadata
        {
            AffectsRender = true
        };
        frameworkPropertyMetadata2.PropertyChangedCallback += OnCaretOpacityPropertyChanged;
        CaretOpacityProperty = DependencyProperty.Register("CaretOpacity", typeof(double), typeof(VisualGradientNodeEditor), frameworkPropertyMetadata2);
    }

    private void OnOperatorSelectedChanged()
    {
        UpdateOperatorSymbol();
    }

    protected override void OnCaretColorChanged(Color color)
    {
        UpdateOperatorSymbol();
    }

    private void OnCaretOpacityChanged(double d)
    {
        UpdateOperatorSymbol();
    }

    internal VisualGradientNodeEditor(VisualGradientOperatorNode editNode, VisualMathRegionNode region)
        : base((VisualMathNode)editNode, region)
    {
        OperatorSymbol = editNode.GradientSymbol.Clone();
        AddVisualChild(OperatorSymbol);
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
        if (AdornedElement is VisualGradientOperatorNode)
        {
            OperatorSymbol.Measure(WpfUtils.SizeInfinity);
        }
        return desiredSize;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        if (AdornedElement is VisualGradientOperatorNode visualGradientOperatorNode)
        {
            OperatorSymbol.Arrange(new Rect(new Point(0.0, visualGradientOperatorNode.TextBaselineY - OperatorSymbol.TextBaselineY), OperatorSymbol.DesiredSize));
        }
        return finalSize;
    }

    private void UpdateOperatorSymbol()
    {
        if (OperatorSelected)
        {
            SetVisibilityOfEditorSymbols(Visibility.Visible);
            OperatorSymbol.MathFormat.FontColor = WpfUtils.GdiToWpf(GetCaretColor());
            if (_operandHighlighters.Count == 0)
            {
                VisualMathNode[] array = (AdornedElement as VisualGradientOperatorNode)?.NodeChildren;
                if (array != null)
                {
                    VisualMathNode[] array2 = array;
                    foreach (VisualMathNode visualMathNode in array2)
                    {
                        _operandHighlighters.Add(CreateNodeHighlighter(visualMathNode, HighlightCornerStyle.Rounded, visualMathNode));
                    }
                }
            }
        }
        else
        {
            SetVisibilityOfEditorSymbols(Visibility.Hidden);
            RemoveHighlighters();
        }
        if (IsCaretBlinkEnabled())
        {
            UpdateCaretBlinking();
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
        WpfUtils.Blink(OperatorSymbol, period);
    }

    private void SetVisibilityOfEditorSymbols(Visibility visibility)
    {
        EditNode.SetVisibilityOfOperatorSymbol((visibility == Visibility.Visible) ? Visibility.Hidden : Visibility.Visible);
        OperatorSymbol.Visibility = visibility;
    }
}
