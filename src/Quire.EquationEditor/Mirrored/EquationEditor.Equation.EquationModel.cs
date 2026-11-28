#nullable disable
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Quire.Calculation.Core;
using Quire.Controls.Core;
using Quire.EquationEditor.Tree;
using Quire.EquationEditor.Tree.Navigation;
using Quire.Mirror;
using Quire.Wpf;
using System.Windows;
using Quire.EquationEditor.Tree.Transform;
using Quire.Native.PtcEquationEditor;
using Quire.Undo;

namespace Quire.EquationEditor.Equation;

public partial class EquationModel
{
    [MirrorName("z")]
    private EngineError z__;
    private volatile CalculatingStatus aw;
    private bool a6;
    private bool bb;
    [MirrorName("r")]
    private IEquationEditorSettings r__;
    [MirrorName("s")]
    private readonly RootMathNode s__;
    [MirrorName("t")]
    private ScriptingError t__;
    [MirrorName("v")]
    private MathNode v__;
    [MirrorName("w")]
    private bool w__;
    [MirrorName("b")]
    private readonly SExpressionToMathNodeTransform b__;
    [MirrorName("aa")]
    private List<string> aa__;
    [MirrorName("ad")]
    private bool ad__;

    [MirrorVerified("Release IL")]
    public event Action<bool> EquationChanged;

    [MirrorVerified("Release IL")]
    public event Action ExpressionValueChanged;

    [MirrorVerified("Release IL")]
    public event Action<bool> AlgorithmChanged;

    [MirrorVerified("Release IL")]
    public event Action EvaluationErrorOccurred;

    [MirrorVerified("Release IL")]
    public event Action WarningsOccurred;

    [MirrorVerified("Release IL")]
    public event Action InCustomInitializationChanged;

    [MirrorVerified("Release IL")]
    public event Action ContentChanged;

    [MirrorVerified("Release IL")]
    public event Action<PropertyChangedEventArgs<EngineError>> CalculationErrorChanged;

    [MirrorVerified("Release IL")]
    public event Action<ScopeStateChangeEventArgs> ScopeChanged;

    [MirrorVerified("Release IL")]
    public event Action<NavigatorStateChangeEventArgs> NavigatorStateChanged;

    [MirrorVerified("Release IL")]
    public event Action<TypeoverChangeEventArgs> TypeoverChanged;

    [MirrorVerified("Release IL")]
    public event Action<IResultNode> NumericResultChanged;

    [MirrorVerified("Release IL")]
    public event Action<NavigatorState> NodeMayBeReplaced;

    [MirrorVerified("Release IL")]
    public event Action<Direction> RequestRemoveEquation;

    [MirrorVerified("Release IL")]
    public event Action<IEquationModel> RequestEvaluation;

    [MirrorVerified("Release IL")]
    public event Action<IEquationModel> RequestFocus;

    [MirrorVerified("Release IL")]
    public event EventHandler<RequestRestoreInsertionPointEventArgs> RequestRestoreInsertionPoint;

    [MirrorVerified("Release IL")]
    public event Action<bool> RestrictedReadonlyChanged;

    [MirrorVerified("Release IL")]
    public event Action<Visibility> VisibleLhsOfDefinitionChanged;

    [MirrorVerified("Release IL")]
    public CalculatingStatus CalculatingStatus { get => aw; set => aw = value; }

    [MirrorVerified("Release IL")]
    public IEquationEditorSettings Settings { get => r__; set => r__ = value; }

    [MirrorVerified("Release IL")]
    public RootMathNode Root => s__;

    [MirrorVerified("Release IL")]
    public ScriptingError ScriptingError { get => t__; set => t__ = value; }

    [MirrorVerified("Release IL")]
    public bool IsScriptError => ScriptingError != null;

    [MirrorVerified("Release IL")]
    public bool IsEvaluatable => EquationRoot.IsEvaluatable;

    [MirrorVerified("Release IL")]
    public string EquationSExpression => Root.SExpression;

    [MirrorVerified("Release IL")]
    public string SerializedContent => Root.SExpression;

    [MirrorVerified("Release IL")]
    public bool HasVisualHistoryItems => aa__.Count > 1;

    [MirrorVerified("Release IL")]
    public bool IsEmpty => EquationRoot.IsPlaceholder;

    [MirrorVerified("Release IL")]
    public bool IsRestrictedReadOnly
    {
        get => a6;
        set
        {
            if (value == a6)
            {
                return;
            }

            a6 = value;
            if (RestrictedReadonlyChanged != null)
            {
                RestrictedReadonlyChanged(IsRestrictedReadOnly);
            }
        }
    }

    [MirrorVerified("Release IL")]
    public bool InCustomInitialization
    {
        get => bb;
        set
        {
            if (value != bb)
            {
                bb = value;
                if (InCustomInitializationChanged != null)
                {
                    InCustomInitializationChanged();
                }
            }
        }
    }

    [MirrorVerified("Release IL")]
    public MathNode ResultScalingFactor { get => v__; set => v__ = value; }

    [MirrorVerified("Release IL")]
    public bool ResultScalingModified { get => w__; set => w__ = value; }

