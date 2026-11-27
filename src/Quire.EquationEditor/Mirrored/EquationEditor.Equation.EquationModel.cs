#nullable disable
using System;
using System.Threading;
using Quire.Calculation.Core;
using Quire.Controls.Core;
using Quire.EquationEditor.Tree;
using Quire.EquationEditor.Tree.Navigation;
using Quire.Mirror;
using Quire.Wpf;
using System.Windows;

namespace Quire.EquationEditor.Equation;

public partial class EquationModel
{
    private Action<bool> af;

    [MirrorVerified("Release IL")]
    public event Action<bool> EquationChanged
    {
        add
        {
            Action<bool> previous = af;
            Action<bool> current;
            do
            {
                current = previous;
                Action<bool> updated = (Action<bool>)Delegate.Combine(current, value);
                previous = Interlocked.CompareExchange(ref af, updated, current);
            }
            while ((object)previous != current);
        }
        remove
        {
            Action<bool> previous = af;
            Action<bool> current;
            do
            {
                current = previous;
                Action<bool> updated = (Action<bool>)Delegate.Remove(current, value);
                previous = Interlocked.CompareExchange(ref af, updated, current);
            }
            while ((object)previous != current);
        }
    }

    private Action ag;

    [MirrorVerified("Release IL")]
    public event Action ExpressionValueChanged
    {
        add
        {
            Action previous = ag;
            Action current;
            do
            {
                current = previous;
                Action updated = (Action)Delegate.Combine(current, value);
                previous = Interlocked.CompareExchange(ref ag, updated, current);
            }
            while ((object)previous != current);
        }
        remove
        {
            Action previous = ag;
            Action current;
            do
            {
                current = previous;
                Action updated = (Action)Delegate.Remove(current, value);
                previous = Interlocked.CompareExchange(ref ag, updated, current);
            }
            while ((object)previous != current);
        }
    }

    private Action<bool> ah;

    [MirrorVerified("Release IL")]
    public event Action<bool> AlgorithmChanged
    {
        add
        {
            Action<bool> previous = ah;
            Action<bool> current;
            do
            {
                current = previous;
                Action<bool> updated = (Action<bool>)Delegate.Combine(current, value);
                previous = Interlocked.CompareExchange(ref ah, updated, current);
            }
            while ((object)previous != current);
        }
        remove
        {
            Action<bool> previous = ah;
            Action<bool> current;
            do
            {
                current = previous;
                Action<bool> updated = (Action<bool>)Delegate.Remove(current, value);
                previous = Interlocked.CompareExchange(ref ah, updated, current);
            }
            while ((object)previous != current);
        }
    }

    private Action ai;

    [MirrorVerified("Release IL")]
    public event Action EvaluationErrorOccurred
    {
        add
        {
            Action previous = ai;
            Action current;
            do
            {
                current = previous;
                Action updated = (Action)Delegate.Combine(current, value);
                previous = Interlocked.CompareExchange(ref ai, updated, current);
            }
            while ((object)previous != current);
        }
        remove
        {
            Action previous = ai;
            Action current;
            do
            {
                current = previous;
                Action updated = (Action)Delegate.Remove(current, value);
                previous = Interlocked.CompareExchange(ref ai, updated, current);
            }
            while ((object)previous != current);
        }
    }

    private Action aj;

    [MirrorVerified("Release IL")]
    public event Action WarningsOccurred
    {
        add
        {
            Action previous = aj;
            Action current;
            do
            {
                current = previous;
                Action updated = (Action)Delegate.Combine(current, value);
                previous = Interlocked.CompareExchange(ref aj, updated, current);
            }
            while ((object)previous != current);
        }
        remove
        {
            Action previous = aj;
            Action current;
            do
            {
                current = previous;
                Action updated = (Action)Delegate.Remove(current, value);
                previous = Interlocked.CompareExchange(ref aj, updated, current);
            }
            while ((object)previous != current);
        }
    }

    private Action ak;

