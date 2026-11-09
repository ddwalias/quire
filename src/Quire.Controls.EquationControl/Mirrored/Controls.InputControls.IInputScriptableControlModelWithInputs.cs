namespace Quire.Controls.InputControls;

public partial interface IInputScriptableControlModelWithInputs
{
    string InputFieldSExpression { get; set; }
    event global::System.Action CommitNeeded;
    void HandleSExpressionChanged(global::Quire.Controls.Core.IEquationControl eq);
}
