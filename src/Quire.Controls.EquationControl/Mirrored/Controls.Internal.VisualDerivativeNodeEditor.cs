using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Media;
using Quire.EquationEditor.View;
using Quire.Wpf;
using Size = System.Windows.Size;

namespace Quire.Controls.Internal;

internal partial class VisualDerivativeNodeEditor : VisualMathNodeEditor<VisualDerivativeOperatorNodeBase>, IVisualUnaryEditor, IVisualMathNodeEditor
{
    public static readonly DependencyProperty CaretOpacityProperty;

    public static readonly DependencyProperty OperatorSelectedProperty;

    private System.Windows.Media.Brush _caretBrush;

    private Geometry _operatorGeometry;

    private readonly List<RubberBandHighlighter> _operandHighlighters = new List<RubberBandHighlighter>();

    private bool OperatorSelected
    {
        get => (bool)GetValue(OperatorSelectedProperty);
        set => SetValue(OperatorSelectedProperty, value);
    }

    private static void OnOperatorSelectedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((VisualDerivativeNodeEditor)d).OnOperatorSelectedChanged();
    }

    private static void OnCaretOpacityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((VisualDerivativeNodeEditor)d).OnCaretOpacityChanged((double)e.NewValue);
    }

    static VisualDerivativeNodeEditor()
    {
        FrameworkPropertyMetadata frameworkPropertyMetadata = new FrameworkPropertyMetadata();
        frameworkPropertyMetadata.AffectsMeasure = true;
        frameworkPropertyMetadata.AffectsRender = true;
        frameworkPropertyMetadata.PropertyChangedCallback += OnOperatorSelectedPropertyChanged;
        frameworkPropertyMetadata.DefaultValue = false;
        OperatorSelectedProperty = DependencyProperty.Register("OperatorSelected", typeof(bool), typeof(VisualDerivativeNodeEditor), frameworkPropertyMetadata);
        FrameworkPropertyMetadata frameworkPropertyMetadata2 = new FrameworkPropertyMetadata();
        frameworkPropertyMetadata2.AffectsRender = true;
        frameworkPropertyMetadata2.PropertyChangedCallback += OnCaretOpacityPropertyChanged;
        CaretOpacityProperty = DependencyProperty.Register("CaretOpacity", typeof(double), typeof(VisualDerivativeNodeEditor), frameworkPropertyMetadata2);
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

    public VisualDerivativeNodeEditor(VisualDerivativeOperatorNodeBase editNode, VisualMathRegionNode region)
        : base(editNode, region)
    {
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

    private void UpdateOperatorSymbol()
    {
        if (OperatorSelected)
        {
            SetVisibilityOfEditorSymbols(Visibility.Visible);
            System.Drawing.Color caretColor = GetCaretColor();
            _caretBrush = WpfUtils.CreateSolidWpfBrush(caretColor);
            if (_operandHighlighters.Count == 0)
            {
                VisualMathNode[] nodeChildren = ((VisualDerivativeOperatorNodeBase)AdornedElement).NodeChildren;
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
        if (EditNode.OperatorElement is IGeometryProvider geometryProvider)
        {
            _operatorGeometry = geometryProvider.Geometry.Clone();
        }
        return EditNode.DesiredSize;
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        if ((bool)GetValue(OperatorSelectedProperty))
        {
            drawingContext.DrawGeometry(_caretBrush, null, _operatorGeometry);
        }
        base.OnRender(drawingContext);
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
        EditNode.SetVisibilityOfOperatorSymbol((visibility == Visibility.Visible) ? Visibility.Hidden : Visibility.Visible);
    }
}