    [MirrorVerified("Release IL")]
    public event Action InCustomInitializationChanged
    {
        add
        {
            Action previous = ak;
            Action current;
            do
            {
                current = previous;
                Action updated = (Action)Delegate.Combine(current, value);
                previous = Interlocked.CompareExchange(ref ak, updated, current);
            }
            while ((object)previous != current);
        }
        remove
        {
            Action previous = ak;
            Action current;
            do
            {
                current = previous;
                Action updated = (Action)Delegate.Remove(current, value);
                previous = Interlocked.CompareExchange(ref ak, updated, current);
            }
            while ((object)previous != current);
        }
    }

    private Action al;

    [MirrorVerified("Release IL")]
    public event Action ContentChanged
    {
        add
        {
            Action previous = al;
            Action current;
            do
            {
                current = previous;
                Action updated = (Action)Delegate.Combine(current, value);
                previous = Interlocked.CompareExchange(ref al, updated, current);
            }
            while ((object)previous != current);
        }
        remove
        {
            Action previous = al;
            Action current;
            do
            {
                current = previous;
                Action updated = (Action)Delegate.Remove(current, value);
                previous = Interlocked.CompareExchange(ref al, updated, current);
            }
            while ((object)previous != current);
        }
    }

    private Action<PropertyChangedEventArgs<EngineError>> am;

    [MirrorVerified("Release IL")]
    public event Action<PropertyChangedEventArgs<EngineError>> CalculationErrorChanged
    {
        add
        {
            Action<PropertyChangedEventArgs<EngineError>> previous = am;
            Action<PropertyChangedEventArgs<EngineError>> current;
            do
            {
                current = previous;
                Action<PropertyChangedEventArgs<EngineError>> updated = (Action<PropertyChangedEventArgs<EngineError>>)Delegate.Combine(current, value);
                previous = Interlocked.CompareExchange(ref am, updated, current);
            }
            while ((object)previous != current);
        }
        remove
        {
            Action<PropertyChangedEventArgs<EngineError>> previous = am;
            Action<PropertyChangedEventArgs<EngineError>> current;
            do
            {
                current = previous;
                Action<PropertyChangedEventArgs<EngineError>> updated = (Action<PropertyChangedEventArgs<EngineError>>)Delegate.Remove(current, value);
                previous = Interlocked.CompareExchange(ref am, updated, current);
            }
            while ((object)previous != current);
        }
    }

    private Action<ScopeStateChangeEventArgs> an;

    [MirrorVerified("Release IL")]
    public event Action<ScopeStateChangeEventArgs> ScopeChanged
    {
        add
        {
            Action<ScopeStateChangeEventArgs> previous = an;
            Action<ScopeStateChangeEventArgs> current;
            do
            {
                current = previous;
                Action<ScopeStateChangeEventArgs> updated = (Action<ScopeStateChangeEventArgs>)Delegate.Combine(current, value);
                previous = Interlocked.CompareExchange(ref an, updated, current);
            }
            while ((object)previous != current);
        }
        remove
        {
            Action<ScopeStateChangeEventArgs> previous = an;
            Action<ScopeStateChangeEventArgs> current;
            do
            {
                current = previous;
                Action<ScopeStateChangeEventArgs> updated = (Action<ScopeStateChangeEventArgs>)Delegate.Remove(current, value);
                previous = Interlocked.CompareExchange(ref an, updated, current);
            }
            while ((object)previous != current);
        }
    }

    private Action<NavigatorStateChangeEventArgs> ao;

    [MirrorVerified("Release IL")]
    public event Action<NavigatorStateChangeEventArgs> NavigatorStateChanged
    {
        add
        {
            Action<NavigatorStateChangeEventArgs> previous = ao;
            Action<NavigatorStateChangeEventArgs> current;
            do
            {
                current = previous;
                Action<NavigatorStateChangeEventArgs> updated = (Action<NavigatorStateChangeEventArgs>)Delegate.Combine(current, value);
                previous = Interlocked.CompareExchange(ref ao, updated, current);
            }
            while ((object)previous != current);
        }
        remove
        {
            Action<NavigatorStateChangeEventArgs> previous = ao;
            Action<NavigatorStateChangeEventArgs> current;
            do
            {
                current = previous;
                Action<NavigatorStateChangeEventArgs> updated = (Action<NavigatorStateChangeEventArgs>)Delegate.Remove(current, value);
                previous = Interlocked.CompareExchange(ref ao, updated, current);
            }
            while ((object)previous != current);
        }
    }

