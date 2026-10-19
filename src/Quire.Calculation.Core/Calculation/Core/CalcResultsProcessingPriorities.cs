using Quire.Mirror;
using System.Windows.Threading;

namespace Quire.Calculation.Core;

public static partial class CalcResultsProcessingPriorities
{
    [MirrorVerified("PtcCalculationCoreStaticConstantsCctorsMechanicalDifferentialTests")]
    static CalcResultsProcessingPriorities()
    {
        DefaultResultDispatchPriority_Background = DispatcherPriority.Background;
        ShadowResultsProcessingPriority_Send = DispatcherPriority.Send;
    }
}
