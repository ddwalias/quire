namespace Quire.Controls.InputControls;

public partial interface IInputScriptableControlModel
{
    string Script { get; }
    bool Apply(string userScript, out object scriptObject, out string exception, object[] inputs, global::Quire.Controls.InputControls.IScriptEngineParams engineParams = default);
}
