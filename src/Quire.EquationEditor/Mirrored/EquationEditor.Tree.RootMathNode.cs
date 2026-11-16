using System;
using System.Threading;

namespace Quire.EquationEditor.Tree;

public partial class RootMathNode
{
    private static readonly MathNodeDescriptor k;
    private Action l;
    private EventHandler<RequestRestoreInsertionPointEventArgs> m;
    private Action<DelegateFunction0<bool>, string, MathNode> n;
    private Action<MathNode> o;

    static RootMathNode()
    {
        k = new MathNodeDescriptor("ROOT");
        k.HasVisualRepresentation = false;
        k.ChildCount = 1;
    }

    public RootMathNode(MathNode child)
        : base(k, new MathNode[] { child })
    {
    }

    [System.Runtime.CompilerServices.SkipLocalsInit]
    public RootMathNode(RootMathNode source)
        : base(k, new MathNode[] { source.Children[0].Clone() })
    {
        if (source.Descriptor.DescriptorName != k.DescriptorName)
        {
            Descriptor = DefaultDescriptorTable.Default[source.Descriptor.DescriptorName];
        }
    }

    public override string SExpression
    {
        get
        {
            if (Children[0] == null)
            {
                return string.Empty;
            }

            return Children[0].SExpression;
        }
    }

    public override MathNode ScopeRoot => Children[0].ScopeRoot;

    public event Action SubTreeNodeValueChanged
    {
        add
        {
            Action action = l;
            Action action2;
            do
            {
                action2 = action;
                Action value2 = (Action)Delegate.Combine(action2, value);
                action = Interlocked.CompareExchange(ref l, value2, action2);
            }
            while ((object)action != action2);
        }
        remove
        {
            Action action = l;
            Action action2;
            do
            {
                action2 = action;
                Action value2 = (Action)Delegate.Remove(action2, value);
                action = Interlocked.CompareExchange(ref l, value2, action2);
            }
            while ((object)action != action2);
        }
    }

    public event EventHandler<RequestRestoreInsertionPointEventArgs> RequestRestoreInsertionPoint
    {
        add
        {
            EventHandler<RequestRestoreInsertionPointEventArgs> eventHandler = m;
            EventHandler<RequestRestoreInsertionPointEventArgs> eventHandler2;
            do
            {
                eventHandler2 = eventHandler;
                EventHandler<RequestRestoreInsertionPointEventArgs> value2 = (EventHandler<RequestRestoreInsertionPointEventArgs>)Delegate.Combine(eventHandler2, value);
                eventHandler = Interlocked.CompareExchange(ref m, value2, eventHandler2);
            }
            while ((object)eventHandler != eventHandler2);
        }
        remove
        {
            EventHandler<RequestRestoreInsertionPointEventArgs> eventHandler = m;
            EventHandler<RequestRestoreInsertionPointEventArgs> eventHandler2;
            do
            {
                eventHandler2 = eventHandler;
                EventHandler<RequestRestoreInsertionPointEventArgs> value2 = (EventHandler<RequestRestoreInsertionPointEventArgs>)Delegate.Remove(eventHandler2, value);
                eventHandler = Interlocked.CompareExchange(ref m, value2, eventHandler2);
            }
            while ((object)eventHandler != eventHandler2);
        }
    }

    public event Action<DelegateFunction0<bool>, string, MathNode> RequestUndoableCommandExecution
    {
        add
        {
            Action<DelegateFunction0<bool>, string, MathNode> action = n;
            Action<DelegateFunction0<bool>, string, MathNode> action2;
            do
            {
                action2 = action;
                Action<DelegateFunction0<bool>, string, MathNode> value2 = (Action<DelegateFunction0<bool>, string, MathNode>)Delegate.Combine(action2, value);
                action = Interlocked.CompareExchange(ref n, value2, action2);
            }
            while ((object)action != action2);
        }
        remove
        {
            Action<DelegateFunction0<bool>, string, MathNode> action = n;
            Action<DelegateFunction0<bool>, string, MathNode> action2;
            do
            {
                action2 = action;
                Action<DelegateFunction0<bool>, string, MathNode> value2 = (Action<DelegateFunction0<bool>, string, MathNode>)Delegate.Remove(action2, value);
                action = Interlocked.CompareExchange(ref n, value2, action2);
            }
            while ((object)action != action2);
        }
    }

    public event Action<MathNode> MatrixDescendantAdded
    {
        add
        {
            Action<MathNode> action = o;
            Action<MathNode> action2;
            do
            {
                action2 = action;
                Action<MathNode> value2 = (Action<MathNode>)Delegate.Combine(action2, value);
                action = Interlocked.CompareExchange(ref o, value2, action2);
            }
            while ((object)action != action2);
        }
        remove
        {
            Action<MathNode> action = o;
            Action<MathNode> action2;
            do
            {
                action2 = action;
                Action<MathNode> value2 = (Action<MathNode>)Delegate.Remove(action2, value);
                action = Interlocked.CompareExchange(ref o, value2, action2);
            }
            while ((object)action != action2);
        }
    }

    public void RequestNotifySubTreeNodeValueChanged(MathNode mathNode)
    {
        l?.Invoke();
    }

    public void DelegateRequestRestoreInsertionPoint(RequestRestoreInsertionPointEventArgs requestRestoreInsertionPointEventArgs)
    {
        m?.Invoke(this, requestRestoreInsertionPointEventArgs);
    }

    public void DispatchRequestUndoableCommandExecution(DelegateFunction0<bool> action, string commandName, MathNode nodeToRestore)
    {
        n?.Invoke(action, commandName, nodeToRestore);
    }

    public MathNode FindDescendant(string labelOrSExp)
    {
        if (!TryFindDescendant(labelOrSExp, out var foundNode))
        {
            throw new ArgumentException("Node not found", "labelOrSExp");
        }

        return foundNode;
    }

    private MathNode b(MathNode A_0, string A_1)
    {
        MathNode mathNode = null;
        foreach (MathNode child in A_0.Children)
        {
            mathNode = b(child, A_1);
            if (mathNode != null)
            {
                return mathNode;
            }
        }

        if (A_1 == A_0.Label)
        {
            return A_0;
        }

        return mathNode;
    }

    private MathNode a(MathNode A_0, string A_1)
    {
        MathNode mathNode = null;
        foreach (MathNode child in A_0.Children)
        {
            mathNode = a(child, A_1);
            if (mathNode != null)
            {
                return mathNode;
            }
        }

        if (A_1 == A_0.SExpression)
        {
            return A_0;
        }

        return mathNode;
    }

    [System.Runtime.CompilerServices.SkipLocalsInit]
    public bool TryFindDescendant(string labelOrSExp, out MathNode foundNode)
    {
        foundNode = null;
        if (string.IsNullOrEmpty(labelOrSExp))
        {
            return false;
        }

        if (Children[0] == null)
        {
            return false;
        }

        foundNode = b(Children[0], labelOrSExp) ?? a(Children[0], labelOrSExp);
        if (foundNode == null)
        {
            return false;
        }

        return true;
    }

    public void FireMatrixDescendantAdded(MathNode matrix)
    {
        if (o != null)
        {
            o(matrix);
        }
    }
}
