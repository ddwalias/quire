#nullable disable
#pragma warning disable

namespace Quire.Services;

partial interface IApplicationBehaviorOptions
{
    int CollectInputOutputStatesTimeout { get; set; }
    int DelayRecalculationInMsecAfterCloseExcel { get; set; }
}