    private Action<TypeoverChangeEventArgs> ap;

    [MirrorVerified("Release IL")]
    public event Action<TypeoverChangeEventArgs> TypeoverChanged
    {
        add
        {
            Action<TypeoverChangeEventArgs> previous = ap;
            Action<TypeoverChangeEventArgs> current;
            do
            {
                current = previous;
                Action<TypeoverChangeEventArgs> updated = (Action<TypeoverChangeEventArgs>)Delegate.Combine(current, value);
                previous = Interlocked.CompareExchange(ref ap, updated, current);
            }
            while ((object)previous != current);
        }
        remove
        {
            Action<TypeoverChangeEventArgs> previous = ap;
            Action<TypeoverChangeEventArgs> current;
            do
            {
                current = previous;
                Action<TypeoverChangeEventArgs> updated = (Action<TypeoverChangeEventArgs>)Delegate.Remove(current, value);
                previous = Interlocked.CompareExchange(ref ap, updated, current);
            }
            while ((object)previous != current);
        }
    }

    private Action<IResultNode> aq;

    [MirrorVerified("Release IL")]
    public event Action<IResultNode> NumericResultChanged
    {
        add
        {
            Action<IResultNode> previous = aq;
            Action<IResultNode> current;
            do
            {
                current = previous;
                Action<IResultNode> updated = (Action<IResultNode>)Delegate.Combine(current, value);
                previous = Interlocked.CompareExchange(ref aq, updated, current);
            }
            while ((object)previous != current);
        }
        remove
        {
            Action<IResultNode> previous = aq;
            Action<IResultNode> current;
            do
            {
                current = previous;
                Action<IResultNode> updated = (Action<IResultNode>)Delegate.Remove(current, value);
                previous = Interlocked.CompareExchange(ref aq, updated, current);
            }
            while ((object)previous != current);
        }
    }

    private Action<NavigatorState> ar;

    [MirrorVerified("Release IL")]
    public event Action<NavigatorState> NodeMayBeReplaced
    {
        add
        {
            Action<NavigatorState> previous = ar;
            Action<NavigatorState> current;
            do
            {
                current = previous;
                Action<NavigatorState> updated = (Action<NavigatorState>)Delegate.Combine(current, value);
                previous = Interlocked.CompareExchange(ref ar, updated, current);
            }
            while ((object)previous != current);
        }
        remove
        {
            Action<NavigatorState> previous = ar;
            Action<NavigatorState> current;
            do
            {
                current = previous;
                Action<NavigatorState> updated = (Action<NavigatorState>)Delegate.Remove(current, value);
                previous = Interlocked.CompareExchange(ref ar, updated, current);
            }
            while ((object)previous != current);
        }
    }

    private Action<Direction> @as;

    [MirrorVerified("Release IL")]
    public event Action<Direction> RequestRemoveEquation
    {
        add
        {
            Action<Direction> previous = @as;
            Action<Direction> current;
            do
            {
                current = previous;
                Action<Direction> updated = (Action<Direction>)Delegate.Combine(current, value);
                previous = Interlocked.CompareExchange(ref @as, updated, current);
            }
            while ((object)previous != current);
        }
        remove
        {
            Action<Direction> previous = @as;
            Action<Direction> current;
            do
            {
                current = previous;
                Action<Direction> updated = (Action<Direction>)Delegate.Remove(current, value);
                previous = Interlocked.CompareExchange(ref @as, updated, current);
            }
            while ((object)previous != current);
        }
    }

    private Action<IEquationModel> at;

