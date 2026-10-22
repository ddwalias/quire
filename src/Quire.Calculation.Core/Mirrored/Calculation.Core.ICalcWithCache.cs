#nullable disable
#pragma warning disable

namespace Quire.Calculation.Core;

partial interface ICalcWithCache
{
    global::Quire.Calculation.Core.EngineError Error { get; }
    global::Quire.Calculation.Core.IResultNode Result { get; }
    global::System.Collections.Generic.IList<global::Quire.Calculation.Core.EngineRedefInfo> EngineWarnings { get; }
}
