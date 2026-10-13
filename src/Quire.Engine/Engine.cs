using Quire.Mpl.Run;
using Quire.Values;

namespace Quire.Engine;

/// <summary>
/// Default <see cref="IEngine"/>. Results are cached together with the scope after each node, so an
/// edit only recomputes the edited node and the nodes below it. Changing a global definition
/// recomputes everything. Not thread-safe: callers serialise access.
/// </summary>
public sealed class Engine : IEngine
{
    private sealed class Node(int handle, string rl)
    {
        public int Handle { get; } = handle;
        public string Rl { get; set; } = rl;
        public Sexpr? Parsed { get; set; }
        public string? ParseError { get; set; }
        public NodeResult? Result { get; set; }
        public Env? EnvBefore { get; set; }
        public Env? EnvAfter { get; set; }
        public bool IsGlobal => Parsed is SList { Head: "global" };
    }

    private readonly List<Node> _nodes = [];
    private readonly Interpreter _interp = new();
    private int _nextHandle = 1;
    private int _dirtyFrom;
    private bool _globalsDirty = true;
    private Env _globalEnv = Env.Empty;
    private readonly Dictionary<int, NodeResult> _globalResults = [];

    public event NodeCallback? NodeUpdated;
    public event Action<bool>? BusyChanged;

    public bool AutoCalc { get; set; } = true;

    public IReadOnlyList<int> Handles => _nodes.Select(n => n.Handle).ToArray();

    public bool IsUpToDate => _dirtyFrom >= _nodes.Count && !_globalsDirty;

    private int IndexOf(int handle)
    {
        int i = _nodes.FindIndex(n => n.Handle == handle);
        return i >= 0 ? i : throw new ArgumentException($"Unknown node handle {handle}", nameof(handle));
    }

    private static void Parse(Node n)
    {
        try
        {
            n.Parsed = SexprReader.Read(n.Rl);
            n.ParseError = null;
        }
        catch (FormatException ex)
        {
            n.Parsed = null;
            n.ParseError = ex.Message;
        }
    }

    private void Invalidate(int index, bool touchesGlobals)
    {
        _dirtyFrom = Math.Min(_dirtyFrom, index);
        if (touchesGlobals)
        {
            _globalsDirty = true;
            _dirtyFrom = 0;
        }
        if (AutoCalc) CalculateNow();
    }

    public int AddRegionAfter(int afterHandle, string rl)
    {
        int index = afterHandle == 0 ? 0 : IndexOf(afterHandle) + 1;
        return Insert(index, rl);
    }

    public int AddRegionBefore(int beforeHandle, string rl)
    {
        int index = beforeHandle == 0 ? _nodes.Count : IndexOf(beforeHandle);
        return Insert(index, rl);
    }

    private int Insert(int index, string rl)
    {
        var node = new Node(_nextHandle++, rl);
        Parse(node);
        _nodes.Insert(index, node);
        Invalidate(index, node.IsGlobal);
        return node.Handle;
    }

    public void UpdateRegion(int handle, string rl)
    {
        int i = IndexOf(handle);
        var node = _nodes[i];
        if (node.Rl == rl && node.Result != null) return;
        bool wasGlobal = node.IsGlobal;
        node.Rl = rl;
        Parse(node);
        Invalidate(i, wasGlobal || node.IsGlobal);
    }

    public void RemoveRegion(int handle)
    {
        int i = IndexOf(handle);
        bool global = _nodes[i].IsGlobal;
        _nodes.RemoveAt(i);
        _globalResults.Remove(handle);
        Invalidate(i, global);
    }

    public void MoveAfter(int handle, int afterHandle) => Move(handle, afterHandle, after: true);

    public void MoveBefore(int handle, int beforeHandle) => Move(handle, beforeHandle, after: false);

    private void Move(int handle, int anchor, bool after)
    {
        if (handle == anchor) return;
        int from = IndexOf(handle);
        if (anchor != 0) IndexOf(anchor); // validate before changing anything
        var node = _nodes[from];
        _nodes.RemoveAt(from);
        int to = anchor == 0 ? (after ? 0 : _nodes.Count) : IndexOf(anchor) + (after ? 1 : 0);
        _nodes.Insert(to, node);
        if (from != to) Invalidate(Math.Min(from, to), node.IsGlobal);
    }

    public void RecalculateAll()
    {
        _globalsDirty = true;
        _dirtyFrom = 0;
        if (AutoCalc) CalculateNow();
    }

    public void CalculateNow(CancellationToken cancellation = default)
    {
        if (IsUpToDate) return;
        _interp.Cancellation = cancellation;
        BusyChanged?.Invoke(true);
        try
        {
            if (_globalsDirty)
            {
                ComputeGlobals();
                _globalsDirty = false;
                _dirtyFrom = 0;
            }
            for (int i = _dirtyFrom; i < _nodes.Count; i++)
            {
                cancellation.ThrowIfCancellationRequested();
                var node = _nodes[i];
                var env = i == 0 ? _globalEnv : _nodes[i - 1].EnvAfter!;
                node.EnvBefore = env;
                if (node.ParseError != null)
                    node.Result = new NodeResult { Status = NodeStatus.Invalid, Error = new EngineError(node.ParseError) };
                else if (node.IsGlobal)
                    node.Result = _globalResults.GetValueOrDefault(node.Handle) ?? new NodeResult { Type = NodeType.GlobalDefinition };
                else
                    node.Result = _interp.Execute(node.Parsed!, ref env);
                node.EnvAfter = env;
                _dirtyFrom = i + 1;
                NodeUpdated?.Invoke(node.Handle, node.Result);
            }
        }
        finally
        {
            BusyChanged?.Invoke(false);
        }
    }

    /// <summary>Global definitions are evaluated first, in worksheet order, and are visible everywhere.</summary>
    private void ComputeGlobals()
    {
        _globalResults.Clear();
        var env = Env.Empty;
        foreach (var node in _nodes.Where(n => n.IsGlobal && n.ParseError == null))
            _globalResults[node.Handle] = _interp.Execute(node.Parsed!, ref env);
        _globalEnv = env;
    }

    public NodeResult? GetResult(int handle) => _nodes.FirstOrDefault(n => n.Handle == handle)?.Result;

    public Value? GetValueBefore(int handle, string name)
    {
        var node = _nodes.FirstOrDefault(n => n.Handle == handle);
        if (node?.EnvBefore == null) return null;
        try { return node.EnvBefore.Lookup(name); }
        catch (CalcException) { return null; }
    }
}
