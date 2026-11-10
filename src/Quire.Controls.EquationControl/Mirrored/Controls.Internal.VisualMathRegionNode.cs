using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Quire.Controls.Core;
using Quire.EquationEditor.View;
using System.Linq;
using Quire.Wpf;
using Quire.Controls.InputControls;

namespace Quire.Controls.Internal;

internal partial class VisualMathRegionNode
{
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
}
