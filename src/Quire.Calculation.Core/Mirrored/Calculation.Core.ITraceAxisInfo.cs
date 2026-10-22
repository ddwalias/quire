#nullable disable
#pragma warning disable

namespace Quire.Calculation.Core;

partial interface ITraceAxisInfo
{
    double Max { get; }
    double Min { get; }
    global::System.Collections.Generic.IList<global::Quire.Calculation.Core.Unit> Units { get; }
}
