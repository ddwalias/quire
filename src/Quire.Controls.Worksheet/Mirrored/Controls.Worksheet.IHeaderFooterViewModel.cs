using System;
using System.Windows;
using Quire.Controls.Core;

namespace Quire.Controls.Worksheet;

public partial interface IHeaderFooterViewModel
{
    WhiteboardItemsCollection CommonItems { get; }

    WhiteboardItemsCollection UniqueItems { get; }

    Thickness Margin { get; set; }

    event Action<IPageNumberControl> PageNumberControlMoved;

    event Action<Direction, bool> NavigateOutOfHeaderFooter;

    event Action<bool> PageNumberControlActivated;

    void UpdateItemLocationsAfterGridSizeChanged(Size oldGridCellSize);

    bool WorksheetHandleDelete();
}
