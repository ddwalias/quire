using System.Collections;

namespace Quire.Controls.InputControls;

internal partial class InputButtonEngineJScript
{
    public const string DefaultUserScriptObjectName = "PushBtn";

    public const string text = "text";

    private const string defaultScriptResourceKey = "Ptc.Controls.InputControls.implementations.ScriptableButton.JsButtonDefaultScript.js";

    private const string scriptInternalClassResourceKey = "Ptc.Controls.InputControls.implementations.ScriptableButton.JsButtonScriptInternalClass.js";

    protected static string _defaultScript;

    public override string Script
    {
        get
        {
            if (_defaultScript == null)
            {
                _defaultScript = GetResourceScriptDecrypt("Ptc.Controls.InputControls.implementations.ScriptableButton.JsButtonDefaultScript.js");
            }
            return _currentUserScript ?? _defaultScript;
        }
    }

    public override string AddInternalScripts(string script, string controlObjectName, object[] inputs, IScriptEngineParams engineParams = null)
    {
        if (_scriptInternalClass == null)
        {
            _scriptInternalClass = GetResourceScriptDecrypt("Ptc.Controls.InputControls.implementations.ScriptableButton.JsButtonScriptInternalClass.js");
        }
        return base.AddInternalScripts(script, controlObjectName, inputs, engineParams);
    }

    protected override void GetExistingEvents(string convertUserScript)
    {
        base.GetExistingEvents(convertUserScript);
        EventClick = ScriptClassObjectName + "_Click(Inputs, Outputs)\r\n";
        if (!convertUserScript.Contains(EventClick.Split('(')[0]))
        {
            EventClick = string.Empty;
        }
    }

    protected override object GetScriptClassProperties(object scriptObjectProperties = null)
    {
        dynamic val = _scriptEngine.Script[ScriptClassObjectName];
        object scriptClassProperties = base.GetScriptClassProperties(scriptObjectProperties);
        if (scriptClassProperties is Hashtable hashtable)
        {
            if (val.Text() != null)
            {
                hashtable["text"] = (object)val.Text();
            }
        }
        return scriptClassProperties;
    }

    protected override void UpdateScriptableClassValues(ref object value, object scriptControlObject, IScriptEngineParams engineParams = null)
    {
    }

    public InputButtonEngineJScript()
    {
    }
}
