#nullable disable
#pragma warning disable
using Quire.Calculation.Core;

namespace Quire.Controls.ExcelComponent.Model;

public partial interface IOleBaseComponentCalculatableChild
{
    Pair<bool, bool> IsFragment { get; }
    bool IsConnectedToTheEngine { get; set; }
    EngineError Error { get; }
}
