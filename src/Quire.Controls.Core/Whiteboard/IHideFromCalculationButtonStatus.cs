#nullable disable
namespace Quire.Controls.Core.Whiteboard;

public partial interface IHideFromCalculationButtonStatus
{
    bool HideFromCalculationButtonEnabled { get; }
    bool HideFromCalculationButtonChecked { get; }
}
