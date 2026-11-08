#nullable disable
#pragma warning disable

using System;
using Quire.Calculation.Core;
using Quire.Controls.Core;
using Quire.Controls.Core.BookmarkTags;
using Quire.Controls.Core.IORegions;
using Quire.Controls.Whiteboard;

namespace Quire.Controls.Mathboard;

public partial interface IMathboardModel
{
    bool IsEmpty { get; }

    IIORegionManager IoRegionManager { get; set; }

    IBookmarksManager BookmarksManager { get; set; }

    bool IsInPrinting { get; set; }

    event Action<ICalculationHideableChild> CalculatableHiddenStatusChanged;

    void CommitItem(IWhiteboardItem item);
}
