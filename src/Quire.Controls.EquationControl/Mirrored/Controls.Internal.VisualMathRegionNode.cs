using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Quire.Controls.Core;
using Quire.EquationEditor.View;
using System.Linq;
using Quire.Wpf;
using Quire.Controls.InputControls;
using Quire.Matrices;

namespace Quire.Controls.Internal;

internal partial class VisualMathRegionNode
{
    private const string SExpressionComplexIndicator = "@";
    private bool _isLogicalFocusWithinScriptableControl;
    private bool _isSymbolicEvalKeywordsShown;
    private Quire.Controls.Internal.GestureManager _activeGestureManager;
    private Quire.Controls.Internal.GestureManager _defaultGestureManager;
    private Quire.Controls.Internal.GestureManager _emptyGestureManager;
    private Quire.Controls.Internal.GestureManager _limitSideGestureManager;
    private Quire.EquationEditor.View.IVisualFactory _visualFactory;
    private Quire.Controls.Core.NodeMessageAdorner _message;
    private Quire.Controls.Internal.RubberBandHighlighter _highlight;
    private Quire.Controls.Internal.HistoryPopup _historyPopup;
    private System.Windows.Point _mouseDownPosition;
    private const double DragHysteresis = 5d;
    private bool _isDragging;
    private string _previouseKeys;
    private Quire.Delegate0 _loadedEvent;
    private readonly Quire.DelegateFunction1<string, Quire.Calculation.Core.EngineError> GetErrorDisplayText;
    private System.Collections.Generic.IDictionary<Quire.Controls.Core.IdLabels, Quire.Controls.Core.ExtendedFont> _previousLabeledIdFormat;
    private System.Collections.Generic.IDictionary<Quire.Controls.Core.IdLabels, int> _previousLabeledIdFormatRevisionNumber;
    private Quire.Controls.Core.NodeMessageAdorner _visibleWarningMessage;
    private System.Windows.Visibility _lhsOfDefinitionVisibility;

