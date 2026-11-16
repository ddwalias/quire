using System;
using System.Windows;
using Quire.Core;

namespace Quire.Controls.Worksheet;

public partial interface IHeaderFooterControl
{
    ICommonContentsContainer CommonContents { get; }

    TextFormat TextFormat { get; set; }

    Thickness Margin { get; }

    Size BodySize { get; }

    bool IsHeader { get; }

    bool IsFooter { get; }

    HeaderFooterRole Role { get; }

    event Action<bool> PageNumberControlActivated;

    event Action<Direction, bool> NavigateOutOfHeaderFooter;

    event Action<IPageNumberControl> PageNumberControlMoved;
}
