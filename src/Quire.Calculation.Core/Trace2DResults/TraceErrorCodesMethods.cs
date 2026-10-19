using System.Globalization;
using Quire.Mirror;

namespace Quire.Calculation.Core.Trace2DResults;

public static partial class TraceErrorCodes
{
    [MirrorVerified("PtcCalculationCoreTrace2DResultsTraceErrorCodesMechanicalDifferentialTests")]
    public static string CreateTooManyPointsInResultMessage(int maximalAllowedNumberOfPoints)
    {
        return string.Format(CultureInfo.CurrentUICulture, "{0}\t\t\t{1:N0}", ErrorTooManyPointsInResult, maximalAllowedNumberOfPoints);
    }
}
