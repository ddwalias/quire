#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Quire.Calculation.Core;
using Quire.Controls.Core;
using Quire.EquationEditor.Equation;
using Quire.EquationEditor.Tree;
using Quire.EquationEditor.Tree.Navigation;
using Quire.EquationEditor.View;
using Quire.Matrices;
using Quire.Mirror;
using Quire.Services;
using Quire.Wpf;

namespace Quire.EquationEditor;

public partial class EquationPresenter
{
    [MirrorName("a")]
    private readonly IEquationModel a__;

    [MirrorName("b")]
    private readonly IVisualMathRegionNode b__;

    [MirrorName("c")]
    private readonly IMathNodeToVisualNodeMap c__;

    [MirrorName("d")]
    private readonly Native.PtcEquationEditor.c d_;

    [MirrorName("e")]
    private readonly Native.PtcEquationEditor.b e_;

    [MirrorName("f")]
    private bool f_;

    [MirrorName("g")]
    private DispatcherOperation g_;

    public static bool WasSuspendedRegenerate;

    [MirrorName("h")]
    private static IFrameworkElementFacade h_;

    [MirrorName("i")]
    private a_ i_;

    protected static IFrameworkElementFacade FrameworkElementFacade
    {
        get
        {
            if (h_ == null || !h_.IsConnected)
            {
                return h_ = AppServiceRegistry.GetService<IFrameworkElementFacade>();
            }
            return h_;
        }
    }

    [MirrorVerified("Release IL")]
    public EquationPresenter(IEquationModel model, IVisualMathRegionNode view) : this(model, view, null, null, null) { }

    [MirrorVerified("Release IL")]
    public EquationPresenter(IEquationModel model, IVisualMathRegionNode view, IMathNodeToVisualNodeMap map) : this(model, view, map, null, null) { }

    [System.Runtime.CompilerServices.SkipLocalsInit]
    internal EquationPresenter(IEquationModel A_0, IVisualMathRegionNode A_1, IMathNodeToVisualNodeMap A_2, Native.PtcEquationEditor.c A_3, Native.PtcEquationEditor.b A_4)
    {
        a__ = A_0;
        b__ = A_1;
        c__ = A_2 ?? new MathNodeToVisualNodeMap(b__.VisualFactory, A_0.Settings);
        d_ = A_3 ?? new Native.PtcEquationEditor.g(b__.VisualFactory);
        e_ = A_4 ?? new Native.PtcEquationEditor.d();
        r();
        u();
        if (a__.InCustomInitialization)
        {
            a(a_.b);
        }
        else
        {
            m();
        }
    }

