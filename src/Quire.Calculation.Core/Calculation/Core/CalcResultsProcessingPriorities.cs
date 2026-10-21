#nullable disable
using System.Windows.Threading;
using Quire.Mirror;

namespace Quire.Calculation.Core;

public static partial class CalcResultsProcessingPriorities
{
    [MirrorVerified("PtcCalculationCoreCalcResultsProcessingPrioritiesMechanicalDifferentialTests")]
    public static readonly DispatcherPriority DefaultResultDispatchPriority_Background = DispatcherPriority.Background;

    [MirrorVerified("PtcCalculationCoreCalcResultsProcessingPrioritiesMechanicalDifferentialTests")]
    public static readonly DispatcherPriority ShadowResultsProcessingPriority_Send = DispatcherPriority.Send;
}
