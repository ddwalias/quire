#nullable disable
using System;
using System.Windows;
using System.Windows.Media;
using Quire.EquationEditor.View;
using Quire.Wpf;
using Color = System.Drawing.Color;
using Size = System.Windows.Size;

namespace Quire.Controls.Internal;

internal partial class VisualLinearUnaryEditor : VisualMathNodeEditor<VisualUnaryOperatorNodeForEditor>, IVisualUnaryEditor, IVisualMathNodeEditor
{
    private static readonly DependencyProperty OperatorSelectedProperty;
    private Brush _caretBrush;
    private Geometry _operatorGeometry;
    private readonly RubberBandHighlighter _operandHighlighter;

    static VisualLinearUnaryEditor()
    {
        FrameworkPropertyMetadata frameworkPropertyMetadata = new FrameworkPropertyMetadata();
        frameworkPropertyMetadata.AffectsMeasure = true;
        frameworkPropertyMetadata.AffectsRender = true;
        frameworkPropertyMetadata.PropertyChangedCallback += OnOperatorSelectedChanged;
        frameworkPropertyMetadata.DefaultValue = false;
        OperatorSelectedProperty = DependencyProperty.Register("OperatorSelected", typeof(bool), typeof(VisualLinearUnaryEditor), frameworkPropertyMetadata);
    }

    private static void OnOperatorSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((VisualLinearUnaryEditor)d).OnOperatorSelectedChanged((bool)e.NewValue);
    }

    public VisualLinearUnaryEditor(VisualUnaryOperatorNodeForEditor editNode, VisualMathRegionNode region)
        : base(editNode, editNode.OperatorElement, region)
    {
        BindToSettings();
        _operandHighlighter = CreateNodeHighlighter(region.RootNode as UIElement, HighlightCornerStyle.Rounded, editNode.Child);
        _operandHighlighter.Visibility = Visibility.Hidden;
    }

    private void OnOperatorSelectedChanged(bool selected)
    {
        EditNode.OperatorElement.Visibility = (selected ? Visibility.Hidden : Visibility.Visible);
        _operandHighlighter.Visibility = (selected ? Visibility.Visible : Visibility.Hidden);
    }

    public void SelectOperator(bool selected)
    {
        SetValue(OperatorSelectedProperty, selected);
    }

    public override void Remove()
    {
        if (_operandHighlighter != null)
        {
            _operandHighlighter.Remove();
        }
        base.Remove();
        EditNode.OperatorElement.Visibility = Visibility.Visible;
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

    protected override void OnCaretColorChanged(Color color)
    {
        _caretBrush = WpfUtils.CreateSolidWpfBrush(color);
    }

    public new void SetCaretType(bool isReadWrite)
    {
        if (!isReadWrite)
        {
            WpfUtils.BindToSetting(MathcadSettings.Instance, this, CaretColorProperty, "ReadOnlyCaretColor");
        }
        base.SetCaretType(isReadWrite);
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
}