    [System.Runtime.CompilerServices.SkipLocalsInit]
    private void x()
    {
        b__.DeleteLeft += i;
        b__.DeleteRight += h;
        b__.KeyGesture += a;
        b__.InsertEvaluation += a__.ProcessEqualsKey;
        b__.InsertSymbolicEvaluation += a__.InsertSymbolicEvaluation;
        b__.InsertOperator += a__.InsertOperator;
        b__.SwitchLineBreakOperator += a__.SwitchLineBreakOperator;
        b__.CanToggleLineBreakOperator += a__.CanToggleLineBreakOperator;
        b__.InsertString += a__.InsertString;
        b__.InsertInputControl += a__.InsertInputControl;
        b__.CanInsertInputControl += a__.CanInsertInputControl;
        b__.InsertMatrix += a__.InsertMatrix;
        b__.InsertFunction += a__.InsertFunction;
        b__.CanInsertFunction += a__.CanInsertFunction;
        b__.InsertFunctionWithArgs += a__.InsertFunctionWithArgs;
        b__.InsertId += a__.InsertId;
        b__.InsertKeyword += a__.InsertKeyword;
        b__.InsertModifier += a__.InsertModifier;
        b__.Replace += a__.Replace;
        b__.Greek += a__.ToggleGreek;
        b__.ToggleSubscript += a__.ToggleSubscript;
        b__.ExitGroupRight += a__.CloseParen;
        b__.ConvertToOperator += a__.ConvertToOperator;
        b__.Cut += a__.Cut;
        b__.Paste += a__.Paste;
        b__.InsertRow += a__.InsertRow;
        b__.InsertMatrixColumn += a__.InsertColumn;
        b__.DeleteRow += a__.DeleteRow;
        b__.DeleteMatrixColumn += a__.DeleteColumn;
        b__.DeleteMatrixContents += a__.DeleteContents;
        b__.ClearCells += a__.ClearCells;
        b__.SplitLinearNaryLine += a__.SplitLinearNaryLine;
        b__.InsertUnitMonomial += a__.InsertUnitMonomial;
        b__.ToggleIdLabel += a__.ToggleIdLabel;
        b__.ToggleUnitIdLabel += a__.ToggleUnitIdLabel;
        b__.SetIdLabel += a__.SetIdLabel;
        b__.UnsetIdLabel += a__.UnsetIdLabel;
        b__.CanInsertKeyword -= b_.a ?? (b_.a = new Func<bool>(v));
        b__.CanPerformReplace -= b_.b ?? (b_.b = new DelegateFunction1<bool, FindReplaceQuery>(a));
        b__.CanCut -= b_.c ?? (b_.c = new DelegateRef1<bool>(a));
        b__.CanPaste -= b_.c ?? (b_.c = new DelegateRef1<bool>(a));
        b__.CanPerformColumnOperations -= b_.c ?? (b_.c = new DelegateRef1<bool>(a));
        b__.CanPerformClearCellsOperation -= b_.c ?? (b_.c = new DelegateRef1<bool>(a));
        b__.CanPerformLinearNaryOperations -= b_.c ?? (b_.c = new DelegateRef1<bool>(a));
        b__.CanPerformRowOperations -= b_.c ?? (b_.c = new DelegateRef1<bool>(a));
        b__.CanPerformIdLabelOperations -= b_.c ?? (b_.c = new DelegateRef1<bool>(a));
        b__.CanInsertKeyword += a__.CanInsertKeyword;
        b__.CanPerformReplace += a__.CanPerformReplace;
        b__.CanCut += a__.CanCut;
        b__.CanPaste += a__.CanPaste;
        b__.CanPerformColumnOperations += a__.CanPerformColumnOperations;
        b__.CanPerformClearCellsOperation += a__.CanPerformClearCellsOperation;
        b__.CanPerformLinearNaryOperations += a__.CanPerformLinearNaryOperations;
        b__.CanPerformRowOperations += a__.CanPerformRowOperations;
        b__.CanPerformIdLabelOperations += a__.CanPerformIdLabelOperations;
    }

    [System.Runtime.CompilerServices.SkipLocalsInit]
    private void w()
    {
        b__.DeleteLeft -= i;
        b__.DeleteRight -= h;
        b__.KeyGesture -= a;
        b__.InsertEvaluation -= a__.ProcessEqualsKey;
        b__.InsertSymbolicEvaluation -= a__.InsertSymbolicEvaluation;
        b__.InsertOperator -= a__.InsertOperator;
        b__.SwitchLineBreakOperator -= a__.SwitchLineBreakOperator;
        b__.InsertString -= a__.InsertString;
        b__.InsertInputControl -= a__.InsertInputControl;
        b__.CanInsertInputControl -= a__.CanInsertInputControl;
        b__.InsertMatrix -= a__.InsertMatrix;
        b__.InsertFunction -= a__.InsertFunction;
        b__.CanInsertFunction -= a__.CanInsertFunction;
        b__.InsertFunctionWithArgs -= a__.InsertFunctionWithArgs;
        b__.InsertId -= a__.InsertId;
        b__.InsertKeyword -= a__.InsertKeyword;
        b__.InsertModifier -= a__.InsertModifier;
        b__.Replace -= a__.Replace;
        b__.Greek -= a__.ToggleGreek;
        b__.ToggleSubscript -= a__.ToggleSubscript;
        b__.ExitGroupRight -= a__.CloseParen;
        b__.ConvertToOperator -= a__.ConvertToOperator;
        b__.Cut -= a__.Cut;
        b__.Paste -= a__.Paste;
        b__.InsertRow -= a__.InsertRow;
        b__.InsertMatrixColumn -= a__.InsertColumn;
        b__.DeleteRow -= a__.DeleteRow;
        b__.DeleteMatrixColumn -= a__.DeleteColumn;
        b__.DeleteMatrixContents -= a__.DeleteContents;
        b__.ClearCells -= a__.ClearCells;
        b__.SplitLinearNaryLine -= a__.SplitLinearNaryLine;
        b__.InsertUnitMonomial -= a__.InsertUnitMonomial;
        b__.ToggleIdLabel -= a__.ToggleIdLabel;
        b__.ToggleUnitIdLabel -= a__.ToggleUnitIdLabel;
        b__.SetIdLabel -= a__.SetIdLabel;
        b__.UnsetIdLabel -= a__.UnsetIdLabel;
        b__.CanInsertKeyword -= a__.CanInsertKeyword;
        b__.CanPerformReplace -= a__.CanPerformReplace;
        b__.CanCut -= a__.CanCut;
        b__.CanPaste -= a__.CanPaste;
        b__.CanPerformColumnOperations -= a__.CanPerformColumnOperations;
        b__.CanPerformClearCellsOperation -= a__.CanPerformClearCellsOperation;
        b__.CanPerformLinearNaryOperations -= a__.CanPerformLinearNaryOperations;
        b__.CanPerformRowOperations -= a__.CanPerformRowOperations;
        b__.CanPerformIdLabelOperations -= a__.CanPerformIdLabelOperations;
        b__.CanInsertKeyword += b_.a ?? (b_.a = new Func<bool>(v));
        b__.CanPerformReplace += b_.b ?? (b_.b = new DelegateFunction1<bool, FindReplaceQuery>(a));
        b__.CanCut += b_.c ?? (b_.c = new DelegateRef1<bool>(a));
        b__.CanPaste += b_.c ?? (b_.c = new DelegateRef1<bool>(a));
        b__.CanPerformColumnOperations += b_.c ?? (b_.c = new DelegateRef1<bool>(a));
        b__.CanPerformClearCellsOperation += b_.c ?? (b_.c = new DelegateRef1<bool>(a));
        b__.CanPerformLinearNaryOperations += b_.c ?? (b_.c = new DelegateRef1<bool>(a));
        b__.CanPerformRowOperations += b_.c ?? (b_.c = new DelegateRef1<bool>(a));
        b__.CanPerformIdLabelOperations += b_.c ?? (b_.c = new DelegateRef1<bool>(a));
    }

