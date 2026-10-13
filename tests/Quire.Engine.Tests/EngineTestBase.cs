using Quire.Engine;
using Quire.Values;

namespace Quire.Engine.Tests;

public abstract class EngineTestBase
{
    protected readonly IEngine Engine = EngineFactory.CreateEngine();

    /// <summary>Appends regions (Rl strings) and returns their results in order.</summary>
    protected IReadOnlyList<NodeResult> Run(params string[] regions)
    {
        int last = Engine.Handles.LastOrDefault();
        var handles = new List<int>();
        foreach (var rl in regions)
        {
            last = Engine.AddRegionAfter(last, rl);
            handles.Add(last);
        }
        Engine.CalculateNow();
        return handles.Select(h => Engine.GetResult(h)!).ToList();
    }

    protected NodeResult Last(params string[] regions) => Run(regions)[^1];

    protected static NumVal Num(NodeResult r)
    {
        Assert.Null(r.Error);
        return Assert.IsType<NumVal>(r.Value);
    }

    protected static void AssertNum(NodeResult r, double expected, Dim? dim = null, double relTol = 1e-9)
    {
        var n = Num(r);
        Assert.True(Math.Abs(n.V - expected) <= relTol * Math.Max(1, Math.Abs(expected)), $"expected {expected}, got {n.V}");
        Assert.Equal(dim ?? Dim.None, n.D);
    }

    protected static void AssertError(NodeResult r, string fragment)
    {
        Assert.NotNull(r.Error);
        Assert.Contains(fragment, r.Error!.Message, StringComparison.OrdinalIgnoreCase);
    }
}
