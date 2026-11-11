using System;
using System.Windows;
using System.Windows.Media;
using Quire.EquationEditor.View;
using Quire.Wpf;
using Color = System.Drawing.Color;
using Size = System.Windows.Size;

namespace Quire.Controls.Internal;

internal partial class VisualAtomicTextLeafEditor : VisualMathNodeEditor<VisualAtomicTextLeafNode>, IVisualAtomicTextLeafEditor, IVisualMathNodeEditor
{
    private static readonly DependencyProperty SelectedProperty;
    private System.Windows.Media.Brush _caretBrush;

    public bool Selected
    {
        get => (bool)GetValue(SelectedProperty);
        set => SetValue(SelectedProperty, value);
    }

    static VisualAtomicTextLeafEditor()
    {
        FrameworkPropertyMetadata frameworkPropertyMetadata = new FrameworkPropertyMetadata();
        frameworkPropertyMetadata.AffectsMeasure = true;
        frameworkPropertyMetadata.AffectsRender = true;
        frameworkPropertyMetadata.PropertyChangedCallback += OnSelectedChanged;
        frameworkPropertyMetadata.DefaultValue = false;
        SelectedProperty = DependencyProperty.Register("Selected", typeof(bool), typeof(VisualAtomicTextLeafEditor), frameworkPropertyMetadata);
    }

    private static void OnSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((VisualAtomicTextLeafEditor)d).OnSelectedChanged((bool)e.NewValue);
    }

    public VisualAtomicTextLeafEditor(VisualAtomicTextLeafNode editNode, VisualMathRegionNode region)
        : base(editNode, region)
    {
        BindToSettings();
    }

    private void OnSelectedChanged(bool selected)
    {
        EditNode.Visibility = selected ? Visibility.Hidden : Visibility.Visible;
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
        if ((bool)GetValue(SelectedProperty))
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
