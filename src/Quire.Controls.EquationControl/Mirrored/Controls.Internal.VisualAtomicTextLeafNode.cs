using System.Windows;

namespace Quire.Controls.Internal;

internal partial class VisualAtomicTextLeafNode
{
    protected string AtomicText { get; set; }

    public override double TextBaselineY => _baseline;

    public override double EffectiveBaselineY => 0.0;

    public override double EffectiveScriptBaselineY => 0.0;

    internal Rect SymbolBounds
    {
        get
        {
            UpdateText();
            return new Rect(GetXCoordinate(), 0.0, _formattedText.Width, _formattedText.Height);
        }
    }
}
