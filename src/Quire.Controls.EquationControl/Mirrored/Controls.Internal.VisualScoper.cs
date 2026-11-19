using System.Windows;
using Quire.Controls.Core;
using Quire.Wpf;

namespace Quire.Controls.Internal;

internal partial class VisualScoper
{
    protected override void OnLostFocus(object sender, RoutedEventArgs eventArgs)
    {
        if (LostFocus != null)
        {
            LostFocus.Invoke();
        }
    }

    protected override void OnLostFocus(RoutedEventArgs e)
    {
        if (LostFocus != null)
        {
            LostFocus.Invoke();
        }
    }

    public override void Remove()
    {
        if (_highlighter != null)
        {
            ((VisualAdornerBase)_highlighter).Remove();
        }

        base.Remove();
    }

    protected override void UpdateCaretBlinking()
    {
        _caret.Blink = base.Blink;
    }

    public void SetCaret(bool isReadWrite)
    {
        if (isReadWrite)
        {
            _caret.Color = MathcadSettings.Instance.CaretColor;
        }
        else
        {
            _caret.Color = MathcadSettings.Instance.ReadOnlyCaretColor;
        }
    }

    private void BindToTypeover()
    {
        WpfUtils.BindToSetting(MathcadSettings.Instance, _highlighter, RubberBandHighlighter.ColorProperty, "TypeoverColor");
        WpfUtils.BindToSetting(MathcadSettings.Instance, _highlighter, UIElement.OpacityProperty, "CaretHighlightOpacity");
        _highlighter.CornerStyle = (HighlightCornerStyle)2;
    }
}
