#nullable disable

namespace Quire.EquationEditor;

public partial interface IEquationModel
{
    global::Quire.EquationEditor.Tree.RootMathNode Root { get; }
    global::Quire.EquationEditor.Tree.MathNode EquationRoot { get; set; }
    global::Quire.EquationEditor.Tree.MathNode ErrorNode { get; set; }
    global::Quire.EquationEditor.Tree.MathNode EvaluationRoot { get; }
    global::Quire.Calculation.Core.EngineError CalculationError { get; set; }
    global::System.Collections.Generic.IList<global::Quire.Calculation.Core.EngineRedefInfo> EngineWarnings { get; }
    global::Quire.EquationEditor.ScriptingError ScriptingError { get; set; }
    bool IsScriptError { get; }
    global::Quire.EquationEditor.EquationEditModeEnum Mode { get; }
    global::Quire.EquationEditor.Tree.Navigation.NavigatorState NavigatorState { get; set; }
    global::Quire.Direction ScopeSide { get; }
    global::Quire.EquationEditor.Tree.Scoping.ScopedNodes ScopedNodesByLines { get; }
    global::Quire.EquationEditor.Tree.Scoping.TypeoverState CurrentTypeoverState { get; }
    string EquationSExpression { get; }
    bool IsScriptableControl { get; }
    global::Quire.Calculation.Core.IDataExchangeModel DataExchangeModel { get; }
    global::Quire.Calculation.Core.IResultNode EngineResultForSerialization { get; set; }
    string EquationSExpressionWithResolvedLabels { get; }
    string CurrentSExpression { get; }
    string ScopedSExpression { get; }
    string TypeoverSExpression { get; }
    global::System.Xml.XmlNode EquationXmlNode { get; set; }
    global::Quire.Controls.Core.EquationTypes EquationType { get; }
    global::System.Collections.Generic.IEnumerable<string> VisualHistory { get; }
    bool HasVisualHistoryItems { get; }
    bool IsEvaluatable { get; }
    bool HasNumericEvaluation { get; }
    bool IsModified { get; set; }
    bool IsExpressionValueModified { get; set; }
    global::Quire.EquationEditor.IEquationEditorSettings Settings { get; set; }
    bool CurrentOperatorMayBeReplaced { get; }
    bool IsReadonly { get; }
    global::Quire.EquationEditor.Tree.Navigation.NavigatorState CurrentModeNavigatorState { get; }
    global::Quire.Controls.Core.NumberFormat ResultNumberFormat { get; set; }
    global::Quire.Controls.Core.MatrixFormat ResultMatrixFormat { get; set; }
    bool AllowExpressionsOnly { get; set; }
    bool ProhibitComboBox { get; set; }
    bool ProhibitInputControl { get; set; }
    bool IsMathFormatHandledLocally { get; set; }
    string ResultScalingSExpression { get; set; }
    bool IsEmpty { get; }
    bool ResultScalingModified { get; set; }
    global::Quire.EquationEditor.Tree.MathNode ResultScalingFactor { get; set; }
    bool IsRestrictedReadOnly { get; set; }
    bool OverrideIdLabelByResolvedForDefinitionLhs { get; set; }
    global::System.Windows.Visibility VisibleLhsOfDefinition { get; }
    global::System.Collections.Generic.List<global::System.ValueTuple<global::Quire.EquationEditor.RedefinitionWarning, global::Quire.EquationEditor.Tree.MathNode>> Warnings { get; }
    event global::System.Action<bool> EquationChanged;
    event global::System.Action ExpressionValueChanged;
    event global::System.Action EvaluationErrorOccurred;
    event global::System.Action WarningsOccurred;
    event global::System.Action ContentChanged;
    event global::System.Action<global::Quire.Wpf.PropertyChangedEventArgs<global::Quire.Calculation.Core.EngineError>> CalculationErrorChanged;
    event global::System.Action<global::Quire.EquationEditor.Equation.ScopeStateChangeEventArgs> ScopeChanged;
    event global::System.Action<global::Quire.EquationEditor.NavigatorStateChangeEventArgs> NavigatorStateChanged;
    event global::System.Action<global::Quire.EquationEditor.TypeoverChangeEventArgs> TypeoverChanged;
    event global::System.Action<global::Quire.Calculation.Core.IResultNode> NumericResultChanged;
    event global::System.Action<global::Quire.Direction> RequestRemoveEquation;
    event global::System.Action<global::Quire.EquationEditor.IEquationModel> RequestEvaluation;
    event global::System.Action<global::Quire.EquationEditor.IEquationModel> RequestFocus;
    event global::System.Action<global::Quire.EquationEditor.Tree.Navigation.NavigatorState> NodeMayBeReplaced;
    event global::System.Action InCustomInitializationChanged;
    event global::System.Action<bool> AlgorithmChanged;
    event global::System.EventHandler<global::Quire.EquationEditor.Tree.RequestRestoreInsertionPointEventArgs> RequestRestoreInsertionPoint;
    event global::System.Action<bool> RestrictedReadonlyChanged;
    event global::System.Action<global::System.Windows.Visibility> VisibleLhsOfDefinitionChanged;
    void SetDataExchangeModel(global::Quire.EquationEditor.IMathNodeToVisualNodeMap nodeMap);
    bool SetEquationSExpression(string sExpr, bool isUserOperation = true);
    void CanPerformIdLabelOperations(ref bool canPerformIdLabelOperations);
    bool SetDefinitionValue(string sExpr, string valueSExpr, string scalingSExpr);
    void EnterNode(global::Quire.Direction direction, global::Quire.EquationEditor.Tree.MathNode nodeToEnter);
    void CancelScope();
    void CancelTypeover();
    bool CanCopy();
    bool CanCut();
    bool CanPaste();
    void CanCopy(ref bool canCopy);
    void CanCut(ref bool canCut);
    void CanPaste(ref bool canPaste);
    void ContractScope();
    void Copy();
    void Cut();
    void Clear();
    void ClearError();
    void Delete(global::Quire.Direction deleteDirection);
    bool CanPerformReplace(global::Quire.Controls.Core.FindReplaceQuery findReplaceQuery);
    void Replace(global::Quire.Controls.Core.FindReplaceQuery findReplaceQuery);
    void CloseParen();
    void ExpandScope();
    void FullyExpandScope();
    void FullyContractScope();
    bool SetIsReplacingPreviouseOperator(bool isReplacingPreviouseOperator);
    void SetCurrentOperatorMayBeReplaced(bool currentOperatorMayBeReplaced);
    void InsertOperator(string mathNodeName);
    void InsertValue(string valueToInsert);
    void InsertString(string str);
    void InsertInputControl(global::Quire.Controls.Core.InputControlTypes inputControlType);
    void CanInsertInputControl(ref bool canInsert, global::System.Nullable<global::Quire.Controls.Core.InputControlTypes> inputControlType);
    void InsertFunction(global::Quire.EquationEditor.Tree.FormattedString funcName, int argsCount, global::System.Nullable<global::Quire.Controls.Core.IdLabels> label);
    void CanInsertFunction(ref bool canInsertFunction);
    void InsertFunctionWithArgs(global::Quire.EquationEditor.Tree.FormattedString funcName, int argsCount, global::System.Nullable<global::Quire.Controls.Core.IdLabels> label, string[] args);
    void InsertId(global::Quire.EquationEditor.Tree.FormattedString name, global::System.Nullable<global::Quire.Controls.Core.IdLabels> label);
    void InsertSymbolicEvaluation();
    void InsertMatrix(global::System.Nullable<global::Quire.TableSize> matrixSize);
    void InsertKeyword(string sExpression);
    bool CanInsertKeyword();
    void InsertModifier(string sExpression);
    void InsertUnitMonomial(global::System.Collections.Generic.IEnumerable<global::Quire.Calculation.Core.Unit> unitMonomial);
    void FireAlgorithmChanged();
    void ProcessEqualsKey();
    void CanPerformColumnOperations(ref bool canPerformMatrixOperation);
    void CanPerformClearCellsOperation(ref bool canPerformClearCellsOperation);
    void CanNavigateToOppositeOfScope(ref bool canNavigateToOppositeOfScope);
    void InsertRow(global::Quire.Direction direction);
    void InsertColumn(global::Quire.Direction direction);
    void DeleteRow();
    void DeleteColumn();
    void DeleteContents(global::Quire.EquationEditor.Tree.MathNode mathNode);
    void MoveToRegionStart();
    void MoveToRegionEnd();
    void MoveToCurrentExpressionEnd();
    void MoveToOppositeSide();
    void MoveToCurrentExpressionStart();
    void MoveLeft();
    void MoveLeftByTerm();
    void MoveRight();
    void MoveRightByTerm();
    void MoveUp();
    void MoveDown();
    void JumpToNode(global::Quire.EquationEditor.Tree.MathNode newNode, global::Quire.Direction caretSide);
    bool Paste();
    void SelectTo(global::Quire.EquationEditor.Tree.MathNode node, global::Quire.Direction nodeSide, global::Quire.Direction dragDirection);
    void ScopeLeft();
    void ScopeRight();
    void SelectLeft();
    void SelectRight();
    void SelectToEnd();
    void SelectToStart();
    int GetRlIndexForNumericResult();
    void SetResult(int rlIndex, global::Quire.Calculation.Core.IResultNode resultValue);
    void HandleError(global::Quire.Calculation.Core.EngineError error, int rlIndex = 0);
    void HandleScriptingEngineError(string error);
    void SetIdentifierLabels(global::System.Collections.Generic.IDictionary<global::Quire.Calculation.Core.ResolvedId, string[]> idLabelMap);
    void SetAutoSelectAlgorithm(global::System.Collections.Generic.IDictionary<int, string> autoAlgorithmMap);
    void SetRedefinitionWarnings(global::System.Collections.Generic.IList<global::Quire.Calculation.Core.EngineRedefInfo> warnings);
    void ToggleGreek();
    void ToggleSubscript();
    void ToggleVisibleLhsOfDefinition(global::System.Windows.Visibility paramVisibility);
    bool CanToggleVisibleLhsOfDefinition();
    void ConvertToOperator();
    void RestoreFromHistory(int historyIndex);
    void FireSynchEvents();
    void OnCanTraverseForward(ref bool canTraverse);
    void OnCanTraverseBackward(ref bool canTraverse);
    void OnTraverseForward(ref bool handled);
    void OnTraverseBackward(ref bool handled);
    void MoveToRegionExtreme(global::Quire.Direction direction);
    void FireRequestFocus();
    void ClearCells();
    bool AtLeftEdge();
    bool AtRightEdge();
    bool AtEdge(global::Quire.Direction direction);
    void ClearContent();
    void EditNode(global::Quire.EquationEditor.Tree.MathNode node, int position);
    void EditNode(global::Quire.EquationEditor.Tree.MathNode node, global::Quire.Direction caretSide, global::Quire.Direction nodeSide);
    void FireEquationChanged(bool shouldRegenerate = true);
    void FireNavigatorStateChanged();
    global::Quire.EquationEditor.Tree.MathNode FindChildWithMatchingId(uint rlid);
    bool Find(string text, bool startFromCaret, global::System.Windows.Documents.LogicalDirection direction, bool matchCase, bool wholeWordOnly, global::System.Collections.Generic.List<global::Quire.Pair<int, int>> subscriptPositions);
    void SetIdLabel(global::Quire.Controls.Core.IdLabels label);
    void UnsetIdLabel();
    void ToggleUnitIdLabel();
    void ToggleIdLabel();
    global::System.Collections.Generic.IList<string> GetDefinedIdentifierSExpressions(bool includeFunctions = false);
    double GetRealResult();
    bool CanNavigate(global::Quire.Direction direction);
    void SplitLinearNaryLine();
    void CanPerformLinearNaryOperations(ref bool canPerform);
    void CanPerformRowOperations(ref bool canPerform);
    global::Quire.EquationEditor.Tree.Navigation.NavigatorState GetNavigatorStateFromScoperCaretSide();
    void DragIntoMatrixHiddenPart(global::Quire.EquationEditor.Tree.MathNode matrix, int rowShift, int columnShift);
    bool SetEquationSExpressionWithAutoLabeling(string sExpression);
    bool ResolvedLabelOverridesIdLabel(global::Quire.EquationEditor.Tree.MathNode curNode);
    void HandleInputTagChanged(bool isRestrictedReadonly);
    bool IsSuitableForInputTagging();
    void ScopeIdentifier(string idSexpr);
    void ScopeResult();
    void ScopeNode(global::Quire.EquationEditor.Tree.MathNode nodeToScope, global::Quire.Direction direction = (global::Quire.Direction)(0));
    string GetDefinitionSExpression(string idSExpr);
    string GetEvaluationSExp(bool getDefineForDefineEval);
    void SwitchLineBreakOperator();
    bool CanToggleLineBreakOperator(out bool isToggled);
}
