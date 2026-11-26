#nullable disable
using System.Windows;
using Net.Sgoliver.NRtfTree.Core;

namespace Quire.Controls.Core;

public partial interface IRtfItemBase
{
    int PageIndex { get; }
    Size TopRectClippingSize { get; }
    Size FullSize { get; }
    Point TopLeft { get; }
    bool NoLeftOffset { get; }

    void AddItemToDocument(RtfTreeNode node, Point offset, bool onePageContent, RtfMerger merger);

}
