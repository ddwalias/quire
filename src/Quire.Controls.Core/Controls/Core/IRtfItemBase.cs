#nullable disable
using System.Windows;

namespace Quire.Controls.Core;

public partial interface IRtfItemBase
{
    int PageIndex { get; }
    Size TopRectClippingSize { get; }
    Size FullSize { get; }
    Point TopLeft { get; }
    bool NoLeftOffset { get; }
}
