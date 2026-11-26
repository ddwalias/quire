#nullable disable
using System.Windows;
using System.Windows.Media;
using Net.Sgoliver.NRtfTree.Core;

namespace Quire.Controls.Worksheet;

public partial class RtfItemModel
{
    public RtfItemModel(RtfTree model, Size size, Brush background, bool hasBorder, Brush borderColor, bool noLeftOffset = false)
    {
        _model = model;
        _background = background;
        FullSize = size;
        NoLeftOffset = noLeftOffset;
        _hasBorder = hasBorder;
        _borderColor = borderColor;
    }
}
