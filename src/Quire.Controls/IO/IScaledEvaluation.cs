#nullable disable
namespace Quire.Controls.IO;

internal partial interface IScaledEvaluation
{
    ScaledEvaluation.ScaledEvaluationResult GetComputedValue(out double value, out string units);
    ScaledEvaluation.ScaledEvaluationResult GetComputedValue(out double[][] value, out string units);
    ScaledEvaluation.ScaledEvaluationResult GetComputedValue(out string value);
}
