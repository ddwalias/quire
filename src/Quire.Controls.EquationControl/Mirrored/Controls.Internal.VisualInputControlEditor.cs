using System.Windows;
using System.Windows.Controls;
using Quire.EquationEditor.View;
using Quire.Wpf;

namespace Quire.Controls.Internal;

internal partial class VisualInputControlEditor
{
    private double BORDER_THICKNESS = 1;

    public VisualInputControlEditor(VisualInputControlNode editNode, VisualMathRegionNode region)
        : base(editNode, region)
    {
        _caret = new Caret();
        _border = new Border
        {
            BorderThickness = new Thickness(BORDER_THICKNESS),
            BorderBrush = WpfUtils.CreateSolidWpfBrush(_caret.Color),
        };
        VisualChildren.Add(_border);
        VisualChildren.Add(_caret.Line);
        BindToSettings();
        base.Add();
    }

    public VisualInputControlEditor(VisualInputControlNode editNode, FrameworkElement adornedElement, VisualMathRegionNode region)
        : base(editNode, adornedElement, region)
    {
    }

    public static DependencyProperty CaretStateProperty = DependencyProperty.Register(
        "CaretState",
        typeof(CaretStateValues),
        typeof(VisualInputControlEditor),
        new FrameworkPropertyMetadata(
            CaretStateValues.LeftMargin,
            FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender,
            HandleCaretPositionChanged));

    public CaretStateValues CaretState
    {
        get => (CaretStateValues)GetValue(CaretStateProperty);
        set => SetValue(CaretStateProperty, value);
    }

    protected override void Add()
    {
    }

    public override void Remove()
    {
        base.Remove();
        EditNode.IsEditorFrameVisible = false;
    }

    protected override void OnCaretColorChanged(System.Drawing.Color color)
    {
        _border.BorderBrush = WpfUtils.CreateSolidWpfBrush(color);
        _caret.Color = color;
    }

    protected override void UpdateCaretBlinking()
    {
        if (DebugTools.DevAssert(_caret != null && _border != null, null).Failed)
        {
            return;
        }

        if (_caret.Line.Visibility == Visibility.Hidden)
        {
            _border.Visibility = Visibility.Visible;
            int blinkPeriod = 0;
            if (Blink)
            {
                blinkPeriod = GetCaretBlinkPeriod();
            }

            EditNode.IsEditorFrameVisible = true;
            WpfUtils.Blink(_border, blinkPeriod);
        }
        else
        {
            _border.Visibility = Visibility.Hidden;
            EditNode.IsEditorFrameVisible = false;
        }

        _caret.Blink = Blink;
    }
}
