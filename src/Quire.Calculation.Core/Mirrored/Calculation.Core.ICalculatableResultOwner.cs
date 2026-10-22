#nullable disable
#pragma warning disable

namespace Quire.Calculation.Core;

partial interface ICalculatableResultOwner
{
    global::Quire.Calculation.Core.EngineResult EngineResult { get; set; }
    bool InterceptAndUpdateResult { get; }
    bool UseICalculatableToRestoreResult { get; }
    void RestoreResult();
}
