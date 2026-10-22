#nullable disable
#pragma warning disable

namespace Quire.Calculation.Core;

partial interface IDataExchangeScriptingModel
{
    global::Quire.Calculation.Core.IResultNode EngineResultForSerialization { get; }
    bool IsBlockRunningScript { get; set; }
    void PushResult(global::Quire.Calculation.Core.IResultNode resultNode, bool isRunEvents = false);
    void SetValue(string id, object scriptingInputs, bool isAutoCalc);
    void SetState(global::Quire.Calculation.Core.CalculatableState state, bool isAutoCalc = false);
    object GetValue(string id, bool isRunScriptingOnRecalculate, bool isAutoCalc);
}
