#nullable disable
#pragma warning disable
using System;

namespace Quire.Controls.ExcelComponent.ViewModel;

public partial interface IOleContainerItemViewModel
{
    bool IsResizing { get; }
    event Action<bool> MinSizeChanged;
}
