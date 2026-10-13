using Quire.Engine.Internal;
using Quire.Mcd;

namespace Quire.Engine.Tests;

public abstract class EngineTestBase
{
    internal readonly WorksheetEngine Engine = new();

    /// <summary>Appends regions (Rl strings) and returns their results in order.</summary>
    internal IReadOnlyList<NodeResult> Run(params string[] regions)
    {
        int last = Engine.Handles.LastOrDefault();
        var handles = new List<int>();
        foreach (var rl in regions)
        {
            last = Engine.AddAfter(last, rl, 0, 0);
            handles.Add(last);
        }
        Engine.CalculateNow(0);
        return handles.Select(h => Engine.GetResult(h)!).ToList();
    }

    internal NodeResult Last(params string[] regions) => Run(regions)[^1];

    internal static NumVal Num(NodeResult r)
    {
        Assert.Null(r.Error);
        return Assert.IsType<NumVal>(r.Value);
    }

    internal static void AssertNum(NodeResult r, double expected, Dim? dim = null, double relTol = 1e-9)
    {
        var n = Num(r);
        Assert.True(Math.Abs(n.Real - expected) <= relTol * Math.Max(1, Math.Abs(expected)), $"expected {expected}, got {n.Real}");
        Assert.Same(dim ?? Dim.identity, n.m_dim);
    }

    internal static void AssertError(NodeResult r, string fragment)
    {
        Assert.NotNull(r.Error);
        Assert.Contains(fragment, r.Error!.Message, StringComparison.OrdinalIgnoreCase);
    }
}
