#nullable disable
#pragma warning disable

namespace Quire.Calculation.Core;

partial interface IEngineError
{
    string ErrorCode { get; }
    global::Quire.Calculation.Core.StringCollection ErrorMessageArguments { get; }
    uint NodeHandle { get; set; }
    uint Argument { get; set; }
    global::Quire.Calculation.Core.ICalculatable ErrorSource { get; }
    string ResourceString { get; set; }
    bool IsRuntime { get; set; }
    global::Quire.Calculation.Core.EngineError OriginalError { get; }
    bool IsEquivalentTo(global::Quire.Calculation.Core.EngineError error, bool checkNodeHandle = true);
    global::Quire.Calculation.Core.EngineError FindNextError(global::Quire.Calculation.Core.EngineError error);
    bool IsAncestorOf(global::Quire.Calculation.Core.EngineError error);
}
