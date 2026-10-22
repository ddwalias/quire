#nullable disable
#pragma warning disable

namespace Quire.Calculation.Core;

partial interface ICalcWithPriority
{
    global::System.Nullable<global::System.Windows.Threading.DispatcherPriority> ResultsProcessingPriority { get; }
}
