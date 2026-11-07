#nullable disable
using System;
using System.Windows;

namespace Quire.Controls.Core;

public partial interface ICollapsibleAreaControl
{
    Action GridCellSizeChangedAction { set; }
    bool IsCollapsed { get; }
    bool IsBorderVisible { get; }
    double AreaHeight { get; set; }
    bool AreaHasErrors { get; set; }
    Rect CollapsedAreaBounds { get; }
    event Action<ICollapsibleAreaControl> CollapsedStateChanged;
    event Action<ICollapsibleAreaControl> BorderStateChanged;
    void OnBorderVisibilityCommand();
}
