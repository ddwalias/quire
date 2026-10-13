using Quire.Values;

namespace Quire.Engine;

public delegate void NodeCallback(int handle, NodeResult result);

/// <summary>
/// The calculation engine keeps an ordered list of nodes (one per region, in worksheet order).
/// A node sees every definition made by the nodes before it, plus all global definitions.
/// </summary>
public interface IEngine
{
    /// <summary>Raised for each node whose result was (re)computed.</summary>
    event NodeCallback? NodeUpdated;

    /// <summary>Raised when a calculation pass starts (true) and finishes (false).</summary>
    event Action<bool>? BusyChanged;

    bool AutoCalc { get; set; }

    IReadOnlyList<int> Handles { get; }

    /// <summary>Inserts a node after <paramref name="afterHandle"/> (0 inserts at the start) and returns its handle.</summary>
    int AddRegionAfter(int afterHandle, string rl);

    int AddRegionBefore(int beforeHandle, string rl);

    void UpdateRegion(int handle, string rl);

    void RemoveRegion(int handle);

    void MoveAfter(int handle, int afterHandle);

    void MoveBefore(int handle, int beforeHandle);

    /// <summary>Computes every node that is out of date.</summary>
    void CalculateNow(CancellationToken cancellation = default);

    /// <summary>Marks every node out of date and recomputes when auto-calculation is on.</summary>
    void RecalculateAll();

    NodeResult? GetResult(int handle);

    /// <summary>Value of <paramref name="name"/> as seen just before <paramref name="handle"/>.</summary>
    Value? GetValueBefore(int handle, string name);

    bool IsUpToDate { get; }
}

public static class EngineFactory
{
    public static IEngine CreateEngine() => new Engine();
}