    public event Quire.Delegate1<char> KeyGesture;
    public event Quire.EquationEditor.View.NodeClickHandler NodeClick;
    public event Quire.EquationEditor.View.NodeDoubleClickHandler NodeDoubleClick;
    public event Quire.Delegate3<Quire.EquationEditor.View.IVisualMathNode, Quire.Direction, Quire.Direction> MouseDragOver;
    public event Quire.Delegate0 Activated;
    public event Quire.Delegate0 Deactivated;
    public event Quire.Delegate0 MoveLeft;
    public event Quire.Delegate0 MoveRight;
    public event Quire.Delegate0 MoveUp;
    public event Quire.Delegate0 MoveDown;
    public event Quire.Delegate0 MoveLeftByTerm;
    public event Quire.Delegate0 MoveRightByTerm;
    public event Quire.Delegate0 MoveToRegionStart;
    public event Quire.Delegate0 MoveToRegionEnd;
    public event Quire.Delegate0 Pi;
    public event Quire.Delegate0 ScopeLeft;
    public event Quire.Delegate0 ScopeRight;
    public event Quire.Delegate0 FullyExpandScope;
    public event Quire.Delegate0 FullyContractScope;
    public event Quire.Delegate0 ExpandScope;
    public event Quire.Delegate0 ContractScope;
    public event Quire.Delegate0 DeleteLeft;
    public event Quire.Delegate0 DeleteRight;
    public event Quire.Delegate0 MoveToStart;
    public event Quire.Delegate0 MoveToEnd;
    public event Quire.Delegate0 MoveToOpposite;
    public event Quire.Delegate0 Greek;
    public event System.Action ToggleSubscript;
    public event Quire.Delegate0 ExitGroupLeft;
    public event Quire.Delegate0 ExitGroupRight;
    public event Quire.Delegate0 ConvertToOperator;
    public event Quire.Delegate0 InsertEvaluation;
    public event System.Action InsertSymbolicEvaluation;
    public event Quire.DelegateRef1<bool> OnCanTraverseForward;
    public event Quire.DelegateRef1<bool> OnCanTraverseBackward;
    public event Quire.DelegateRef1<bool> OnTraverseForward;
    public event Quire.DelegateRef1<bool> OnTraverseBackward;
    public event Quire.Delegate0 Copy;
    public event Quire.DelegateRef1<bool> CanCopy;
    public event Quire.DelegateRef1<bool> CanPaste;
    public event Quire.DelegateFunction0<bool> Paste;
    public event Quire.Delegate0 Cut;
    public event Quire.DelegateRef1<bool> CanCut;
    public event Quire.Delegate0 RequestShowRegionHistory;
    public event Quire.Delegate0 RequestChangeToTextBox;
    public event Quire.DelegateRef1<bool> CanShowRegionHistory;
    public event Quire.Delegate1<int> RestoreFromHistory;
    public event Quire.Delegate1<string> InsertOperator;
    public event System.Action SwitchLineBreakOperator;
    public event System.Action WarningDisplayFilterChanged;
    public event Quire.EquationEditor.View.CanExecuteWithToggle CanToggleLineBreakOperator;
    public event Quire.DelegateFunction1<bool, bool> SetIsReplacingPreviouseOperator;
    public event Quire.Delegate1<bool> SetCurrentOperatorMayBeReplaced;
    public event Quire.Delegate1<string> InsertString;
    public event System.Action<Quire.Controls.Core.InputControlTypes> InsertInputControl;
    public event Quire.DelegateRef1Var1<bool, System.Nullable<Quire.Controls.Core.InputControlTypes>> CanInsertInputControl;
    public event Quire.DelegateRef1<bool> CanInsertFunction;
    public event Quire.DelegateFunction1<bool, Quire.Controls.Core.FindReplaceQuery> CanPerformReplace;
    public event Quire.Delegate1<Quire.Controls.Core.FindReplaceQuery> Replace;
    public event Quire.Delegate1<System.Nullable<Quire.TableSize>> InsertMatrix;
    public event Quire.Delegate3<Quire.EquationEditor.Tree.FormattedString, int, System.Nullable<Quire.Controls.Core.IdLabels>> InsertFunction;
    public event System.Action<Quire.EquationEditor.Tree.FormattedString, int, System.Nullable<Quire.Controls.Core.IdLabels>, string[]> InsertFunctionWithArgs;
    public event Quire.Delegate2<Quire.EquationEditor.Tree.FormattedString, System.Nullable<Quire.Controls.Core.IdLabels>> InsertId;
    public event System.Action<string> InsertKeyword;
    public event System.Action<string> InsertModifier;
    public event System.Func<bool> CanInsertKeyword;
    public event System.Action<Quire.EquationEditor.View.IVisualMathRegionNode, System.Windows.DependencyObject, System.Windows.DependencyObject> VisualParentChanged;
    public event System.Action<Quire.EquationEditor.View.IVisualMatrixNode, int, int> DragIntoMatrixHiddenPart;
    public event Quire.Delegate1<Quire.Direction> InsertRow;
    public event Quire.Delegate1<Quire.Direction> InsertMatrixColumn;
    public event Quire.Delegate0 DeleteRow;
    public event Quire.Delegate0 DeleteMatrixColumn;
    public event Quire.Delegate1<Quire.EquationEditor.Tree.MathNode> DeleteMatrixContents;
    public event Quire.Delegate0 ClearCells;
    public event Quire.DelegateRef1<bool> CanPerformColumnOperations;
    public event Quire.DelegateRef1<bool> CanPerformClearCellsOperation;
    public event Quire.DelegateRef1<bool> CanNavigateToOppositeOfScope;
    public event Quire.Delegate0 SplitLinearNaryLine;
    public event Quire.DelegateRef1<bool> CanPerformLinearNaryOperations;
    public event Quire.DelegateRef1<bool> CanPerformRowOperations;
    public event Quire.Delegate1<string> InsertValue;
    public event Quire.Delegate1<System.Windows.Vector> DropRegion;
    public event Quire.Delegate0 LabeledIdFormatChanged;
    public event System.Action<System.Windows.DependencyPropertyChangedEventArgs> MathFormatChanged;
    public event System.Action RootNodeChanged;
    public event Quire.Delegate0 Evaluate;
    public event System.Action<bool> RequestToAllowContextMenu;
    public event System.Action VisibleWarningMessageChanged;
    public event System.Action<bool> InputTagChanged;
    public event System.Action<System.Collections.Generic.IEnumerable<Quire.Calculation.Core.Unit>> InsertUnitMonomial;
    public event Quire.DelegateRef1<bool> CanPerformIdLabelOperations;
    public event System.Action ToggleUnitIdLabel;
    public event System.Action ToggleIdLabel;
    public event System.Action UnsetIdLabel;
    public event System.Action<Quire.Controls.Core.IdLabels> SetIdLabel;
    public event System.Action Disposed;

    public Visibility LhsOfDefinitionVisibility
    {
        get => _lhsOfDefinitionVisibility;
        set
        {
            _lhsOfDefinitionVisibility = value;
            if (TryFindDefinitionAndUpdateIsLhsVisible())
            {
                InvalidateVisual();
            }
        }
    }

    public bool SymbolicEvalKeywordsVisibilityState
    {
        get => _isSymbolicEvalKeywordsShown;
        set => _isSymbolicEvalKeywordsShown = value;
    }

    private bool TryFindDefinitionAndUpdateIsLhsVisible()
    {
        VisualDefinitionNode definitionNode = GetDefinitionNode();
        if (definitionNode == null)
        {
            return false;
        }

        definitionNode.UpdateVisibility(_lhsOfDefinitionVisibility);
        return true;
    }

    public IVisualFactory VisualFactory
    {
        get => _visualFactory;
        set => _visualFactory = value;
    }

