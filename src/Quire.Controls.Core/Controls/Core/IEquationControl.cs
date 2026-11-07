#nullable disable
using System;
using System.Windows;
using System.Windows.Media;
using System.Xml;

namespace Quire.Controls.Core;

public partial interface IEquationControl
{
    string EquationSExpression { get; set; }
    string TypeoverSExpression { get; }
    string EquationSExpressionWithResolvedLabels { get; }
    EquationTypes EquationType { get; }
    string[] History { get; }
    bool IsScriptableControl { get; }
    bool IsWithinSolveBlock { get; set; }
    string CurrentNodeLabel { get; }
    bool ErrorVisible { get; }
    int CurrentNodeCaretPosition { get; }
    string[] HighlightedOperands { get; }
    bool CurrentNodeIsIdentifier { get; }
    bool CurrentNodeIsNumeric { get; }
    bool IsReadonly { get; }
    bool IsScoped { get; }
    bool IsTypeover { get; }
    Direction CurrentNodeNodeSide { get; }
    Direction CurrentNodeCaretSide { get; }
    Direction ScopeDirection { get; }
    string SelectedOperator { get; }
    string CurrentNodeSExpression { get; }
    TableCoordinate ActiveMatrixCell { get; }
    int MatrixRows { get; }
    int MatrixColumns { get; }
    bool AllowExpressionsOnly { get; set; }
    bool ProhibitComboBox { get; set; }
    bool ProhibitInputControl { get; set; }
    bool IsMathFormatHandledLocally { get; set; }
    bool IsExceedsWidthConstrain { get; }
    bool IsShrunk { get; }
    bool IsResizable { get; }
    bool IsResizableInputControl { get; }
    double SymbolicResultsWidth { get; }
    bool OverrideIdLabelByResolvedForDefinitionLhs { get; set; }
    event Delegate1<IEquationControl> ChangeToTextBox;
    event Action<string> SExpressionChanging;
    event Action<IEquationControl> ExpressionValueChanged;
    event Action RootNodeChanged;
    bool AddToDataObject(IDataObject dataObject);
    int GetRegionHistoryCount();
    bool IsChangeToTextBoxReady();
    void Restore(int historyIndex);
    void EditNode(string label);
    void EditNode(string label, int caretPosition);
    void EditNodeOnCaretSide(string label, Direction caretSide);
    void EditNodeOnNodeSide(string label, Direction nodeSide);
    void ScopeTo(string label, Direction direction);
    void ScopeLhs();
    void Cut();
    void Copy();
    void Paste();
    void Backspace();
    void Delete();
    void ContractScope();
    void ExpandScope();
    void ExpandScopeLeft();
    void ExpandScopeRight();
    void ExitGroupOnRight();
    void FullyExpandScope();
    void FullyContractScope();
    void TransformCharacter();
    void NavigateLeftByTerm();
    void NavigateRightByTerm();
    void NavigateRegionEnd();
    void NavigateRegionStart();
    void Pi();
    void ScopeHome();
    void ScopeEnd();
    Rect GetLabeledScreenInkBounds(string nodeLabel);
    ScaleTransform GetTransformBasedOnNumericNodeHeight(double desiredHeight);
    void Keypress(string s);
    void SetRegionXmlNode(XmlNode value);
    XmlNode GetRegionXmlNode();
    void ExecuteOnRootLoaded(Action action);
    string GetCurrentNodeIdentifier();
    void InputControlOnEquationActivated(object param);
    void DetachFromModel();
    void HandleSizeChanged(Vector offset, bool changeWidth, bool changeHeight, bool resizeInProgress);
}
