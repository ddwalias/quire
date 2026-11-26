#nullable disable
using System.Windows;
using System.Windows.Media;
using Net.Sgoliver.NRtfTree.Core;
using Quire.Controls.Core;

namespace Quire.Controls.Worksheet;

public partial class RtfItemModel
{
    public string Rtf => _model.Rtf;

    public RtfItemModel(RtfTree model, Size size, Brush background, bool hasBorder, Brush borderColor, bool noLeftOffset = false)
    {
        _model = model;
        _background = background;
        FullSize = size;
        NoLeftOffset = noLeftOffset;
        _hasBorder = hasBorder;
        _borderColor = borderColor;
    }

    protected override void AddItemToDocumentInternal(RtfTreeNode node, Rect clippingRect, bool relativeToCaret, RtfMerger merger)
    {
        node.AddRichTextbox(_model, _background, clippingRect, GetLocationFlags(relativeToCaret), merger, _hasBorder, _borderColor);
    }
}