    private Thickness RootPadding => (Thickness)GetValue(RootPaddingProperty);

    public IMathFormat RootMathFormat => new StaticMathFormat(MathFormat);

    public IVisualMathNode RootNode
    {
        get => (IVisualMathNode)GetValue(RootNodeProperty);
        set => SetValue(RootNodeProperty, value);
    }

    public override double EffectiveBaselineY => 0.0;

    public override double EffectiveScriptBaselineY => 0.0;

    public bool IsLogicalFocusWithin { get; set; }

    public bool IsLogicalFocusWithinScriptableControl => _isLogicalFocusWithinScriptableControl;

    public NodeMessageAdorner VisibleWarningMessage
    {
        get => _visibleWarningMessage;
        private set
        {
            _visibleWarningMessage = value;
            VisibleWarningMessageChanged?.Invoke();
        }
    }

    public Point Location
    {
        get => (Point)GetValue(LocationProperty);
        set => SetValue(LocationProperty, value);
    }

    internal RubberBandHighlighter ErrorHighlighter
    {
        get => _highlight;
        set
        {
            if (_highlight != null)
            {
                _highlight.Remove();
            }

            _highlight = value;
        }
    }

    internal NodeMessageAdorner ErrorMessage
    {
        get => _message;
        set
        {
            if (_message != null)
            {
                _message.Remove();
            }

            _message = value;
        }
    }

    public WarningsTypesToShow WarningDisplayFilter => AttachedProperties.GetWarningDisplayFilter(this);

    internal IList<ValueTuple<RubberBandHighlighter, NodeMessageAdorner>> WarningTuples { get; set; }

    public IEnumerable<NodeMessageAdorner> NodeMessageAdorners
    {
        get
        {
            List<NodeMessageAdorner> adorners = new List<NodeMessageAdorner>();
            if (ErrorMessage != null)
            {
                adorners.Add(ErrorMessage);
            }

            return adorners;
        }
    }

    public bool Activating { get; set; }

    private void FireLoaded(object sender, RoutedEventArgs e)
    {
        if (_loadedEvent != null)
        {
            _loadedEvent.Invoke();
        }
    }

    internal void FireNodeDoubleClick(IVisualMathNode node, Direction caretSide, Direction nodeSide, int? operatorPartIndex, Point hitPoint)
    {
        if (NodeDoubleClick != null)
        {
            NodeDoubleClick.Invoke(node, caretSide, nodeSide, operatorPartIndex, hitPoint.X, hitPoint.Y);
        }
    }

    internal void FireActivated(bool setLogicalFocus)
    {
        if (setLogicalFocus)
        {
            IsLogicalFocusWithin = true;
        }

        if (Activated != null)
        {
            Activated.Invoke();
        }
    }

    internal void FireDeactivated()
    {
        if (Deactivated != null)
        {
            Deactivated.Invoke();
        }
    }

    internal void FireKeypress(char c)
    {
        if (KeyGesture != null)
        {
            KeyGesture.Invoke(c);
        }
    }

    internal void FireNodeClick(IVisualMathNode node, Direction caretSide, Direction nodeSide, int? operatorPartIndex, Point hitPoint)
    {
        if (NodeClick != null)
        {
            NodeClick.Invoke(node, caretSide, nodeSide, operatorPartIndex, hitPoint.X, hitPoint.Y);
        }
    }

    internal void FireMoveLeft()
    {
        if (MoveLeft != null)
        {
            MoveLeft.Invoke();
        }
    }

    internal void FireMoveRight()
    {
        if (MoveRight != null)
        {
            MoveRight.Invoke();
        }
    }

    internal void FireMoveUp()
    {
        if (MoveUp != null)
        {
            MoveUp.Invoke();
        }
    }

    internal void FireMoveDown()
    {
        if (MoveDown != null)
        {
            MoveDown.Invoke();
        }
    }

    internal void FireMoveRightByTerm()
    {
        if (MoveRightByTerm != null)
        {
            MoveRightByTerm.Invoke();
        }
    }

    internal void FireEvaluate()
    {
        if (Evaluate != null)
        {
            Evaluate.Invoke();
        }
    }

    internal void FireMoveLeftByTerm()
    {
        if (MoveLeftByTerm != null)
        {
            MoveLeftByTerm.Invoke();
        }
    }

    internal void FireMoveToRegionStart()
    {
        if (MoveToRegionStart != null)
        {
            MoveToRegionStart.Invoke();
        }
    }

    internal void FireMoveToRegionEnd()
    {
        if (MoveToRegionEnd != null)
        {
            MoveToRegionEnd.Invoke();
        }
    }

    internal void FireMoveToStart()
    {
        if (MoveToStart != null)
        {
            MoveToStart.Invoke();
        }
    }

    internal void FireMoveToEnd()
    {
        if (MoveToEnd != null)
        {
            MoveToEnd.Invoke();
        }
    }

