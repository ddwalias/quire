#nullable disable
using System;
using System.Collections.Generic;
using System.Windows;
using Quire.EquationEditor.View;
using Quire.Wpf;
using Color = System.Drawing.Color;
using Size = System.Windows.Size;

namespace Quire.Controls.Internal;

internal partial class VisualConjugateNodeEditor : VisualMathNodeEditor<VisualConjugateOperatorNode>, IVisualUnaryEditor, IVisualMathNodeEditor
{
    public static readonly DependencyProperty OperatorSelectedProperty;
    public static readonly DependencyProperty CaretOpacityProperty;
    private readonly Fence _operatorSymbol;
    private readonly List<RubberBandHighlighter> _operandHighlighters = new List<RubberBandHighlighter>();

    private bool OperatorSelected
    {
        get => (bool)GetValue(OperatorSelectedProperty);
        set => SetValue(OperatorSelectedProperty, value);
    }

    private static void OnOperatorSelectedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((VisualConjugateNodeEditor)d).OnOperatorSelectedChanged();
    }

    private static void OnCaretOpacityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((VisualConjugateNodeEditor)d).OnCaretOpacityChanged((double)e.NewValue);
    }

    static VisualConjugateNodeEditor()
    {
        FrameworkPropertyMetadata frameworkPropertyMetadata = new FrameworkPropertyMetadata();
        frameworkPropertyMetadata.AffectsRender = true;
        frameworkPropertyMetadata.DefaultValue = false;
        frameworkPropertyMetadata.PropertyChangedCallback += OnOperatorSelectedPropertyChanged;
        OperatorSelectedProperty = DependencyProperty.Register("OperatorSelected", typeof(bool), typeof(VisualConjugateNodeEditor), frameworkPropertyMetadata);

        FrameworkPropertyMetadata frameworkPropertyMetadata2 = new FrameworkPropertyMetadata();
        frameworkPropertyMetadata2.AffectsRender = true;
        frameworkPropertyMetadata2.PropertyChangedCallback += OnCaretOpacityPropertyChanged;
        CaretOpacityProperty = DependencyProperty.Register("CaretOpacity", typeof(double), typeof(VisualConjugateNodeEditor), frameworkPropertyMetadata2);
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

    public VisualConjugateNodeEditor(VisualConjugateOperatorNode editNode, VisualMathRegionNode region)
        : base(editNode, region)
    {
        _operatorSymbol = editNode.OperatorSymbol.Clone() as Fence;
        AddVisualChild(_operatorSymbol);
        BindToSettings();
        Blink = true;
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
        if (!(AdornedElement is VisualConjugateOperatorNode visualConjugateOperatorNode))
        {
            return desiredSize;
        }
        _operatorSymbol.ContentSize = visualConjugateOperatorNode.OperatorSymbol.ContentSize;
        _operatorSymbol.Measure(visualConjugateOperatorNode.OperatorSymbol.ContentSize);
        return desiredSize;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        if (!(AdornedElement is VisualConjugateOperatorNode visualConjugateOperatorNode))
        {
            return finalSize;
        }
        _operatorSymbol.Arrange(new Rect(visualConjugateOperatorNode.ConjugateSymbolTopLeft, _operatorSymbol.DesiredSize));
        return finalSize;
    }

    private void UpdateOperatorSymbol()
    {
        if (OperatorSelected)
        {
            SetVisibilityOfEditorSymbols(Visibility.Visible);
            _operatorSymbol.MathFormat.FontColor = WpfUtils.GdiToWpf(GetCaretColor());
            _operatorSymbol.Opacity = 1.0;
            if (_operandHighlighters.Count == 0)
            {
                VisualMathNode[] nodeChildren = ((VisualConjugateOperatorNode)AdornedElement).NodeChildren;
                for (int i = 0; i < nodeChildren.Length; i++)
                {
                    VisualMathNode visualMathNode = nodeChildren[i];
                    _operandHighlighters.Add(CreateNodeHighlighter(visualMathNode, HighlightCornerStyle.Rounded, visualMathNode));
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
        WpfUtils.Blink(_operatorSymbol, period);
    }

    private void SetVisibilityOfEditorSymbols(Visibility visibility)
    {
        EditNode.SetVisibilityOfOperatorSymbol((visibility == Visibility.Visible) ? Visibility.Hidden : Visibility.Visible);
        _operatorSymbol.Visibility = visibility;
    }
}
