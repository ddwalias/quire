using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Quire.EquationEditor.Tree;

namespace Quire.Controls.Internal;

internal partial class VisualStringNode
{
    public override double EffectiveBaselineY => _baseline;

    public override double EffectiveScriptBaselineY => _exponentBaseline;

    public override FormattedText FormattedNodeText
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

    public override INodeInfo NodeInfo
    {
        get => base.NodeInfo;
        set
        {
            base.NodeInfo = value;
            ValueNodeInfo valueInfo = value as ValueNodeInfo;
            if (valueInfo == null)
            {
                ((VisualTextNodeForEditor)this).Text = string.Empty;
                return;
            }

            ((VisualTextNodeForEditor)this).Text = valueInfo.Value.ToString();
        }
    }

    public Geometry Geometry => ((VisualTextNodeForEditor)this).FormattedNodeText.BuildGeometry(new Point(0.0, 0.0));

    public override IList<double> ScreenCharacterWidths
    {
        get
        {
            IList<double> widths = ((VisualTextNodeForEditor)this).CharacterWidths;
            Point origin = PointToScreen(new Point(0.0, 0.0));
            for (int i = 0; i < widths.Count; i++)
            {
                Point charPoint = new Point(widths[i], 0.0);
                widths[i] = PointToScreen(charPoint).X - origin.X;
            }

            return widths;
        }
    }

    public VisualStringNode()
    {
    }
}