    internal void FireMoveToOpposite()
    {
        if (MoveToOpposite != null)
        {
            MoveToOpposite.Invoke();
        }
    }

    internal void FireFullyContractScope()
    {
        if (FullyContractScope != null)
        {
            FullyContractScope.Invoke();
        }
    }

    internal void OnPiCommand()
    {
        FireKeypress('π');
    }

    internal void OnInfinityCommand()
    {
        FireKeypress('∞');
    }

    internal void FireScopeLeft()
    {
        if (ScopeLeft != null)
        {
            ScopeLeft.Invoke();
        }
    }

    internal void FireScopeRight()
    {
        if (ScopeRight != null)
        {
            ScopeRight.Invoke();
        }
    }

    internal void FireFullyExpandScope()
    {
        if (FullyExpandScope != null)
        {
            FullyExpandScope.Invoke();
        }
    }

    internal void FireExpandScope()
    {
        if (ExpandScope != null)
        {
            ExpandScope.Invoke();
        }
    }

    internal void FireContractScope()
    {
        if (ContractScope != null)
        {
            ContractScope.Invoke();
        }
    }

    internal void FireGreek()
    {
        if (Greek != null)
        {
            Greek.Invoke();
        }
    }

    internal void FireToggleSubscript()
    {
        if (ToggleSubscript != null)
        {
            ToggleSubscript.Invoke();
        }
    }

    internal void FireExitGroupLeft()
    {
        if (ExitGroupLeft != null)
        {
            ExitGroupLeft.Invoke();
        }
    }

    internal void FireExitGroupRight()
    {
        if (ExitGroupRight != null)
        {
            ExitGroupRight.Invoke();
        }
    }

    internal void FireConvertToOperator()
    {
        if (ConvertToOperator != null)
        {
            ConvertToOperator.Invoke();
        }
    }

    internal void FireInsertEvaluation()
    {
        if (InsertEvaluation != null)
        {
            InsertEvaluation.Invoke();
        }
    }

    internal void FireInsertSymbolicEvaluation()
    {
        if (InsertSymbolicEvaluation != null)
        {
            InsertSymbolicEvaluation.Invoke();
        }
    }

    public void CloseEditingMode()
    {
        WpfUtils.ExecuteCommandIfFocusedElementIsDescendantOf(this, EditableComboBoxCommands.CloseEditingMode);
    }

    public void CloseComboBoxDropDown()
    {
        WpfUtils.ExecuteCommandIfFocusedElementIsDescendantOf(this, ComboBoxControl.CloseComboBoxDropDownCommand);
    }

    protected override void OnLostKeyboardFocus(System.Windows.Input.KeyboardFocusChangedEventArgs e)
    {
        base.OnLostKeyboardFocus(e);
        ClearKeyStrokesCache();
    }

    protected override void OnLostMouseCapture(System.Windows.Input.MouseEventArgs e)
    {
        base.OnLostMouseCapture(e);
        ClearKeyStrokesCache();
    }

    protected override void OnGotKeyboardFocus(System.Windows.Input.KeyboardFocusChangedEventArgs e)
    {
        base.OnGotKeyboardFocus(e);
        if (e.NewFocus == this)
        {
            ImeHandler.HandleImeFocus();
        }
    }

    public void FireInsertKeyword(string sExpression)
    {
        if (InsertKeyword != null)
        {
            InsertKeyword.Invoke(sExpression);
        }
    }

    public void FireInsertModifier(string sExpression)
    {
        if (InsertModifier != null)
        {
            InsertModifier.Invoke(sExpression);
        }
    }

    public void FireSwitchLineBreakOperator()
    {
        if (SwitchLineBreakOperator != null)
        {
            SwitchLineBreakOperator.Invoke();
        }
    }

