using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Quire.EquationEditor.View;
using Quire.Wpf;

namespace Quire.Controls.Internal;

internal partial class VisualInputControlEditor
{
    private double BORDER_THICKNESS = 1;
    private readonly Caret _caret;
    private readonly Border _border;

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

    protected override Size MeasureOverride(Size constraint)
    {
        Size size = EditNode.DesiredSize;
        _border.Width = size.Width;
        _border.Height = size.Height;
        _border.Measure(constraint);
        _caret.Line.BeginInit();
        _caret.Line.X1 = _caret.Line.StrokeThickness / 2;
        _caret.Line.X2 = _caret.Line.StrokeThickness / 2;
        _caret.Line.Y1 = 0;
        _caret.Line.Y2 = size.Height;
        _caret.Line.EndInit();
        _caret.Line.Measure(constraint);
        size.Width += 2;
        return size;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        double offset = CaretState == CaretStateValues.RightMargin ? finalSize.Width - 1 : 0.0;
        _caret.Line.Arrange(new Rect(new Point(offset, 0.0), _caret.Line.DesiredSize));
        _border.Arrange(new Rect(new Point(1.0, 0.0), _border.DesiredSize));
        return finalSize;
    }

    public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
    {
        return WpfUtils.GroupTransformsAsFrozen(new TranslateTransform(-1, 0), base.GetDesiredTransform(transform));
    }

    protected static void HandleCaretPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((VisualInputControlEditor)d).OnCaretStateChanged();
    }

    protected virtual void OnCaretStateChanged()
    {
        switch (CaretState)
        {
            case CaretStateValues.Left:
                EditNode.NavigateInto(Direction.Left);
                break;
            case CaretStateValues.Right:
                EditNode.NavigateInto(Direction.Right);
                break;
            case CaretStateValues.LeftMargin:
            case CaretStateValues.RightMargin:
                break;
            default:
                DebugTools.FailAssertion("unexpected Caret State");
                EditNode.NavigateInto(Direction.Indeterminate);
                break;
        }

        _caret.Line.Visibility = CaretState == CaretStateValues.LeftMargin || CaretState == CaretStateValues.RightMargin ? Visibility.Visible : Visibility.Hidden;
        UpdateCaretBlinking();
    }
}
