using System.Collections.Generic;

namespace Quire.Controls.Internal;

internal partial class VisualStringNode
{
    public override double EffectiveBaselineY => _baseline;

    public override double EffectiveScriptBaselineY => _exponentBaseline;

    public override System.Windows.Media.FormattedText FormattedNodeText
    {
        get
        {
            if (_formattedText == null)
            {
                UpdateText();
            }

            return _formattedText;
        }
    }

    public override IList<double> CharacterWidths
    {
        get
        {
            UpdateText();
            return _characterWidths;
        }
    }

    public VisualStringNode()
    {
    }
}
