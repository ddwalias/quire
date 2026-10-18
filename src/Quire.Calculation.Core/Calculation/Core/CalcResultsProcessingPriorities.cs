using Quire.Mirror;
using System.Windows.Threading;

namespace Quire.Calculation.Core;

public static partial class CalcResultsProcessingPriorities
{
    [MirrorVerified("PtcCalculationCoreStaticConstantsCctorsDifferentialTests")]
    static CalcResultsProcessingPriorities()
    {
        DefaultResultDispatchPriority_Background = DispatcherPriority.Background;
        ShadowResultsProcessingPriority_Send = DispatcherPriority.Send;
    }
}
