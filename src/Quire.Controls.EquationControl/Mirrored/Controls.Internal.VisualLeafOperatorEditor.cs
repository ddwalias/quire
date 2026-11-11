using System;
using System.Windows;
using System.Windows.Media;
using Quire.EquationEditor.View;
using Quire.Wpf;
using Color = System.Drawing.Color;
using Size = System.Windows.Size;

namespace Quire.Controls.Internal;

internal partial class VisualLeafOperatorEditor : VisualMathNodeEditor<VisualLeafOperatorNode>, IVisualLeafOperatorEditor, IVisualMathNodeEditor
{
    private static readonly DependencyProperty OperatorSelectedProperty;
    private System.Windows.Media.Brush _caretBrush;

    static VisualLeafOperatorEditor()
    {
        FrameworkPropertyMetadata frameworkPropertyMetadata = new FrameworkPropertyMetadata();
        frameworkPropertyMetadata.AffectsMeasure = true;
        frameworkPropertyMetadata.AffectsRender = true;
        frameworkPropertyMetadata.PropertyChangedCallback += OnOperatorSelectedChanged;
        frameworkPropertyMetadata.DefaultValue = false;
        OperatorSelectedProperty = DependencyProperty.Register("OperatorSelected", typeof(bool), typeof(VisualLeafOperatorEditor), frameworkPropertyMetadata);
    }

    private static void OnOperatorSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((VisualLeafOperatorEditor)d).OnOperatorSelectedChanged((bool)e.NewValue);
    }

    public VisualLeafOperatorEditor(VisualLeafOperatorNode editNode, VisualMathRegionNode region)
        : base(editNode, region)
    {
        BindToSettings();
    }

    private void OnOperatorSelectedChanged(bool selected)
    {
        EditNode.Visibility = selected ? Visibility.Hidden : Visibility.Visible;
    }

    public void SelectOperator(bool selected)
    {
        SetValue(OperatorSelectedProperty, selected);
    }

    public override void Remove()
    {
        base.Remove();
        EditNode.Visibility = Visibility.Visible;
    }

    protected override Size MeasureOverride(Size constraint)
    {
        return EditNode.DesiredSize;
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        if ((bool)GetValue(OperatorSelectedProperty))
        {
            drawingContext.DrawGeometry(_caretBrush, null, EditNode.Geometry.Clone());
        }
        base.OnRender(drawingContext);
    }

    protected override void OnCaretColorChanged(Color color)
    {
        _caretBrush = WpfUtils.CreateSolidWpfBrush(color);
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
