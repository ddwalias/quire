using System.Globalization;
using System.Reflection;

namespace Quire.Differential.Tests;

/// <summary>Direct comparisons for the generated TraceErrorCodes message formatter.</summary>
public class PtcCalculationCoreTrace2DResultsTraceErrorCodesMechanicalDifferentialTests
{
    private const string Contract = "Ptc.Calculation.Core.Trace2DResults.TraceErrorCodes";
    private const int Token = 0x0600027f;

    [SkippableFact]
    public void CreateTooManyPointsInResultMessage_0600027f()
    {
        MethodInfo qMethod = typeof(global::Quire.Calculation.Core.Trace2DResults.TraceErrorCodes)
            .GetMethod(nameof(global::Quire.Calculation.Core.Trace2DResults.TraceErrorCodes.CreateTooManyPointsInResultMessage), BindingFlags.Static | BindingFlags.Public)!;
        MethodInfo mMethod = Mathcad.Type("Ptc.Calculation.Core", Contract).Module.ResolveMethod(Token) as MethodInfo
            ?? throw new InvalidOperationException("Mathcad formatter method was not found");
        CultureInfo originalUiCulture = CultureInfo.CurrentUICulture;
        try
        {
            foreach (string cultureName in new[] { "en-US", "de-DE", "fr-FR" })
            {
                CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo(cultureName);
                foreach (int limit in new[] { -1, 0, 1, 999, 1000, 1000000 })
                {
                    MethodComparison.Compare($"{cultureName}: {limit}", Contract, qMethod, null, [limit],
                        mMethod, null, [limit]);
                }
            }
        }
        finally
        {
            CultureInfo.CurrentUICulture = originalUiCulture;
        }
    }
}
