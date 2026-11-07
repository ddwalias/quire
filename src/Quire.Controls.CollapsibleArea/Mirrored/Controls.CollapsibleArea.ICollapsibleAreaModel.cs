#nullable disable
#pragma warning disable

using System;
using System.Windows;
using Quire.PersistentData;

namespace Quire.Controls.CollapsibleArea;

public partial interface ICollapsibleAreaModel
{
    bool CanToggleCollapseState { get; }

    bool IsCollapsed { get; }

    bool IsBorderVisible { get; set; }

    Rect? PrintViewport { get; set; }

    bool ToggleCollapseState();

    void GetPersistentData(IAreaType data);

    void SetPersistentData(IAreaType data, Version formatVersion);

    bool ToggleBorderVisibleState();
}