    [MirrorVerified("Release IL")]
    public event Action<IEquationModel> RequestEvaluation
    {
        add
        {
            Action<IEquationModel> previous = at;
            Action<IEquationModel> current;
            do
            {
                current = previous;
                Action<IEquationModel> updated = (Action<IEquationModel>)Delegate.Combine(current, value);
                previous = Interlocked.CompareExchange(ref at, updated, current);
            }
            while ((object)previous != current);
        }
        remove
        {
            Action<IEquationModel> previous = at;
            Action<IEquationModel> current;
            do
            {
                current = previous;
                Action<IEquationModel> updated = (Action<IEquationModel>)Delegate.Remove(current, value);
                previous = Interlocked.CompareExchange(ref at, updated, current);
            }
            while ((object)previous != current);
        }
    }

    private Action<IEquationModel> au;

    [MirrorVerified("Release IL")]
    public event Action<IEquationModel> RequestFocus
    {
        add
        {
            Action<IEquationModel> previous = au;
            Action<IEquationModel> current;
            do
            {
                current = previous;
                Action<IEquationModel> updated = (Action<IEquationModel>)Delegate.Combine(current, value);
                previous = Interlocked.CompareExchange(ref au, updated, current);
            }
            while ((object)previous != current);
        }
        remove
        {
            Action<IEquationModel> previous = au;
            Action<IEquationModel> current;
            do
            {
                current = previous;
                Action<IEquationModel> updated = (Action<IEquationModel>)Delegate.Remove(current, value);
                previous = Interlocked.CompareExchange(ref au, updated, current);
            }
            while ((object)previous != current);
        }
    }

    private EventHandler<RequestRestoreInsertionPointEventArgs> av;

    [MirrorVerified("Release IL")]
    public event EventHandler<RequestRestoreInsertionPointEventArgs> RequestRestoreInsertionPoint
    {
        add
        {
            EventHandler<RequestRestoreInsertionPointEventArgs> previous = av;
            EventHandler<RequestRestoreInsertionPointEventArgs> current;
            do
            {
                current = previous;
                EventHandler<RequestRestoreInsertionPointEventArgs> updated = (EventHandler<RequestRestoreInsertionPointEventArgs>)Delegate.Combine(current, value);
                previous = Interlocked.CompareExchange(ref av, updated, current);
            }
            while ((object)previous != current);
        }
        remove
        {
            EventHandler<RequestRestoreInsertionPointEventArgs> previous = av;
            EventHandler<RequestRestoreInsertionPointEventArgs> current;
            do
            {
                current = previous;
                EventHandler<RequestRestoreInsertionPointEventArgs> updated = (EventHandler<RequestRestoreInsertionPointEventArgs>)Delegate.Remove(current, value);
                previous = Interlocked.CompareExchange(ref av, updated, current);
            }
            while ((object)previous != current);
        }
    }

    private Action<bool> a5;

    [MirrorVerified("Release IL")]
    public event Action<bool> RestrictedReadonlyChanged
    {
        add
        {
            Action<bool> previous = a5;
            Action<bool> current;
            do
            {
                current = previous;
                Action<bool> updated = (Action<bool>)Delegate.Combine(current, value);
                previous = Interlocked.CompareExchange(ref a5, updated, current);
            }
            while ((object)previous != current);
        }
        remove
        {
            Action<bool> previous = a5;
            Action<bool> current;
            do
            {
                current = previous;
                Action<bool> updated = (Action<bool>)Delegate.Remove(current, value);
                previous = Interlocked.CompareExchange(ref a5, updated, current);
            }
            while ((object)previous != current);
        }
    }

    private Action<Visibility> be;

    [MirrorVerified("Release IL")]
    public event Action<Visibility> VisibleLhsOfDefinitionChanged
    {
        add
        {
            Action<Visibility> previous = be;
            Action<Visibility> current;
            do
            {
                current = previous;
                Action<Visibility> updated = (Action<Visibility>)Delegate.Combine(current, value);
                previous = Interlocked.CompareExchange(ref be, updated, current);
            }
            while ((object)previous != current);
        }
        remove
        {
            Action<Visibility> previous = be;
            Action<Visibility> current;
            do
            {
                current = previous;
                Action<Visibility> updated = (Action<Visibility>)Delegate.Remove(current, value);
                previous = Interlocked.CompareExchange(ref be, updated, current);
            }
            while ((object)previous != current);
        }
    }
}
