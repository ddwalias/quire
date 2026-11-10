using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Quire.EquationEditor.View;
using Quire.Wpf;

namespace Quire.Controls.Internal;

internal partial class VisualInputScriptableControlNodeEditor
{
    private double BORDER_THICKNESS = 1.0;
    private readonly Caret _caret;
    private readonly Border _border;

    public static DependencyProperty CaretStateProperty = DependencyProperty.Register(
        "CaretState",
        typeof(CaretStateValues),
        typeof(VisualInputScriptableControlNodeEditor),
        new FrameworkPropertyMetadata(
            CaretStateValues.LeftMargin,
            FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender,
            HandleCaretPositionChanged));

    public CaretStateValues CaretState
    {
        get => (CaretStateValues)GetValue(CaretStateProperty);
        set => SetValue(CaretStateProperty, value);
    }

    public VisualInputScriptableControlNodeEditor(VisualMathNode editNode, VisualMathRegionNode region)
        : base(editNode, region)
    {
        _caret = new Caret();
        _border = new Border
        {
            BorderThickness = new Thickness(BORDER_THICKNESS),
            BorderBrush = WpfUtils.CreateSolidWpfBrush(_caret.Color)
        };
        VisualChildren.Add(_border);
        BindToSettings();
        base.Add();
    }

    protected override void OnContextMenuOpening(ContextMenuEventArgs e)
    {
        _ = ContextMenu;
    }

    protected override void OnCaretColorChanged(System.Drawing.Color color)
    {
        _border.BorderBrush = WpfUtils.CreateSolidWpfBrush(color);
        _caret.Color = color;
    }

    protected override void UpdateCaretBlinking()
    {
        if (_caret == null && _border == null)
        {
            return;
        }

        if (_caret.Line.Visibility == Visibility.Hidden && CaretState != CaretStateValues.Intermediate)
        {
            _border.Visibility = Visibility.Visible;
            int period = 0;
            if (Blink)
            {
                period = GetCaretBlinkPeriod();
            }

            WpfUtils.Blink(_border, period);
        }
        else
        {
            _border.Visibility = Visibility.Hidden;
        }

        _caret.Blink = Blink;
    }

    protected override Size MeasureOverride(Size constraint)
    {
        Size desiredSize = EditNode.DesiredSize;
        _border.Width = desiredSize.Width;
        _border.Height = desiredSize.Height;
        _border.Measure(constraint);
        _caret.Line.BeginInit();
        _caret.Line.X1 = _caret.Line.StrokeThickness / 2.0;
        _caret.Line.X2 = _caret.Line.StrokeThickness / 2.0;
        _caret.Line.Y1 = 0.0;
        _caret.Line.Y2 = desiredSize.Height;
        _caret.Line.EndInit();
        _caret.Line.Measure(constraint);
        desiredSize.Width += 2.0;
        return desiredSize;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        double num = CaretState == CaretStateValues.RightMargin ? finalSize.Width - 1.0 : 0.0;
        _caret.Line.Arrange(new Rect(new Point(num, 0.0), _caret.Line.DesiredSize));
        _border.Arrange(new Rect(new Point(1.0, 0.0), _border.DesiredSize));
        return finalSize;
    }

    protected override void OnPreviewKeyDown(KeyEventArgs e)
    {
        if ((e.KeyboardDevice.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
        {
            return;
        }

        switch (e.Key)
        {
            case Key.Up:
                break;
            case Key.Down:
                break;
        }

        base.OnPreviewKeyDown(e);
    }

    public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
    {
        return WpfUtils.GroupTransformsAsFrozen(new TranslateTransform(-1.0, 0.0), base.GetDesiredTransform(transform));
    }

    protected static void HandleCaretPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((VisualInputScriptableControlNodeEditor)d).OnCaretStateChanged();
    }

    protected virtual void OnCaretStateChanged()
    {
        switch (CaretState)
        {
            case CaretStateValues.Right:
                EditNode.NavigateInto(Direction.Right);
                break;
            default:
                DebugTools.FailAssertion("unexpected Caret State");
                break;
            case CaretStateValues.Left:
            case CaretStateValues.LeftMargin:
            case CaretStateValues.Intermediate:
            case CaretStateValues.RightMargin:
                break;
        }

        _caret.Line.Visibility = CaretState == CaretStateValues.LeftMargin || CaretState == CaretStateValues.RightMargin
            ? Visibility.Visible
            : Visibility.Hidden;
        UpdateCaretBlinking();
    }
}