    internal void FireCanNavigateToOppositeOfScope(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
    {
        bool canNavigate = false;
        if (CanNavigateToOppositeOfScope != null)
        {
            CanNavigateToOppositeOfScope.Invoke(ref canNavigate);
        }

        e.CanExecute = canNavigate;
    }

    public void FireCanInsertKeyword(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
    {
        bool canInsert = false;
        if (CanInsertKeyword != null)
        {
            canInsert = CanInsertKeyword.Invoke();
        }

        e.CanExecute = canInsert;
    }

    public void OnDragCommitElement(System.Windows.Vector offset)
    {
        if (DropRegion != null)
        {
            DropRegion.Invoke(offset);
        }
    }

    private static void OnRootNodePropertyChanged(System.Windows.DependencyObject d, System.Windows.DependencyPropertyChangedEventArgs e)
    {
        ((VisualMathRegionNode)d).OnRootNodeChanged(e);
    }

    private Quire.Controls.Internal.VisualDefinitionNode GetDefinitionNode()
    {
        return WpfUtils.GetVisualDescendantsBreadthFirstOfType<VisualDefinitionNode>(this).FirstOrDefault();
    }

    protected override void OnMouseUp(System.Windows.Input.MouseButtonEventArgs e)
    {
        if (Mouse.Captured == this)
        {
            Mouse.Capture(null);
        }

        base.OnMouseUp(e);
    }

    protected override void OnStylusDown(System.Windows.Input.StylusDownEventArgs e)
    {
        e.Handled = OnInput(e.GetPosition(this), e.TapCount, true);
    }

    private Quire.Controls.Internal.VisualMathNodeHitTestResult GetHitTestResult(System.Windows.Point position)
    {
        return GetNodeAtPoint(position) as VisualMathNodeHitTestResult;
    }

    private void HandleRequestRestoreInsertionPoint(object sender, System.Windows.RoutedEventArgs args)
    {
        if (IsLogicalFocusWithin)
        {
            args.Handled = true;
        }
    }

    public void EnsureFocus()
    {
        if (DebugTools.DevAssert(IsLogicalFocusWithin, null).Passed && !IsFocused)
        {
            base.Focus();
        }
    }

    public void FireInsertOperator(string nodeName)
    {
        if (InsertOperator != null)
        {
            InsertOperator.Invoke(nodeName);
        }
    }

    public void FireSetCurrentOperatorMayBeReplaced(bool currentOperatorMayBeReplaced)
    {
        if (SetCurrentOperatorMayBeReplaced != null)
        {
            SetCurrentOperatorMayBeReplaced.Invoke(currentOperatorMayBeReplaced);
        }
    }

    public void FireInsertMatrix(System.Nullable<Quire.TableSize> matrixSize)
    {
        if (InsertMatrix != null)
        {
            InsertMatrix.Invoke(matrixSize);
        }
    }

    public void FireInsertValue(string value)
    {
        if (InsertValue != null)
        {
            InsertValue.Invoke(value);
        }
    }

    public void FireInsertId(Quire.EquationEditor.Tree.FormattedString value, System.Nullable<Quire.Controls.Core.IdLabels> label)
    {
        if (InsertId != null)
        {
            InsertId.Invoke(value, label);
        }
    }

    public void FireInsertString(string str)
    {
        if (InsertString != null)
        {
            InsertString.Invoke(str);
        }
    }

    public void FireInsertRow(Quire.Direction direction)
    {
        if (InsertRow != null)
        {
            InsertRow.Invoke(direction);
        }
    }

    public void FireInsertMatrixColumn(Quire.Direction direction)
    {
        if (InsertMatrixColumn != null)
        {
            InsertMatrixColumn.Invoke(direction);
        }
    }

    public void FireDeleteRow()
    {
        if (DeleteRow != null)
        {
            DeleteRow.Invoke();
        }
    }

    public void FireDeleteMatrixColumn()
    {
        if (DeleteMatrixColumn != null)
        {
            DeleteMatrixColumn.Invoke();
        }
    }

    public void FireDeleteMatrixContents(Quire.EquationEditor.Tree.MathNode mathNode)
    {
        if (DeleteMatrixContents != null)
        {
            DeleteMatrixContents.Invoke(mathNode);
        }
    }

    public void FireClearCells()
    {
        if (ClearCells != null)
        {
            ClearCells.Invoke();
        }
    }

    private void FireRootNodeChanged()
    {
        if (RootNodeChanged != null)
        {
            RootNodeChanged.Invoke();
        }
    }

    internal void FireDeleteLeft()
    {
        if (DeleteLeft != null)
        {
            DeleteLeft.Invoke();
        }
    }

    internal void FireDeleteRight()
    {
        if (DeleteRight != null)
        {
            DeleteRight.Invoke();
        }
    }

    internal void FireReplace(Quire.Controls.Core.FindReplaceQuery findReplaceQuery)
    {
        if (Replace != null)
        {
            Replace.Invoke(findReplaceQuery);
        }
    }

    public void FireNavigateUp()
    {
        if (MoveUp != null)
        {
            MoveUp.Invoke();
        }
    }

    public void FireNavigateDown()
    {
        if (MoveDown != null)
        {
            MoveDown.Invoke();
        }
    }

    public void FireLabeledIdFormatChanged()
    {
        if (LabeledIdFormatChanged != null)
        {
            LabeledIdFormatChanged.Invoke();
        }
    }

    public void FireSplitLinearNaryLine()
    {
        if (SplitLinearNaryLine != null)
        {
            SplitLinearNaryLine.Invoke();
        }
    }

    public void FireInputTagChanged(bool isInput)
    {
        if (InputTagChanged != null)
        {
            InputTagChanged.Invoke(isInput);
        }
    }

    public void FireInsertUnitMonomial(System.Collections.Generic.IEnumerable<Quire.Calculation.Core.Unit> unitMonomial)
    {
        if (InsertUnitMonomial != null)
        {
            InsertUnitMonomial.Invoke(unitMonomial);
        }
    }

    public void FireToggleUnitIdLabel()
    {
        if (ToggleUnitIdLabel != null)
        {
            ToggleUnitIdLabel.Invoke();
        }
    }

    public void FireToggleIdLabel()
    {
        if (ToggleIdLabel != null)
        {
            ToggleIdLabel.Invoke();
        }
    }

    public void FireUnsetIdLabel()
    {
        if (UnsetIdLabel != null)
        {
            UnsetIdLabel.Invoke();
        }
    }

    public void FireSetIdLabel(Quire.Controls.Core.IdLabels idLabels)
    {
        if (SetIdLabel != null)
        {
            SetIdLabel.Invoke(idLabels);
        }
    }

    public void ClearEditors()
    {
        RecursiveRemoveOfEditors(this);
    }

    public void HandleLabeledIdFormatChanged(System.Windows.DependencyPropertyChangedEventArgs args)
    {
        FireLabeledIdFormatChanged();
    }

    internal void FireCanPerformColumnOperations(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
    {
        bool canExecute = false;
        if (CanPerformColumnOperations != null)
        {
            CanPerformColumnOperations.Invoke(ref canExecute);
        }

        e.CanExecute = canExecute;
    }

    internal void FireCanPerformCearCellsOperation(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
    {
        bool canExecute = false;
        if (CanPerformClearCellsOperation != null)
        {
            CanPerformClearCellsOperation.Invoke(ref canExecute);
        }

        e.CanExecute = canExecute;
    }

    public void FireCanCopy(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
    {
        bool canExecute = false;
        if (CanCopy != null)
        {
            CanCopy.Invoke(ref canExecute);
        }

        e.CanExecute = canExecute;
    }

    public void FireCanCut(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
    {
        bool canExecute = false;
        if (CanCut != null)
        {
            CanCut.Invoke(ref canExecute);
        }

        e.CanExecute = canExecute;
    }

    public void FireCanShowRegionHistory(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
    {
        bool canExecute = false;
        if (CanShowRegionHistory != null)
        {
            CanShowRegionHistory.Invoke(ref canExecute);
        }

        e.CanExecute = canExecute;
    }

    public void FireCanPerformLinearNaryOperations(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
    {
        bool canExecute = false;
        if (CanPerformLinearNaryOperations != null)
        {
            CanPerformLinearNaryOperations.Invoke(ref canExecute);
        }

        e.CanExecute = canExecute;
    }

    public void FireCanPerformRowOperations(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
    {
        bool canExecute = false;
        if (CanPerformRowOperations != null)
        {
            CanPerformRowOperations.Invoke(ref canExecute);
        }

        e.CanExecute = canExecute;
    }

    public void FireCanPerformIdLabelOperations(object sender, System.Windows.Input.CanExecuteRoutedEventArgs eventArgs)
    {
        bool canExecute = false;
        if (CanPerformIdLabelOperations != null)
        {
            CanPerformIdLabelOperations.Invoke(ref canExecute);
        }

        eventArgs.CanExecute = canExecute;
    }

    public void FireRequestChangeToTextBox()
    {
        RequestChangeToTextBox?.Invoke();
    }

    public void FireInsertComboBoxControl(Quire.Controls.Core.InputControlTypes inputControlType)
    {
        InsertInputControl?.Invoke(inputControlType);
    }

    public void FireInsertInputControl(Quire.Controls.Core.InputControlTypes inputControlType)
    {
        InsertInputControl?.Invoke(inputControlType);
    }

    public void ClearKeyStrokesCache()
    {
        _previouseKeys = string.Empty;
    }

    public void UseDefaultGestures()
    {
        _activeGestureManager = _defaultGestureManager;
    }

    public void DisableGestures()
    {
        _activeGestureManager = _emptyGestureManager;
    }

    public void UseGesturesForLimitSideNode()
    {
        _activeGestureManager = _limitSideGestureManager;
    }

    public void RemoveError()
    {
        ErrorMessage = null;
        ErrorHighlighter = null;
    }

    public void GetContent(System.IO.Stream stream)
    {
        throw new NotImplementedException();
    }

    public void FireCanInsertFunction(object sender, System.Windows.Input.CanExecuteRoutedEventArgs eArgs)
    {
        bool canExecute = false;
        CanInsertFunction?.Invoke(ref canExecute);

        eArgs.CanExecute = canExecute;
    }

    public void FireSetIsReplacingPreviouseOperator(bool isReplacingPreviouseOperator)
    {
        if (SetIsReplacingPreviouseOperator != null)
        {
            SetIsReplacingPreviouseOperator.Invoke(isReplacingPreviouseOperator);
        }
    }

    public Size Size
    {
        get
        {
            // Mathcad reads ActualHeight before ActualWidth.
            double height = ActualHeight;
            return new Size(ActualWidth, height);
        }
    }

    public override double TextBaselineY
    {
        get
        {
            if (RootNode is VisualMathNode rootNode)
            {
                return rootNode.TextBaselineY + RootPadding.Top;
            }
            return 0.0;
        }
    }

    [System.Diagnostics.Conditional("EQUATION_WRAPPING_DEBUG")]
    private void Register_EQUATION_WRAPPING_DEBUG_Handlers()
    {
    }

    public void WriteImageTo(System.IO.Stream stream)
    {
        WpfUtils.DoEvents();
        ImageUtils.SavePngFromElementUsingScreenDpi(this, stream, null);
    }

    void IVisualMathRegionNode.Focus()
    {
        base.Focus();
    }

    private static void HandleProhibitMouseClickPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        System.Windows.Controls.UIElementCollection children = ((VisualMathRegionNode)d).Children;
        DebugTools.DevAssert(children.Count < 2);
        bool isHitTestVisible = !Equals(e.NewValue, true);
        for (int i = 0; i < children.Count; i++)
        {
            children[i].IsHitTestVisible = isHitTestVisible;
        }
    }

    public void FirePaste(object sender, ExecutedRoutedEventArgs e)
    {
        if (Paste != null)
        {
            BuildAndExecPasteCmd();
            e.Handled = true;
        }
    }

    public void FirePaste()
    {
        if (Paste != null)
        {
            BuildAndExecPasteCmd();
        }
    }

    public void FireCut(object sender, ExecutedRoutedEventArgs e)
    {
        if (Cut != null)
        {
            BuildAndExecCutCmd();
            e.Handled = true;
        }
    }

    public void FireCut()
    {
        if (Cut != null)
        {
            BuildAndExecCutCmd();
        }
    }

    public void FireCopy(object sender, ExecutedRoutedEventArgs e)
    {
        if (Copy != null)
        {
            Copy.Invoke();
            e.Handled = true;
        }
    }

    public void FireCopy()
    {
        if (Copy != null)
        {
            Copy.Invoke();
        }
    }

    public void FireShowRegionHistory(object sender, ExecutedRoutedEventArgs e)
    {
        if (RequestShowRegionHistory != null)
        {
            RequestShowRegionHistory.Invoke();
            e.Handled = true;
        }
    }

    public void FireCanPaste(object sender, CanExecuteRoutedEventArgs e)
    {
        bool canPaste = false;
        if (CanPaste != null)
        {
            CanPaste.Invoke(ref canPaste);
        }
        e.CanExecute = canPaste;
        if (!e.CanExecute)
        {
            e.Handled = true;
        }
    }

    public void FireRequestRestoreInsertionPoint(Quire.EquationEditor.Tree.RequestRestoreInsertionPointEventArgs requestRestoreInsertionPointEventArgs)
    {
        var routedEventArgs = new RoutedEventArgs(CommonRoutedEvents.RequestRestoreInsertionPointEvent);
        RaiseEvent(routedEventArgs);
        requestRestoreInsertionPointEventArgs.RequestHandled = routedEventArgs.Handled;
    }

    public void FireSetIsReplacingPreviouseOperator(bool isReplacingPreviouseOperator, out bool success)
    {
        success = false;
        if (SetIsReplacingPreviouseOperator != null)
        {
            success = SetIsReplacingPreviouseOperator.Invoke(isReplacingPreviouseOperator);
        }
    }

    public void FireCanTraverse(CanExecuteRoutedEventArgs e, FocusNavigationDirection direction)
    {
        bool canTraverse = false;
        if (direction == FocusNavigationDirection.Next && OnCanTraverseForward != null)
        {
            OnCanTraverseForward.Invoke(ref canTraverse);
        }
        else if (direction == FocusNavigationDirection.Previous && OnCanTraverseBackward != null)
        {
            OnCanTraverseBackward.Invoke(ref canTraverse);
        }
        e.CanExecute = canTraverse;
    }

    public void FireTraversalRequest(ExecutedRoutedEventArgs e, Direction direction)
    {
        bool handled = false;
        if (direction == Direction.Right && OnTraverseForward != null)
        {
            OnTraverseForward.Invoke(ref handled);
        }
        else if (direction == Direction.Left && OnTraverseBackward != null)
        {
            OnTraverseBackward.Invoke(ref handled);
        }
        e.Handled = handled;
    }

    public bool FireCanPerformReplace(FindReplaceQuery findReplaceQuery)
    {
        if (CanPerformReplace != null)
        {
            return CanPerformReplace.Invoke(findReplaceQuery);
        }
        return false;
    }

    public void FireRestoreFromHistory(int index)
    {
        if (_historyPopup != null)
        {
            _historyPopup.Hide();
            _historyPopup.HistoryItemSelected -= FireRestoreFromHistory;
            Children.Remove(_historyPopup);
            _historyPopup = null;
        }
        if (RestoreFromHistory != null)
        {
            RestoreFromHistory.Invoke(index);
        }
    }

    private static void HandleWarningDisplayFilterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is VisualMathRegionNode node)
        {
            node.WarningDisplayFilterChanged?.Invoke();
        }
    }

    internal override System.Windows.Media.HitTestResult GetNodeAtPoint(Point hitPoint)
    {
        var node = (VisualMathNode)Children[0];
        System.Windows.Media.GeneralTransform transform = TransformToDescendant(node);
        return node.GetNodeAtPoint(transform.Transform(hitPoint));
    }

    protected override void OnVisualParentChanged(DependencyObject obj)
    {
        base.OnVisualParentChanged(obj);
        if (VisualParentChanged != null)
        {
            VisualParentChanged.Invoke(this, obj, VisualParent);
        }
    }

    public System.Windows.Media.ScaleTransform GetTransformBasedOnNumericNodeHeight(double desiredHeight)
    {
        double scale = desiredHeight / Font.FontSize;
        return new System.Windows.Media.ScaleTransform(scale, scale);
    }

    public override void HandleMathFormatChanged(DependencyPropertyChangedEventArgs e)
    {
        base.HandleMathFormatChanged(e);
        if (MathFormatChanged != null)
        {
            MathFormatChanged.Invoke(e);
        }
    }

    public void HandleLabeledIdFormatDictionaryChanged(DependencyPropertyChangedEventArgs args)
    {
        var previousFormat = args.OldValue as IDictionary<IdLabels, ExtendedFont>;
        var currentFormat = args.NewValue as IDictionary<IdLabels, ExtendedFont>;
        if (currentFormat == null)
        {
            BackUpLabeledIdDictionary(previousFormat);
            return;
        }
        if (previousFormat == null && IsLabeledIdDictionaryEqualToBackUp(currentFormat))
        {
            return;
        }
        FireLabeledIdFormatChanged();
    }

    private bool IsExtendedFontEqualsToBackUp(IdLabels labelID, ExtendedFont extendedFont)
    {
        if (extendedFont.Equals(_previousLabeledIdFormat[labelID]))
        {
            return extendedFont.RevisionNumber != _previousLabeledIdFormatRevisionNumber[labelID];
        }
        return true;
    }

    internal void Dispose()
    {
        Disposed?.Invoke();
    }

    protected override void OnTextInput(TextCompositionEventArgs e)
    {
        base.OnTextInput(e);
        foreach (char c in e.Text)
        {
            AutomateKeypress(c);
        }
        e.Handled = true;
    }

    public void FireInsertFunction(Quire.EquationEditor.Tree.FormattedString funcName, int argsCount, System.Nullable<Quire.Controls.Core.IdLabels> label)
    {
        InsertFunction?.Invoke(funcName, argsCount, label);
    }

    public void FireInsertFunction(Quire.EquationEditor.Tree.FormattedString funcName, int argsCount, System.Nullable<Quire.Controls.Core.IdLabels> label, string[] args)
    {
        InsertFunctionWithArgs?.Invoke(funcName, argsCount, label, args);
    }

    private System.Windows.Documents.Adorner[] GetAdorners(UIElement visual)
    {
        var adornerLayer = System.Windows.Documents.AdornerLayer.GetAdornerLayer(visual);
        if (adornerLayer != null)
        {
            return adornerLayer.GetAdorners(visual);
        }
        return null;
    }

    private void OnRootNodeChanged(DependencyPropertyChangedEventArgs args)
    {
        Children.Clear();
        if (args.NewValue == null)
        {
            return;
        }
        var element = (UIElement)args.NewValue;
        element.IsHitTestVisible = !AttachedProperties.GetProhibitMouseClick(this);
        Children.Add(element);
        TryFindDefinitionAndUpdateIsLhsVisible();
        FireRootNodeChanged();
    }

    internal void SimulateMouseClick(Point location, int numberOfClicks)
    {
        var hit = GetHitTestResult(location);
        if (hit != null)
        {
            switch (numberOfClicks)
            {
                case 1:
                    FireNodeClick(hit.NodeHit, hit.CaretSideHit, hit.NodeSideHit, hit.OperatorPartIndexHit, hit.PointHit);
                    break;
                case 2:
                    FireNodeDoubleClick(hit.NodeHit, hit.CaretSideHit, hit.NodeSideHit, hit.OperatorPartIndexHit, hit.PointHit);
                    break;
            }
        }
    }

    event Quire.Delegate0 Quire.EquationEditor.View.IVisualMathRegionNode.Loaded
    {
        add
        {
            _loadedEvent = (Quire.Delegate0)Delegate.Combine(_loadedEvent, value);
        }
        remove
        {
            _loadedEvent = (Quire.Delegate0)Delegate.Remove(_loadedEvent, value);
        }
    }

    public void ShowVisualHistory(IList<Quire.EquationEditor.View.IVisualMathNode> nodes)
    {
        if (_historyPopup != null)
        {
            Children.Remove(_historyPopup);
        }
        _historyPopup = new HistoryPopup(nodes);
        Children.Add(_historyPopup);
        _historyPopup.HistoryItemSelected += FireRestoreFromHistory;
        _historyPopup.Show();
    }

}