    [MirrorVerified("Release IL")]
    public string ResultScalingSExpression
    {
        get
        {
            if (v__ == null)
            {
                return string.Empty;
            }

            return v__.SExpression;
        }

        set => v__ = b__.Transform(value);
    }

    [MirrorVerified("Release IL")]
    public bool AllowExpressionsOnly
    {
        get => ad__;
        set
        {
            if (IsEvaluatable)
            {
                return;
            }

            ad__ = value;
        }
    }

    [MirrorVerified("Release IL")]
    public void CanPerformIdLabelOperations(ref bool canPerformIdLabelOperations) => canPerformIdLabelOperations = z();

    [MirrorVerified("Release IL")]
    public void CanPerformColumnOperations(ref bool canPerformColumnOperation) => canPerformColumnOperation = u();

    [MirrorVerified("Release IL")]
    public void CanPerformLinearNaryOperations(ref bool canPerformLinearNaryOperations) => canPerformLinearNaryOperations = s();

    [MirrorVerified("Release IL")]
    public void CanPerformRowOperations(ref bool canPerformRowOperations) => canPerformRowOperations = r();

    [MirrorVerified("Release IL")]
    public void CanCopy(ref bool canCopy) => canCopy = CanCopy();

    [MirrorVerified("Release IL")]
    public void CanCut(ref bool canCut) => canCut = CanCut();

    [MirrorVerified("Release IL")]
    public void CanPaste(ref bool canPaste) => canPaste = CanPaste();

    [MirrorVerified("Release IL")]
    public void ClearError()
    {
        ErrorNode = null;
        CalculationError = null;
    }

    [MirrorVerified("Release IL")]
    public bool SetIsReplacingPreviouseOperator(bool isReplacingPreviouseOperator)
    {
        // Mathcad combines the two bools with a non-short-circuit `and`, so this is `&`, not `&&`.
        if (PrevStepForMultiKeyOpFailed & isReplacingPreviouseOperator)
        {
            return false;
        }

        IsReplacingPreviouseOperator = isReplacingPreviouseOperator;
        return true;
    }

    [MirrorVerified("Release IL")]
    public string GetDefinitionSExpression(string idSExpr)
    {
        MathNode definition = d(idSExpr).LastOrDefault();
        if (definition != null)
        {
            return definition.SExpression;
        }

        return null;
    }

    [MirrorVerified("Release IL")]
    public void EnterNode(Direction direction, MathNode nodeToEnter) => NavigatorState = bq.c(direction, nodeToEnter).f();

    [MirrorVerified("Release IL")]
    public virtual void HandleScriptingEngineError(string error)
    {
        if (error == null)
        {
            t__ = null;
        }
        else
        {
            t__ = new ScriptingError(error);
        }

        EvaluationErrorOccurred?.Invoke();
    }

    [MirrorVerified("Release IL")]
    public NavigatorState GetNavigatorStateFromScoperCaretSide() => g__.a();

    [MirrorVerified("Release IL")]
    public void FireAlgorithmChanged()
    {
        IsExpressionValueModified = true;
        AlgorithmChanged(true);
        FireNavigatorStateChanged();
    }

    [MirrorVerified("Release IL")]
    public bool CanInsertKeyword()
    {
        if (Mode != 0)
        {
            return false;
        }

        if (AllowExpressionsOnly)
        {
            return false;
        }

        return true;
    }

    [MirrorVerified("Release IL")]
    public void FireRequestFocus()
    {
        if (RequestFocus != null)
        {
            RequestFocus(this);
        }
    }

    [MirrorVerified("Release IL")]
    public void FireExpressionValueChanged(NavigatorState navigatorState, bool shouldRegenerate = true)
    {
        IsExpressionValueModified = true;
        ExpressionValueChanged?.Invoke();
        FireEquationChanged(shouldRegenerate);
        a(navigatorState);
    }

    [MirrorVerified("Release IL")]
    public void FireEquationChanged(bool shouldRegenerate = true)
    {
        IsModified = true;
        EquationChanged?.Invoke(shouldRegenerate);
        FireContentChanged();
    }

    [MirrorVerified("Release IL")]
    public void FireContentChanged()
    {
        if (ContentChanged != null)
        {
            ContentChanged();
        }
    }

    [MirrorVerified("Release IL")]
    public void FireNavigatorStateChanged()
    {
        if (NavigatorStateChanged != null)
        {
            NavigatorStateChanged(new NavigatorStateChangeEventArgs(NavigatorState));
        }
    }

    [MirrorVerified("Release IL")]
    public void FireNodeMayBeReplaced(NavigatorState navigatorState)
    {
        if (NodeMayBeReplaced != null)
        {
            NodeMayBeReplaced(navigatorState);
        }
    }

    [MirrorVerified("Release IL")]
    public void HookupDescendantsMatrixFormatChanged() => HookupDescendantsMatrixFormatChanged(EquationRoot);

    [MirrorVerified("Release IL")]
    public void MoveToRegionExtreme(Direction direction)
    {
        switch (direction)
        {
            case Direction.Left:
                MoveToRegionStart();
                break;
            case Direction.Right:
                MoveToRegionEnd();
                break;
        }
    }

