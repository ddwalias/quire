using System.Collections;
using Quire.Mcd;
using Quire.Mpl.Internal;
using Quire.Mpl.Run;

namespace Quire.Engine.Internal;

/// <summary>Creates <see cref="WorksheetEngine"/>s.</summary>
internal sealed class EngineFactory : IFactory
{
    private readonly List<WeakReference<WorksheetEngine>> _engines = [];

    public int WorkerStackSize { get; set; }

    public void Initialize(string appData) { }

    public bool VerifyIntegrity() => true;

    public IEngine NewEngine()
    {
        var e = new WorksheetEngine();
        lock (_engines) _engines.Add(new WeakReference<WorksheetEngine>(e));
        return e;
    }

    public ICollection GetEngines()
    {
        lock (_engines)
        {
            _engines.RemoveAll(w => !w.TryGetTarget(out _));
            return _engines.Select(w => w.TryGetTarget(out var e) ? e : null).OfType<WorksheetEngine>().ToArray();
        }
    }

    public void Dispose()
    {
        lock (_engines) _engines.Clear();
    }
}

/// <summary>
/// The <see cref="IEngine"/> implementation. Results are cached together with the scope after each node, so an
/// edit only recomputes the edited node and the nodes below it. Changing a global definition recomputes
/// everything. Calculation runs synchronously on the calling thread; callers serialise access.
/// </summary>
internal sealed class WorksheetEngine : IEngine
{
    private sealed class Node(int handle, string rl, int priority, int cookie)
    {
        public int Handle { get; } = handle;
        public string Rl { get; set; } = rl;
        public int Priority { get; set; } = priority;
        public int Cookie { get; set; } = cookie;
        public Sexpr? Parsed { get; set; }
        public string? ParseError { get; set; }
        public NodeResult? Result { get; set; }
        public Env? EnvBefore { get; set; }
        public Env? EnvAfter { get; set; }
        public bool IsGlobal => Internal.Rl.Head(Parsed) == "global";
    }

    private readonly List<Node> _nodes = [];
    private readonly Interpreter _interp = new();
    private readonly List<NodeCallback> _callbacks = [];
    private readonly Dictionary<int, NodeResult> _globalResults = [];
    private int _nextHandle = 1;
    private int _dirtyFrom;
    private bool _globalsDirty = true;
    private bool _autoCalc = true;
    private Env _globalEnv = Env.Empty;

    public event BooleanCallback? OnUpToDate;
    public event BooleanCallback? OnBusy;

    public bool IsBusy { get; private set; }

    /// <summary>True when every node's result is current.</summary>
    public bool IsUpToDate => _dirtyFrom >= _nodes.Count && !_globalsDirty;

    public IReadOnlyList<int> Handles => _nodes.Select(n => n.Handle).ToArray();

    /// <summary>The engine holds no unmanaged resources, so there is nothing to release.</summary>
    public void Dispose() { }

    public string GetVersion() => typeof(WorksheetEngine).Assembly.GetName().Version?.ToString() ?? "0";

    public void AddNodeCallback(NodeCallback A_0) => _callbacks.Add(A_0);

    public void SetAutoCalc(bool b)
    {
        _autoCalc = b;
        if (b) AutoCalculate();
    }

    public bool GetAutoCalc() => _autoCalc;

    private int IndexOf(int handle)
    {
        int i = _nodes.FindIndex(n => n.Handle == handle);
        return i >= 0 ? i : throw new ArgumentException($"Unknown node handle {handle}", nameof(handle));
    }

    private Node NodeOf(int handle) => _nodes[IndexOf(handle)];

