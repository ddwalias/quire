using System.Windows;
using Quire.Controls.Core;
using Quire.Controls.EquationProperties;

namespace Quire.Controls.Internal;

internal partial class VisualWrappingSign
{
    private double _yoffset;

    public VisualWrappingSign()
    {
        VisualMathNode.SetOperatorFontProperties(MathFormat);
        MathFormat.FontColor = FontConstants.DefaultNonEditableMathFontColor.Color;
    }

    public override string Text => Settings.Default.LineWrappingSign;

    public double YOffset
    {
        get
        {
            double scale = !MathFormat.FontSize.HasValue ? 1.0 : MathFormat.FontSize.Value / FontConstants.DefaultMathFontSizeInPoints;
            _yoffset = scale * Settings.Default.LineWrappingSignOffset;
            DebugTools.DevAssert(DesiredSize.Height - Geometry.Bounds.Bottom >= _yoffset, null);
            return _yoffset;
        }
    }
}
