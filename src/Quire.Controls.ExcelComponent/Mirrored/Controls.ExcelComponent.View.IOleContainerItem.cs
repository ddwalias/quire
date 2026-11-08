#nullable disable
#pragma warning disable
using Quire.Calculation;
using Quire.Calculation.Core;
using Quire.Controls.ExcelComponent.ViewModel;

namespace Quire.Controls.ExcelComponent.View;

public partial interface IOleContainerItem
{
    ICalculationComponent Component { get; }
    IOleContainerItemViewModel ViewModel { get; }
    bool IsResizing { get; set; }
    void OnEngineError(EngineError error);
}
