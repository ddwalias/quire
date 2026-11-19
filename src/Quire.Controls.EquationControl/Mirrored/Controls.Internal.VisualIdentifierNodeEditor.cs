using System.Drawing;
using Quire.Controls.Core;

namespace Quire.Controls.Internal;

internal partial class VisualIdentifierNodeEditor
{
    public override void Remove()
    {
        UnhookEvents();
        base.Remove();
    }

    public void UnsetSelection()
    {
        _highlightX = 0.0;
        _highlightBox.Width = 0.0;
    }

    public void SetCaretType(bool isReadWrite)
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

    protected override void OnCaretColorChanged(Color color)
    {
        _caret.Color = color;
    }

    protected override void UpdateCaretBlinking()
    {
        _caret.Blink = base.Blink;
    }

    private void OnHighlightOpacityChanged(double opacity)
    {
        _highlightBrush.Opacity = opacity;
    }
}
