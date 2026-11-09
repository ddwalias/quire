namespace Quire.Controls.InputControls;

public partial interface IInputControlEditorScriptableModel
{
    string Script { get; set; }
    string EngineError { get; }
    void HandlePostError(string error);
    string DefaultScript(global::Quire.EquationEditor.Tree.InputControlEngineTypes engineType);
    bool Apply(string userScript, out object scriptObject, out string exception, object[] inputs, global::Quire.Controls.InputControls.IScriptEngineParams engineParams = default);
}
