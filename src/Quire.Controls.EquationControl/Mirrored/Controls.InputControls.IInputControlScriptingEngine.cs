namespace Quire.Controls.InputControls;

public partial interface IInputControlScriptingEngine
{
    string Script { get; }
    string ScriptClassObjectName { get; }
    string ScriptInternalInputs { get; }
    global::System.Collections.Generic.IList<object> GetCalculatedValue(string userScript, ref object value, out dynamic scriptControlObject, out string exception, object[] inputs, global::Quire.Controls.InputControls.IScriptEngineParams engineParams = null);
    bool Apply(string userScript, out object scriptObject, out string exception, object[] inputs, global::Quire.Controls.InputControls.IScriptEngineParams engineParams = null);
    bool CanExecEvent(string eventCommand);
    global::System.Collections.Generic.IList<object> RunEventStandalone(string eventCommand, out string exception);
    string ConvertUserScript(string userScript);
    string GetUserScriptObjectName(string userScript, global::Quire.Controls.InputControls.IScriptEngineParams engineParams = null);
    string GetResourceScriptDecrypt(string path);
    bool ScriptInternalInputsInit(object[] inputs, int? currentInputSize, bool reset = false);
    string ScriptInternalInputOutputsInit(string inOut, int length);
    string[] GetLabelsFromSExpresstion(string SExpresstion);
}
