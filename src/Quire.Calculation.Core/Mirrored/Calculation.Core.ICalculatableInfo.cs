#nullable disable
#pragma warning disable

namespace Quire.Calculation.Core;

partial interface ICalculatableInfo
{
    global::Quire.Calculation.Core.SynchronizationStatus SynchronizationStatus { get; }
    global::Quire.Calculation.Core.CalculatingStatus CalculatingStatus { get; }
    bool IsCommitted { get; }
    global::Quire.Calculation.Core.EngineError Error { get; }
    int ErrorCount { get; }
    string ErrorCode { get; }
    string ErrorText { get; }
    string ErrorSExpression { get; }
}
