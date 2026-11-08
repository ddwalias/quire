#nullable disable
#pragma warning disable

using System;
using System.ComponentModel;
using Quire.Controls.Core;
using Quire.Controls.Core.Whiteboard;
using Quire.Controls.Whiteboard;
using Quire.Core;

namespace Quire.Controls.Mathboard;

public partial interface IMathboardViewModel
{
    IMathboardModel Model { get; }

    bool HideFromCalculationButtonEnabled { get; }

    bool HideFromCalculationButtonChecked { get; }

    bool IsInPrinting { get; set; }

    void Commit();

    void RecalculateCmdHandler();

    void Commit(IWhiteboardItem item);

    void ToggleInputRegions();

    void ToggleOutputRegions();

    bool CanToggleRegionsAsInput();

    bool CanToggleRegionsAsOutput();

    void ToggleHideFromCalculationContext();
}
