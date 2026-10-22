#nullable disable
#pragma warning disable

namespace Quire.Calculation.Core;

partial interface ICalculationErrorOwner
{
    global::Quire.Calculation.Core.EngineError CurrentError { get; }
    bool ContainsError(global::Quire.Calculation.Core.EngineError error);
    void ShowError(global::Quire.Calculation.Core.EngineError error);
}
