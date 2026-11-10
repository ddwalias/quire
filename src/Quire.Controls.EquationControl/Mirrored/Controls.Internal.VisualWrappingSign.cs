using Quire.Controls.Core;

namespace Quire.Controls.Internal;

internal partial class VisualWrappingSign
{
    public double YOffset
    {
        get
        {
            double scale = !MathFormat.FontSize.HasValue ? 1.0 : MathFormat.FontSize.Value / FontConstants.DefaultMathFontSizeInPoints;
            _yoffset = scale * EquationProperties.Settings.Default.LineWrappingSignOffset;
            DebugTools.DevAssert(DesiredSize.Height - Geometry.Bounds.Bottom >= _yoffset, null);
            return _yoffset;
        }
    }
}