    private static void Parse(Node n)
    {
        try
        {
            n.Parsed = RlReader.Read(n.Rl);
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
        AutoCalculate();
    }

    private void AutoCalculate()
    {
        if (!_autoCalc) return;
        // The edit has already been made; a cancelled pass leaves the rest dirty for the next CalculateNow.
        try { CalculateNow(0); }
        catch (OperationCanceledException) { }
    }

    // ---------------------------------------------------------------- node list

    public int AddAfter(int h, string expr, int priority, int client_cookie) =>
        Insert(h == 0 ? 0 : IndexOf(h) + 1, expr, priority, client_cookie);

    public int AddBefore(int h, string expr, int priority, int client_cookie) =>
        Insert(h == 0 ? _nodes.Count : IndexOf(h), expr, priority, client_cookie);

    private int Insert(int index, string rl, int priority, int cookie)
    {
        var node = new Node(_nextHandle++, rl, priority, cookie);
        Parse(node);
        _nodes.Insert(index, node);
        Invalidate(index, node.IsGlobal);
        return node.Handle;
    }

    public void Update(int h, string expr, int priority, int client_cookie) => Change(h, expr, priority, client_cookie, force: false);

    public void Replace(int h, string expr, int priority, int client_cookie) => Change(h, expr, priority, client_cookie, force: true);

    private void Change(int h, string expr, int priority, int cookie, bool force)
    {
        int i = IndexOf(h);
        var node = _nodes[i];
        node.Priority = priority;
        node.Cookie = cookie;
        if (!force && node.Rl == expr && node.Result != null) return;
        bool wasGlobal = node.IsGlobal;
        node.Rl = expr;
        Parse(node);
        Invalidate(i, wasGlobal || node.IsGlobal);
    }

    public void Remove(int h, int client_cookie)
    {
        int i = IndexOf(h);
        bool global = _nodes[i].IsGlobal;
        _nodes.RemoveAt(i);
        _globalResults.Remove(h);
        Invalidate(i, global);
    }

    public void MoveAfter(int h1, int h2, int client_cookie) => Move(h1, h2, after: true);

    public void MoveBefore(int h1, int h2, int client_cookie) => Move(h1, h2, after: false);

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

    public void SetPriority(int h, int priority) => NodeOf(h).Priority = priority;

    public int GetPriority(int h) => NodeOf(h).Priority;

    public IList GetTopNodes() => _nodes.Select(n => n.Handle).ToList();

    public IList GetDefined(int h) => (NodeOf(h).Result?.DefinedNames ?? []).ToList();

    /// <summary>Marks every node out of date and recomputes when auto-calculation is on.</summary>
    public void RecalculateAll()
    {
        _globalsDirty = true;
        _dirtyFrom = 0;
        AutoCalculate();
    }

    // ---------------------------------------------------------------- calculation

    public void CalculateNow(int h)
    {
        if (IsUpToDate) return;
        RuntimeThread.Ensure();
        int last = h == 0 ? _nodes.Count - 1 : IndexOf(h);
        IsBusy = true;
        OnBusy?.Invoke(true);
        try
        {
            if (_globalsDirty)
            {
                ComputeGlobals();
                _globalsDirty = false;
                _dirtyFrom = 0;
            }
            for (int i = _dirtyFrom; i <= last; i++)
            {
                if (Rts.IsCancellationRequested()) throw new OperationCanceledException();
                var node = _nodes[i];
                var env = i == 0 ? _globalEnv : _nodes[i - 1].EnvAfter!;
                node.EnvBefore = env;
                if (node.ParseError != null)
                    node.Result = new NodeResult { Status = NodeStatus.NODE_INVALID, Error = new EngineError(node.ParseError) };
                else if (node.IsGlobal)
                    node.Result = _globalResults.GetValueOrDefault(node.Handle) ?? new NodeResult { Kind = NodeKind.GlobalDefinition };
                else
                    node.Result = _interp.Execute(node.Parsed!, ref env);
                node.EnvAfter = env;
                _dirtyFrom = i + 1;
                foreach (var cb in _callbacks.ToArray()) cb(node.Handle, node.Result.Status, node.Cookie, 0, 0);
            }
        }
        finally
        {
            IsBusy = false;
            OnBusy?.Invoke(false);
        }
        if (IsUpToDate) OnUpToDate?.Invoke(true);
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

    // ---------------------------------------------------------------- results

    public NodeResult? GetResult(int handle) => _nodes.FirstOrDefault(n => n.Handle == handle)?.Result;

    /// <summary>A node at or below the first out-of-date node reports <see cref="NodeStatus.NODE_NOT_COMPUTING"/>,
    /// with its previous value if it had one.</summary>
    public IDictionary GetValueSync(int h, int client_cookie)
    {
        int i = _nodes.FindIndex(n => n.Handle == h);
        var d = (i < 0 ? null : _nodes[i].Result)?.ToDictionary() ?? new Hashtable();
        if (i < 0 || i >= _dirtyFrom || _nodes[i].Result == null) d[ResultKeys.Status] = NodeStatus.NODE_NOT_COMPUTING;
        return d;
    }

    public IDictionary GetValueAsync(int h, int client_cookie) => GetValueSync(h, client_cookie);

    /// <summary>The value of <paramref name="name"/> as seen just before <paramref name="handle"/>, or null.</summary>
    public DimVal? GetValueBefore(int handle, string name) => LookupIn(_nodes.FirstOrDefault(n => n.Handle == handle)?.EnvBefore, name);

    private static DimVal? LookupIn(Env? env, string name)
    {
        if (env == null) return null;
        try { return env.Lookup(name); }
        catch (CalcException) { return null; }
    }

    private static IDictionary ValueDictionary(DimVal? v) =>
        new Hashtable { [ResultKeys.Status] = v == null ? NodeStatus.NODE_INVALID : NodeStatus.NODE_READY, [ResultKeys.Value] = v };

    public IDictionary GetValueBeforeAsync(int h, string nm) => ValueDictionary(GetValueBefore(h, nm));

    public IDictionary GetValueAfter(int h, string nm) => ValueDictionary(LookupIn(NodeOf(h).EnvAfter, nm));

    public int IsBoundAt(int h, string nm) => GetValueBefore(h, nm) == null ? 0 : 1;
}