    private static void a(ref bool A_0)
    {
        A_0 = false;
    }

    [MirrorVerified("Release IL")]
    private static bool v() => false;

    [MirrorVerified("Release IL")]
    private static bool a(FindReplaceQuery findReplaceQuery) => false;

    [System.Runtime.CompilerServices.SkipLocalsInit]
    private void u()
    {
        b__.Disposed += o;
        b__.Loaded += l;
        b__.Activated += k;
        b__.Deactivated += j;
        b__.NodeClick += b;
        b__.NodeDoubleClick += a;
        b__.MouseDragOver += a;
        b__.LabeledIdFormatChanged += d;
        b__.MathFormatChanged += a;
        b__.SetIsReplacingPreviouseOperator += a__.SetIsReplacingPreviouseOperator;
        b__.SetCurrentOperatorMayBeReplaced += a__.SetCurrentOperatorMayBeReplaced;
        b__.MoveLeft += a__.MoveLeft;
        b__.MoveRight += a__.MoveRight;
        b__.MoveUp += a__.MoveUp;
        b__.MoveDown += a__.MoveDown;
        b__.MoveLeftByTerm += a__.MoveLeftByTerm;
        b__.MoveRightByTerm += a__.MoveRightByTerm;
        b__.ScopeLeft += a__.ScopeLeft;
        b__.ScopeRight += a__.ScopeRight;
        b__.ExpandScope += a__.ExpandScope;
        b__.ContractScope += a__.ContractScope;
        b__.FullyExpandScope += a__.FullyExpandScope;
        b__.FullyContractScope += a__.FullyContractScope;
        b__.MoveToRegionStart += a__.MoveToRegionStart;
        b__.MoveToRegionEnd += a__.MoveToRegionEnd;
        b__.MoveToStart += a__.MoveToCurrentExpressionStart;
        b__.MoveToEnd += a__.MoveToCurrentExpressionEnd;
        b__.MoveToOpposite += a__.MoveToOppositeSide;
        b__.Copy += a__.Copy;
        b__.CanCopy += a__.CanCopy;
        b__.OnCanTraverseForward += a__.OnCanTraverseForward;
        b__.OnCanTraverseBackward += a__.OnCanTraverseBackward;
        b__.OnTraverseBackward += a__.OnTraverseBackward;
        b__.CanNavigateToOppositeOfScope += a__.CanNavigateToOppositeOfScope;
        b__.DragIntoMatrixHiddenPart += a;
        b__.InputTagChanged += a__.HandleInputTagChanged;
        b__.WarningDisplayFilterChanged += q;
        b__.OnTraverseForward += a__.OnTraverseForward;
        if (!a__.IsRestrictedReadOnly)
        {
            x();
        }
    }

