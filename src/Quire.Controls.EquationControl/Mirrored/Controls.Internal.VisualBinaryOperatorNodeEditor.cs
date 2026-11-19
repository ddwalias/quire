using System.Drawing;
using Quire.Wpf;

namespace Quire.Controls.Internal;

internal partial class VisualBinaryOperatorNodeEditor
{
    public void RemoveNodeHighlights()
    {
        foreach (RubberBandHighlighter highlighter in _highlighters)
        {
            ((Quire.Controls.Core.VisualAdornerBase)highlighter).Remove();
        }
    }

    public override void Remove()
    {
        RemoveNodeHighlights();
        base.Remove();
        ShowOperatorHighlight(false);
    }

    protected override void OnCaretColorChanged(Color color)
    {
        _caretBrush = WpfUtils.CreateSolidWpfBrush(color);
    }

    protected override void UpdateCaretBlinking()
    {
        int blinkPeriod = 0;
        if (Blink)
        {
            blinkPeriod = GetCaretBlinkPeriod();
        }

        WpfUtils.Blink(this, blinkPeriod);
    }
}
