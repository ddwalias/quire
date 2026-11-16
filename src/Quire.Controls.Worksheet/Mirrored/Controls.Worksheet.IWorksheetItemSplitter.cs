using System;
using System.Collections.Generic;
using System.Windows;
using Quire.Controls.Core;

namespace Quire.Controls.Worksheet;

public partial interface IWorksheetItemSplitter : IDisposable
{
    IList<IWhiteboardItem> GetItemParts(IWhiteboardItem item);

    void Split(IWhiteboardItem item, int pageIndex, out IWhiteboardItemZombie activeZombie, IWorksheetPage prevItemContainingPage = null);

    bool IsItemSplit(IWhiteboardItem item);

    void RemoveZombies(IWhiteboardItem item);

    void ReplaceZombieWithRealItem(IWhiteboardItemZombie zombie);

    double GetCollectiveSplitOffsetDiff(IWhiteboardItem item, Rect itemBounds, double itemTopOnPage);

    IWhiteboardItemZombie GetItemZombieOnPage(IWhiteboardItem item, IWorksheetPage page);

    void HideZombies(IWhiteboardItem item, bool clearItemClip);

    void ShowZombies(IWhiteboardItem item);

    IWhiteboardItem GetMatchingPart(IWhiteboardItem item, Point pt);

    bool TranslateToRepresentMatchingPartLocation(IWhiteboardItem item, Point ptIn, out Point ptOut);
}
