#nullable disable
#pragma warning disable

namespace Quire.Calculation.Core;

partial interface ICanContainSymbolicCalculation
{
    bool ContainsSymbolicEvaluation { get; }
}