    [MirrorVerified("Release IL")]
    private void t() { }

    private void s()
    {
        if (a__.InCustomInitialization)
        {
            return;
        }
        switch (i_)
        {
            case a_.b:
                m();
                break;
            case a_.c:
                b();
                break;
            case a_.d:
                n();
                f();
                break;
            default:
                DebugTools.FailAssertion(null, Array.Empty<object>());
                break;
            case a_.a:
                break;
        }
        i_ = a_.a;
    }

    private void a(IVisualMatrixNode A_0, int A_1, int A_2)
    {
        MathNode node = c__.AssociateNode(A_0);
        if (node != null)
        {
            a__.DragIntoMatrixHiddenPart(node, A_1, A_2);
        }
    }

    [System.Runtime.CompilerServices.SkipLocalsInit]
    private void r()
    {
        a__.InCustomInitializationChanged += s;
        a__.EquationChanged += a;
        a__.ExpressionValueChanged += g;
        a__.EvaluationErrorOccurred += f;
        a__.WarningsOccurred += p;
        a__.NavigatorStateChanged += a;
        a__.ScopeChanged += a;
        a__.TypeoverChanged += a;
        a__.NodeMayBeReplaced += a;
        a__.RestrictedReadonlyChanged += b;
        a__.RequestRestoreInsertionPoint += a;
        a__.AlgorithmChanged += a;
    }

    [MirrorVerified("Release IL")]
    private void q() => p();

    [System.Runtime.CompilerServices.SkipLocalsInit]
    private void p()
    {
        if (a__.InCustomInitialization)
        {
            return;
        }
        if (b__.WarningDisplayFilter.ShouldDisplayAllWarnings())
        {
            b__.SetWarnings(a__.Warnings?.Select(b).ToList());
        }
        else if (b__.WarningDisplayFilter.IsAnyWarningActive())
        {
            b__.SetWarnings(a__.Warnings?.Select(a).Where(a).ToList());
        }
        else
        {
            b__.RemoveWarnings();
        }
    }

    [System.Runtime.CompilerServices.SkipLocalsInit]
    private void o()
    {
        a__.InCustomInitializationChanged -= s;
        a__.EquationChanged -= a;
        a__.ExpressionValueChanged -= g;
        a__.EvaluationErrorOccurred -= f;
        a__.NavigatorStateChanged -= a;
        a__.ScopeChanged -= a;
        a__.TypeoverChanged -= a;
        a__.NodeMayBeReplaced -= a;
        a__.RestrictedReadonlyChanged -= b;
        a__.RequestRestoreInsertionPoint -= a;
        a__.AlgorithmChanged -= a;
    }

    private void a(object A_0, RequestRestoreInsertionPointEventArgs A_1)
    {
        b__.FireRequestRestoreInsertionPoint(A_1);
    }

    private void b(bool A_0)
    {
        if (A_0)
        {
            w();
        }
        else
        {
            x();
        }
    }

    [System.Runtime.CompilerServices.SkipLocalsInit]
    private void n()
    {
        b__.ClearEditors();
        m();
        if (a__.EvaluationRoot != null && a__.IsExpressionValueModified)
        {
            MathNode.Utilities.GetEvaluationNodes(a__.EquationRoot).Each(a);
        }
    }

    [MirrorStub]
    [MirrorBlocked("calls the FrameworkElementFacade getter, which Mathcad names y(); the accessor name cannot be mapped")]
    private void m() => throw MirrorStub.NotImplemented();

    private void l()
    {
        f();
        p();
    }

    [MirrorStub]
    [MirrorBlocked("calls the FrameworkElementFacade getter, which Mathcad names y(); the accessor name cannot be mapped")]
    private void k() => throw MirrorStub.NotImplemented();

    private void j()
    {
        b__.ClearEditors();
        f_ = false;
        EngineError calculationError = a__.CalculationError;
        if (calculationError != null && calculationError.IsRuntime)
        {
            a__.HandleError(null);
        }
    }

    private void a(char A_0)
    {
        a__.InsertValue(A_0.ToString());
    }

    private void i()
    {
        a__.Delete(Direction.Left);
    }

    private void h()
    {
        a__.Delete(Direction.Right);
    }

