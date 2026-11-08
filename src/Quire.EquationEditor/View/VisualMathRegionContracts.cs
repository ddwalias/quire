namespace Quire.EquationEditor.View
{
    public partial interface IVisualMathRegionNode
    {
        IVisualMathNode RootNode { get; set; }

        bool IsLogicalFocusWithin { get; }

        bool IsLogicalFocusWithinScriptableControl { get; }

        global::System.Windows.Visibility LhsOfDefinitionVisibility { get; }

        bool SymbolicEvalKeywordsVisibilityState { get; }

        global::System.Windows.Rect ScreenBounds { get; }

        IVisualFactory VisualFactory { get; set; }

        global::System.Windows.Size Size { get; }

        global::Quire.Controls.Core.IMathFormat RootMathFormat { get; }

        global::Quire.Controls.Core.NodeMessageAdorner VisibleWarningMessage { get; }

        global::Quire.Controls.Core.WarningsTypesToShow WarningDisplayFilter { get; }

        event global::Quire.Delegate0 Loaded;

        event global::Quire.Delegate0 Activated;

        event global::Quire.Delegate0 Deactivated;

        event global::Quire.Delegate1<char> KeyGesture;

        event global::Quire.EquationEditor.View.NodeClickHandler NodeClick;

        event global::Quire.EquationEditor.View.NodeDoubleClickHandler NodeDoubleClick;

        event global::Quire.Delegate3<global::Quire.EquationEditor.View.IVisualMathNode, global::Quire.Direction, global::Quire.Direction> MouseDragOver;

        event global::Quire.Delegate0 MoveLeft;

        event global::Quire.Delegate0 MoveRight;

        event global::Quire.Delegate0 MoveUp;

        event global::Quire.Delegate0 MoveDown;

        event global::Quire.Delegate0 MoveRightByTerm;

        event global::Quire.Delegate0 MoveLeftByTerm;

        event global::Quire.Delegate0 MoveToRegionStart;

        event global::Quire.Delegate0 MoveToRegionEnd;

        event global::Quire.Delegate0 Pi;

        event global::Quire.Delegate0 ScopeLeft;

        event global::Quire.Delegate0 ScopeRight;

        event global::Quire.Delegate0 ExpandScope;

        event global::Quire.Delegate0 ContractScope;

        event global::Quire.Delegate0 FullyExpandScope;

        event global::Quire.Delegate0 FullyContractScope;

        event global::Quire.Delegate0 DeleteLeft;

        event global::Quire.Delegate0 DeleteRight;

        event global::Quire.Delegate0 MoveToStart;

        event global::Quire.Delegate0 MoveToEnd;

        event global::Quire.Delegate0 MoveToOpposite;

        event global::Quire.Delegate0 Greek;

        event global::System.Action ToggleSubscript;

        event global::Quire.Delegate0 ExitGroupLeft;

        event global::Quire.Delegate0 ExitGroupRight;

        event global::Quire.Delegate0 ConvertToOperator;

        event global::Quire.Delegate0 InsertEvaluation;

        event global::System.Action InsertSymbolicEvaluation;

        event global::Quire.DelegateRef1<bool> OnCanTraverseForward;

        event global::Quire.DelegateRef1<bool> OnCanTraverseBackward;

        event global::Quire.DelegateRef1<bool> OnTraverseForward;

        event global::Quire.DelegateRef1<bool> OnTraverseBackward;

        event global::Quire.Delegate1<string> InsertOperator;

        event global::Quire.DelegateFunction1<bool, bool> SetIsReplacingPreviouseOperator;

        event global::Quire.Delegate1<bool> SetCurrentOperatorMayBeReplaced;

        event global::Quire.Delegate1<string> InsertString;

        event global::System.Action<global::Quire.Controls.Core.InputControlTypes> InsertInputControl;

        event global::Quire.DelegateRef1Var1<bool, global::System.Nullable<global::Quire.Controls.Core.InputControlTypes>> CanInsertInputControl;

        event global::Quire.DelegateRef1<bool> CanInsertFunction;

        event global::Quire.DelegateFunction1<bool, global::Quire.Controls.Core.FindReplaceQuery> CanPerformReplace;

        event global::Quire.Delegate1<global::Quire.Controls.Core.FindReplaceQuery> Replace;

        event global::Quire.Delegate3<global::Quire.EquationEditor.Tree.FormattedString, int, global::System.Nullable<global::Quire.Controls.Core.IdLabels>> InsertFunction;

        event global::System.Action<global::Quire.EquationEditor.Tree.FormattedString, int, global::System.Nullable<global::Quire.Controls.Core.IdLabels>, string[]> InsertFunctionWithArgs;

        event global::Quire.Delegate2<global::Quire.EquationEditor.Tree.FormattedString, global::System.Nullable<global::Quire.Controls.Core.IdLabels>> InsertId;

        event global::System.Action<string> InsertKeyword;

        event global::System.Func<bool> CanInsertKeyword;

        event global::System.Action<string> InsertModifier;

        event global::Quire.Delegate1<global::System.Nullable<global::Quire.TableSize>> InsertMatrix;

        event global::Quire.Delegate1<global::Quire.Direction> InsertRow;

        event global::Quire.Delegate1<global::Quire.Direction> InsertMatrixColumn;

        event global::Quire.Delegate0 DeleteRow;

        event global::Quire.Delegate0 DeleteMatrixColumn;

        event global::Quire.Delegate1<global::Quire.EquationEditor.Tree.MathNode> DeleteMatrixContents;

        event global::Quire.Delegate0 ClearCells;

        event global::Quire.DelegateRef1<bool> CanPerformColumnOperations;

        event global::Quire.DelegateRef1<bool> CanPerformClearCellsOperation;

        event global::Quire.DelegateRef1<bool> CanNavigateToOppositeOfScope;

        event global::Quire.Delegate0 SplitLinearNaryLine;

        event global::Quire.DelegateRef1<bool> CanPerformLinearNaryOperations;

        event global::Quire.DelegateRef1<bool> CanPerformRowOperations;

        event global::Quire.Delegate0 Copy;

        event global::Quire.DelegateRef1<bool> CanCopy;

        event global::Quire.DelegateFunction0<bool> Paste;

        event global::Quire.DelegateRef1<bool> CanPaste;

        event global::Quire.Delegate0 Cut;

        event global::Quire.DelegateRef1<bool> CanCut;

        event global::Quire.Delegate0 RequestShowRegionHistory;

        event global::Quire.DelegateRef1<bool> CanShowRegionHistory;

        event global::Quire.Delegate1<int> RestoreFromHistory;

        event global::Quire.Delegate1<global::System.Windows.Vector> DropRegion;

        event global::Quire.Delegate0 LabeledIdFormatChanged;

        event global::System.Action<global::System.Windows.DependencyPropertyChangedEventArgs> MathFormatChanged;

        event global::System.Action<global::Quire.EquationEditor.View.IVisualMathRegionNode, global::System.Windows.DependencyObject, global::System.Windows.DependencyObject> VisualParentChanged;

        event global::System.Action<global::Quire.EquationEditor.View.IVisualMatrixNode, int, int> DragIntoMatrixHiddenPart;

        event global::System.Action RootNodeChanged;

        event global::System.Action<bool> InputTagChanged;

        event global::System.Action<global::System.Collections.Generic.IEnumerable<global::Quire.Calculation.Core.Unit>> InsertUnitMonomial;

        event global::Quire.DelegateRef1<bool> CanPerformIdLabelOperations;

        event global::System.Action ToggleUnitIdLabel;

        event global::System.Action ToggleIdLabel;

        event global::System.Action UnsetIdLabel;

        event global::System.Action<global::Quire.Controls.Core.IdLabels> SetIdLabel;

        event global::System.Action SwitchLineBreakOperator;

        event global::System.Action WarningDisplayFilterChanged;

        event global::Quire.EquationEditor.View.CanExecuteWithToggle CanToggleLineBreakOperator;

        event global::System.Action Disposed;

        void Focus();

        void WriteImageTo(global::System.IO.Stream stream);

        void SetError(global::Quire.Calculation.Core.IEngineError error, IVisualMathNode node);

        void SetError(global::Quire.Calculation.Core.IEngineError error, global::System.Collections.Generic.IList<IVisualMathNode> nodes);

        void RemoveError();

        void ShowVisualHistory(global::System.Collections.Generic.IList<IVisualMathNode> nodes);

        void ClearEditors();

        void ClearKeyStrokesCache();

        global::System.Windows.Media.ScaleTransform GetTransformBasedOnNumericNodeHeight(double desiredHeight);

        void UseDefaultGestures();

        void DisableGestures();

        void UseGesturesForLimitSideNode();

        void RefreshView();

        void EnsureFocus();

        void CloseEditingMode();

        void CloseComboBoxDropDown();

        void FireRequestRestoreInsertionPoint(global::Quire.EquationEditor.Tree.RequestRestoreInsertionPointEventArgs requestRestoreInsertionPointEventArgs);

        void SetWarnings(global::System.Collections.Generic.IList<global::System.ValueTuple<global::Quire.EquationEditor.RedefinitionWarning, IVisualMathNode>> warning);

        void RemoveWarnings();
    }
}
