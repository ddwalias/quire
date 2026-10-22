#nullable disable
#pragma warning disable

namespace Quire.Calculation.Core.Support;

partial interface ICalcWithSyncStatus
{
    global::Quire.Calculation.Core.SynchronizationStatus SynchronizationStatus { get; }
    event global::System.Action<global::Quire.Calculation.Core.Support.ICalcWithSyncStatus> Synchronized ;
}