    private void b(IVisualMathNode A_0, Direction A_1, Direction A_2, int? A_3, double A_4, double A_5)
    {
        NavigatorState state = e_.a(c__, A_0, A_1, A_2, A_3, A_4, A_5);
        if (state.Equals(NavigatorState.Empty))
        {
            return;
        }
        a__.NavigatorState = state;
        if (!b__.IsLogicalFocusWithin || !(A_0 is IVisualInputControlNode))
        {
            b__.Focus();
        }
    }

    private void a(IVisualMathNode A_0, Direction A_1, Direction A_2, int? A_3, double A_4, double A_5)
    {
        b(A_0, A_1, A_2, A_3, A_4, A_5);
        a__.ExpandScope();
    }

    private void a(IVisualMathNode A_0, Direction A_1, Direction A_2)
    {
        MathNode node = c__.AssociateNode(A_0);
        if (node != null)
        {
            a__.SelectTo(node, A_2, A_1);
        }
    }

    [MirrorStub]
    [MirrorBlocked("calls interface c.c(...); C# forbids a member named like its enclosing type, so interface c cannot declare it")]
    private void a(ScopeStateChangeEventArgs A_0) => throw MirrorStub.NotImplemented();

    private void a(NavigatorStateChangeEventArgs A_0)
    {
        if (!a__.CurrentOperatorMayBeReplaced)
        {
            b__.ClearKeyStrokesCache();
        }
        b__.ClearEditors();
        b__.UseDefaultGestures();
        if (A_0.NavigatorState.Current != null)
        {
            switch (A_0.NavigatorState.Current.DescriptorName)
            {
                case "STRING":
                    if (A_0.NavigatorState.CaretPosition.HasValue)
                    {
                        b__.DisableGestures();
                    }
                    break;
                case "LIMIT_SIDE":
                    b__.UseGesturesForLimitSideNode();
                    break;
                case "ScriptableControl":
                    f_ = true;
                    break;
            }
        }
        if (f_ || b__.IsLogicalFocusWithin)
        {
            if (!f_)
            {
                b__.EnsureFocus();
            }
            d_.b(c__, A_0.NavigatorState);
        }
    }

    private void a(TypeoverChangeEventArgs A_0)
    {
        b__.ClearEditors();
        if (f_ || b__.IsLogicalFocusWithin)
        {
            d_.d(c__, A_0);
        }
    }

    [System.Runtime.CompilerServices.SkipLocalsInit]
    private void a(bool A_0)
    {
        if (a__.EquationRoot == null)
        {
            throw new InvalidOperationException("Logic error: model's EquationRoot should not be null when the EquationChanged event is fired.");
        }
        if (a__.InCustomInitialization)
        {
            a(a_.d);
            return;
        }
        if (A_0)
        {
            n();
        }
        else
        {
            b__.RefreshView();
        }
        f();
    }

    private a_ a(a_ A_0)
    {
        if (i_ < A_0)
        {
            i_ = A_0;
        }
        return i_;
    }

    private void b(MathNode A_0)
    {
        MathNode modelNode;
        switch (A_0.DescriptorName)
        {
            case "ScriptableControl":
                modelNode = A_0[0];
                break;
            case "EVALUATION":
                modelNode = A_0[1][0];
                break;
            case "SYMBOLIC_EVALUATION":
                modelNode = A_0[2];
                break;
            default:
                throw new InvalidOperationException(string.Format("Logic error: trying to handle an unsupported evaluation {0}.", A_0.DescriptorName));
        }
        c__.AssociateNode(modelNode).IsValid = false;
    }

    private void g()
    {
        b__.RemoveError();
        b__.RemoveWarnings();
    }

    [System.Runtime.CompilerServices.SkipLocalsInit]
    private void f()
    {
        if (e())
        {
            return;
        }
        if (a__.InCustomInitialization)
        {
            return;
        }
        if (a__.IsScriptableControl && a__.IsScriptError)
        {
            b__.SetError(a__.ScriptingError, c__.AssociateNode(a__.ErrorNode ?? ((a__.EquationRoot.ChildCount > 1) ? a__.EquationRoot[a__.EquationRoot.ChildCount - 1] : a__.EquationRoot)));
        }
        else
        {
            b__.SetError(a__.CalculationError, c__.AssociateNode(a__.ErrorNode));
        }
    }