    [MirrorVerified("Release IL")]
    public bool SetEquationSExpressionWithAutoLabeling(string sExpression) => sExpression == Root.SExpression || a(sExpression, true);

    [MirrorVerified("Release IL")]
    public void SwitchLineBreakOperator()
    {
        MathNode node = v();
        if (node != null)
        {
            InsertOperator(node.SplitOposite);
        }
    }

    [MirrorVerified("Release IL")]
    public void CanNavigateToOppositeOfScope(ref bool canNavigateToOppositeOfScope)
    {
        if (Mode == EquationEditModeEnum.Scope)
        {
            canNavigateToOppositeOfScope = true;
        }
        else
        {
            canNavigateToOppositeOfScope = false;
        }
    }

    [MirrorVerified("Release IL")]
    public void InsertValue(string valueToInsert)
    {
        FormattedString formatted;
        if (Mode == EquationEditModeEnum.Normal && NavigatorState.CaretInsertionType == CaretInsertionTypes.Subscript)
        {
            formatted = FormattedString.CreateEditable();
            formatted.Insert(0, valueToInsert, 1);
        }
        else
        {
            formatted = FormattedString.CreateEditable(valueToInsert);
        }

        a(formatted);
    }

    [MirrorVerified("Release IL")]
    public void EditNode(MathNode node, int position)
    {
        if (node == null)
        {
            throw new ArgumentNullException("node");
        }

        NavigatorState state = new NavigatorState(node, position);
        NavigatorState = state;
    }

    [MirrorVerified("Release IL")]
    public void OnTraverseForward(ref bool handled) => handled = a(Direction.Right);

    [MirrorVerified("Release IL")]
    public void OnTraverseBackward(ref bool handled) => handled = a(Direction.Left);

    [MirrorVerified("Release IL")]
    public MathNode EquationRoot
    {
        get => s__[0];
        set
        {
            h_ closure = new h_();
            closure.a_ = this;
            closure.b = value;
            if (closure.b == null)
            {
                throw new ArgumentNullException("value");
            }

            if (closure.b == s__.Children[0])
            {
                return;
            }

            if (CommandScope.CanUndoTreeOperations)
            {
                i_ command = new i_();
                command.b = closure;
                command.a_ = s__[0];
                CommandBuffer.Execute("Set Root Node", new DelegateFunction0<bool>(command.b.a), new Delegate0(command.a));
            }
            else
            {
                s__[0] = closure.b;
            }

            NavigatorState state = bq.c(Direction.Left, EquationRoot).f();
            FireExpressionValueChanged(state, true);
            q = state;
            HookupDescendantsMatrixFormatChanged();
            if (s__[0].Descriptor is MathNodeDescriptorPiggyback && RequestEvaluation != null)
            {
                RequestEvaluation(this);
            }
        }
    }

    [MirrorVerified("Release IL")]
    public EngineError CalculationError
    {
        get => z__;
        set
        {
            if (z__ == value)
            {
                return;
            }

            EngineError previous = z__;
            z__ = value;
            if (CalculationErrorChanged != null)
            {
                PropertyChangedEventArgs<EngineError> args = new PropertyChangedEventArgs<EngineError>("CalculationError", previous, value);
                CalculationErrorChanged(args);
            }
        }
    }

    private bool a8;

    [MirrorVerified("Release IL")]
    public bool ProhibitComboBox
    {
        get => a8;
        set
        {
            if (value)
            {
                if (DebugTools.DevAssert(!EquationRoot.ContainsOrIsComboBox, null).Failed)
                {
                    return;
                }
            }

            a8 = value;
        }
    }

    [MirrorVerified("Release IL")]
    [MirrorBlocked("IL keeps ab/ac on the eval stack across two delegate creations (dup, ldftn b, ldftn a), no closure local; Roslyn cannot emit")]
    public NumberFormat ResultNumberFormat
    {
        get => k__.DisplayNumberFormat.Clone();
        set => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    }

    [MirrorVerified("Release IL")]
    [MirrorBlocked("IL keeps ab/ac on the eval stack across two delegate creations (dup, ldftn b, ldftn a), no closure local; Roslyn cannot emit")]
    public MatrixFormat ResultMatrixFormat
    {
        get => k__.ResultMatrixFormat.Clone();
        set => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    }

    [MirrorVerified("Release IL")]
    public bool SetEquationSExpression(string sExpr, bool isUserOperation = true)
    {
        if (isUserOperation)
        {
            DebugTools.DevAssert(!IsRestrictedReadOnly, null);
        }

        if (sExpr == Root.SExpression)
        {
            return true;
        }

        return a(sExpr, false);
    }

    [MirrorVerified("Release IL")]
    [SkipLocalsInit] // Mathcad has .initlocals False
    public void ScopeResult()
    {
        if (DebugTools.DevAssert(EvaluationRoot != null && EvaluationRoot.DescriptorName == "EVALUATION", null).Passed)
        {
            ScopeNode(EvaluationRoot[1]);
        }
    }
}
