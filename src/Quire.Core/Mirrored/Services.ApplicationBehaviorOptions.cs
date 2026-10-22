#nullable disable
#pragma warning disable

namespace Quire.Services;

partial class ApplicationBehaviorOptions
{
    public int DelayRecalculationInMsecAfterCloseExcel
    {
        get => _delayRecalculationInMsecAfterCloseExcel;
        set => _delayRecalculationInMsecAfterCloseExcel = value < 0 ? 0 : value;
    }
}