    private bool e()
    {
        c_ closure = new c_();
        closure.a_ = this;
        if (a__.ErrorNode == null)
        {
            return false;
        }
        closure.b = a__.ErrorNode.GetNearestAncestor("MATRIX") ?? a__.ErrorNode.GetNearestAncestor("ScriptableControl");
        if (closure.b == null)
        {
            return false;
        }
        MatrixNodeInfo matrixInfo = (MatrixNodeInfo)closure.b.NodeInfo;
        if (matrixInfo.IsInResult)
        {
            return false;
        }
        while (closure.b.GetNearestAncestor("MATRIX") != null)
        {
            closure.b = closure.b.GetNearestAncestor("MATRIX");
        }
        // The locals are declared up front to keep Mathcad's local slot order.
        bool hidden = false;
        Action action;
        int cellIndex;
        IList<int> path;
        int depth;
        closure.c = 0;
        closure.d = 0;
        do
        {
            matrixInfo = (MatrixNodeInfo)closure.b.NodeInfo;
            cellIndex = a__.ErrorNode.GetMatrixCellIndex(closure.b);
            closure.c = MatrixHelper.RowOf(cellIndex, matrixInfo.FullSize.Columns);
            closure.d = MatrixHelper.ColumnOf(cellIndex, matrixInfo.FullSize.Columns);
            hidden = closure.c < matrixInfo.ViewportOffset.Row || closure.d < matrixInfo.ViewportOffset.Column || closure.c >= matrixInfo.ViewportOffset.Row + matrixInfo.ViewportSize.Rows || closure.d >= matrixInfo.ViewportOffset.Column + matrixInfo.ViewportSize.Columns;
            if (hidden)
            {
                break;
            }
            path = a__.ErrorNode.PathFromAncestor(closure.b);
            for (depth = 0; depth < path.Count - 1; depth++)
            {
                closure.b = closure.b[path[depth]];
                if (closure.b.DescriptorName == "MATRIX")
                {
                    break;
                }
            }
        }
        while (depth != path.Count - 1);
        if (!hidden)
        {
            return false;
        }
        action = closure.a;
        WpfUtils.ExecuteOnLoad(b__.RootNode as FrameworkElement, action);
        return true;
    }

    private void a(NavigatorState A_0)
    {
        d_.b(c__, A_0);
    }

    [MirrorVerified("Release IL")]
    private void d() => c();

    [MirrorVerified("Release IL")]
    private void a(DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs) => c();

    [System.Runtime.CompilerServices.SkipLocalsInit]
    private void c()
    {
        if (a__.InCustomInitialization)
        {
            DebugTools.DevAssert(!f_, null);
            a(a_.c);
        }
        else if (g_ == null || g_.Status == DispatcherOperationStatus.Completed)
        {
            WasSuspendedRegenerate = true;
            g_ = Dispatcher.CurrentDispatcher.BeginInvoke(new Action(b), DispatcherPriority.DataBind);
            g_.Completed += a;
        }
        else if (g_.Status == DispatcherOperationStatus.Executing)
        {
            DebugTools.DevAssert(false, null);
        }
    }

    private void a(object A_0, EventArgs A_1)
    {
        Action action = a;
        Dispatcher.CurrentDispatcher.BeginInvoke(action, DispatcherPriority.Background);
    }

    [MirrorStub]
    [MirrorBlocked("calls interface c.c(...); C# forbids a member named like its enclosing type, so interface c cannot declare it")]
    private void b() => throw MirrorStub.NotImplemented();

    private (RedefinitionWarning parsedWarning, IVisualMathNode) b((RedefinitionWarning parsedWarning, MathNode warningNode) A_0)
    {
        return (parsedWarning: A_0.parsedWarning, c__.AssociateNode(A_0.warningNode));
    }

    private (RedefinitionWarning parsedWarning, IVisualMathNode) a((RedefinitionWarning parsedWarning, MathNode warningNode) A_0)
    {
        return (parsedWarning: A_0.parsedWarning, c__.AssociateNode(A_0.warningNode));
    }

    private bool a((RedefinitionWarning parsedWarning, IVisualMathNode) A_0)
    {
        return b__.WarningDisplayFilter.HasFlag(A_0.parsedWarning.WarningType);
    }

    [MirrorVerified("Release IL")]
    private void a(MathNode mathNode) => b(mathNode);

    private void a()
    {
        f();
        p();
    }

    partial class c_
    {
        [MirrorVerified("Release IL")]
        public c_() { }

        internal void a()
        {
            IVisualMatrixNode matrix = a_.c__.AssociateNode(b) as IVisualMatrixNode;
            matrix.ErrorCell = new TableCoordinate(c, d);
            IVisualMathNode errorNode = matrix.ErrorNode;
            a_.b__.SetError(a_.a__.CalculationError, errorNode);
        }
    }
}
