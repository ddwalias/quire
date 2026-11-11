using System;
using System.Drawing;
using System.Windows;
using Quire.EquationEditor.View;
using Quire.Wpf;
using Size = System.Windows.Size;
using Point = System.Windows.Point;

namespace Quire.Controls.Internal;

internal partial class VisualProgramNodeEditor : VisualMathNodeEditor<VisualProgramNode>, IVisualUnaryEditor, IVisualMathNodeEditor
{
    public static readonly DependencyProperty OperatorSelectedProperty;

    public static readonly DependencyProperty CaretOpacityProperty;

    private readonly Fence _operatorSymbol;

    private RubberBandHighlighter _operandHighlighter;

    private bool OperatorSelected
    {
        get => (bool)GetValue(OperatorSelectedProperty);
        set => SetValue(OperatorSelectedProperty, value);
    }

    private static void OnOperatorSelectedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((VisualProgramNodeEditor)d).OnOperatorSelectedChanged();
    }

    private static void OnCaretOpacityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((VisualProgramNodeEditor)d).OnCaretOpacityChanged((double)e.NewValue);
    }

    static VisualProgramNodeEditor()
    {
        FrameworkPropertyMetadata frameworkPropertyMetadata = new FrameworkPropertyMetadata();
        frameworkPropertyMetadata.AffectsRender = true;
        frameworkPropertyMetadata.DefaultValue = false;
        frameworkPropertyMetadata.PropertyChangedCallback += OnOperatorSelectedPropertyChanged;
        OperatorSelectedProperty = DependencyProperty.Register("OperatorSelected", typeof(bool), typeof(VisualProgramNodeEditor), frameworkPropertyMetadata);
        FrameworkPropertyMetadata frameworkPropertyMetadata2 = new FrameworkPropertyMetadata();
        frameworkPropertyMetadata2.AffectsRender = true;
        frameworkPropertyMetadata2.PropertyChangedCallback += OnCaretOpacityPropertyChanged;
        CaretOpacityProperty = DependencyProperty.Register("CaretOpacity", typeof(double), typeof(VisualProgramNodeEditor), frameworkPropertyMetadata2);
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

    public VisualProgramNodeEditor(VisualProgramNode editNode, VisualMathRegionNode region)
        : base(editNode, region)
    {
        _operatorSymbol = ((Fence)editNode.Operator).Clone() as Fence;
        AddVisualChild(_operatorSymbol);
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
        RemoveHighlighter();
        base.Remove();
    }

    protected override Size MeasureOverride(Size constraint)
    {
        Size desiredSize = AdornedElement.DesiredSize;
        if (!(AdornedElement is VisualProgramNode visualProgramNode))
        {
            return desiredSize;
        }
        Size contentSize = ((Fence)visualProgramNode.Operator).ContentSize;
        _operatorSymbol.ContentSize = contentSize;
        _operatorSymbol.Measure(WpfUtils.SizeInfinity);
        return desiredSize;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        if (!(AdornedElement is VisualProgramNode))
        {
            return finalSize;
        }
        _operatorSymbol.Arrange(new Rect(new Point(0.0, 0.0), _operatorSymbol.DesiredSize));
        return finalSize;
    }

    private void UpdateOperatorSymbol()
    {
        if (OperatorSelected)
        {
            SetVisibilityOfEditorSymbols(Visibility.Visible);
            _operatorSymbol.MathFormat.FontColor = WpfUtils.GdiToWpf(GetCaretColor());
            _operatorSymbol.Opacity = 1.0;
            _operandHighlighter = CreateNodeHighlighter(AdornedElement, HighlightCornerStyle.Rounded, ((VisualProgramNode)AdornedElement).NodeChildren);
        }
        else
        {
            SetVisibilityOfEditorSymbols(Visibility.Hidden);
            RemoveHighlighter();
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

    private void RemoveHighlighter()
    {
        if (_operandHighlighter != null)
        {
            _operandHighlighter.Remove();
        }
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
