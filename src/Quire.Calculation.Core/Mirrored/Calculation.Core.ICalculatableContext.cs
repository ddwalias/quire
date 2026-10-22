#nullable disable
#pragma warning disable

namespace Quire.Calculation.Core;

partial interface ICalculatableContext
{
    bool IsCalculating { get; set; }
    bool EvaluatingEnabled { get; set; }
    event global::System.Func<string> GetEngineVersion ;
    event global::System.Action StartEvaluating ;
    event global::System.Action StopEvaluating ;
    event global::System.Action<bool> SimplifyUnitsFlagUpdated ;
    event global::System.Action<string, object> CalculationOptionUpdated ;
    event global::System.Action<string, object> SystemVariableUpdated ;
    event global::System.Action StartAutoCalc ;
    event global::System.Action StopAutoCalc ;
    event global::System.Action<global::System.Collections.Generic.IEnumerable<global::Quire.Calculation.Core.ICalculatable>> CommitAndNotifyDependentCalculatables ;
    event global::System.Action ContextUnloaded ;
    event global::System.Action<int, global::System.Windows.Threading.DispatcherPriority> WaitForCalculationIdle ;
    event global::System.Func<string, object> GetEngineOption ;
}
