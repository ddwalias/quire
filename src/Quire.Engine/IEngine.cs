#nullable disable
using System.Collections;
using Quire.Mirror;

namespace Quire.Engine;

/// <summary>Reports that node <paramref name="h"/> (added with <paramref name="client_cookie"/>) reached status
/// <paramref name="s"/>. The trigger arguments name the node whose change caused the update, when known.</summary>
[MirrorOf("Engine.NodeCallback, iengine")]
public delegate void NodeCallback(int h, NodeStatus s, int client_cookie, int trigger_handle, int trigger_cookie);

public delegate void BooleanCallback(bool A_0);

[MirrorOf("Engine.NodeStatus, iengine")]
public enum NodeStatus
{
    NODE_INVALID = 0,
    NODE_COMPUTING = 1,
    NODE_NOT_COMPUTING = 2,
    NODE_READY = 3,
    NODE_TREATED_READY = 4,
}

public enum NodeType
{
    AUTO_EVAL_ON = 0,
    AUTO_EVAL_OFF = 1,
}

/// <summary>
/// The calculation engine keeps an ordered list of nodes, one per region in worksheet order, each holding an Rl
/// expression. A node sees every definition made by the nodes before it, plus all global definitions. Handle 0
/// stands for "the start" in <see cref="AddAfter"/> and "the end" in <see cref="AddBefore"/>.
/// </summary>
public partial interface IEngine : IDisposable
{
    bool IsBusy { get; }

    /// <summary>Raised with true when a calculation pass has brought every node up to date.</summary>
    event BooleanCallback OnUpToDate;

    /// <summary>Raised with true when a calculation pass starts and false when it ends.</summary>
    event BooleanCallback OnBusy;

    int AddAfter(int h, string expr, int priority, int client_cookie);

    int AddBefore(int h, string expr, int priority, int client_cookie);

    void MoveAfter(int h1, int h2, int client_cookie);

    void MoveBefore(int h1, int h2, int client_cookie);

    void Remove(int h, int client_cookie);

    void Update(int h, string expr, int priority, int client_cookie);

    /// <summary>Like <see cref="Update"/>, but recomputes the node even when its expression is unchanged.</summary>
    void Replace(int h, string expr, int priority, int client_cookie);

    void SetPriority(int h, int priority);

    int GetPriority(int h);

    void SetAutoCalc(bool b);

    bool GetAutoCalc();

    /// <summary>The result of node <paramref name="h"/>; see <c>ResultKeys</c> for the entries.</summary>
    IDictionary GetValueSync(int h, int client_cookie);

    IDictionary GetValueAsync(int h, int client_cookie);

    /// <summary>The value of <paramref name="nm"/> as seen just after node <paramref name="h"/>.</summary>
    IDictionary GetValueAfter(int h, string nm);

    /// <summary>The value of <paramref name="nm"/> as seen just before node <paramref name="h"/>.</summary>
    IDictionary GetValueBeforeAsync(int h, string nm);

    /// <summary>1 when <paramref name="nm"/> has a value just before node <paramref name="h"/>, otherwise 0.</summary>
    int IsBoundAt(int h, string nm);

    IList GetTopNodes();

    /// <summary>The names node <paramref name="h"/> defines.</summary>
    IList GetDefined(int h);

    string GetVersion();

    void AddNodeCallback(NodeCallback A_0);

    bool IsUpToDate { get; }

    void RecalculateAll();

    /// <summary>Brings every node up to date; <paramref name="h"/> = 0 means all of them, otherwise the
    /// calculation stops after node <paramref name="h"/>. Stops early with <see cref="OperationCanceledException"/>
    /// when the calling thread's calculation is cancelled (see <c>Rts.RegisterThreadForCancellationChecking</c>).</summary>
    void CalculateNow(int h);
}

public partial interface IBuiltin
{
    object GetVariable(string name);
    void SetVariable(string name, object value);
    void UnsetVariable(string name);
    IList GetBuiltins();
}

public partial interface IEnginePriv : IEngine
{
    void Initialize(string appData);
}

public partial interface IEngineUnstable : IEngine
{
    int GetNodeType(int h);
}

public partial interface IMCEngine : IEngine
{
    void AddNotify(nint cookie);
    void RemoveNotify(nint cookie);
    void StartNotify();
}

