using Quire.Mcd;

namespace Quire.Engine.Internal;

/// <summary>Plot data produced by a <c>(plot2d …)</c> node: one or more x/y traces in SI magnitudes.</summary>
internal sealed class TraceVal(IReadOnlyList<Trace> traces) : DimVal
{
    public IReadOnlyList<Trace> Traces { get; } = traces;
}

internal sealed record Trace(string Label, double[] X, double[] Y, Dim XDim, Dim YDim);
