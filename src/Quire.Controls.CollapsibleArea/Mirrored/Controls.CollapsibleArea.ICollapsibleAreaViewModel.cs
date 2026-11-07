#nullable disable
#pragma warning disable

using System;
using System.Windows;
using Quire.Controls.Core.Protection;
using Quire.PersistentData;

namespace Quire.Controls.CollapsibleArea;

public partial interface ICollapsibleAreaViewModel
{
    bool IsCollapsed { get; }

    bool IsBorderVisible { get; set; }

    bool AreaHasErrors { get; set; }

    double AreaHeight { get; set; }

    double AreaWidth { get; set; }

    Func<ProtectionDialogState, bool> ShowProtectionDialogRequest { set; }

    Func<Func<string, bool>, bool> ShowUnProtectDialogRequest { set; }

    Action<string> ShowWarningDialogRequest { set; }

    Rect? PrintViewport { get; set; }

    new CollapsibleAreaViewModel Clone();

    void SetPersistentData(IAreaType collapsibleArea, Version formatVersion = null);

    void GetPersistentData(IAreaType collapsibleArea);

    void OnBorderVisibilityCommand();
}
