using System.Collections.Generic;
using System.Windows;
using Quire.Controls.Core;

namespace Quire.Controls.Worksheet;

public partial interface IWorksheetSection
{
    void HandleDirectionalMove(Direction direction);

    void UpdateItemLocationsAfterGridSizeChanged(Size oldGridCellSize);

    bool HandleDelete();

    void MoveItemsBy(IEnumerable<IWhiteboardItem> items, Vector offset);
}
