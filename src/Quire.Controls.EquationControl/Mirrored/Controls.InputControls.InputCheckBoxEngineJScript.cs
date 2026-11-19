using System.Collections;
using System.Globalization;

namespace Quire.Controls.InputControls;

internal partial class InputCheckBoxEngineJScript
{
    public const string DefaultUserScriptObjectName = "CheckBox";

    private const string scriptInternalClassResourceKey = "Ptc.Controls.InputControls.implementations.ScriptableCheckBox.JsCheckBoxScriptInternalClass.js";

    private const string defaultScriptResourceKey = "Ptc.Controls.InputControls.implementations.ScriptableCheckBox.JsCheckBoxDefaultScript.js";

    protected static string _defaultScript;

    public const string text = "text";

    public const string leftText = "leftText";

    public const string check = "check";

    public override string GetUserScriptObjectName(string userScript, IScriptEngineParams engineParams = null)
    {
        return base.GetUserScriptObjectName(userScript, engineParams) ?? "CheckBox";
    }

    public override string Script
    {
        get
        {
            if (_defaultScript == null)
            {
                _defaultScript = GetResourceScriptDecrypt("Ptc.Controls.InputControls.implementations.ScriptableCheckBox.JsCheckBoxDefaultScript.js");
            }
            return _currentUserScript ?? _defaultScript;
        }
    }

    public override string AddInternalScripts(string script, string controlObjectName, object[] inputs, IScriptEngineParams engineParams = null)
    {
        if (_scriptInternalClass == null)
        {
            _scriptInternalClass = GetResourceScriptDecrypt("Ptc.Controls.InputControls.implementations.ScriptableCheckBox.JsCheckBoxScriptInternalClass.js");
        }
        return base.AddInternalScripts(script, controlObjectName, inputs, engineParams);
    }

    protected override void UpdateScriptableClassValues(ref object value, object scriptControlObject, IScriptEngineParams engineParams = null)
    {
        if (scriptControlObject is Hashtable hashtable && !engineParams.IsInteractChanged && hashtable.ContainsKey("check"))
        {
            value = double.Parse(hashtable["check"].ToString(), CultureInfo.InvariantCulture);
        }
        _scriptEngine.Script[ScriptClassObjectName].Check(value);
    }

    protected override void GetExistingEvents(string convertUserScript)
    {
        base.GetExistingEvents(convertUserScript);
        EventClick = ScriptClassObjectName + "_Click(Inputs, Outputs)\r\n";
        if (!convertUserScript.Contains(EventClick.Split(new char[] { '(' })[0]))
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
            if (val.LeftText() is bool)
            {
                hashtable["leftText"] = (object)val.LeftText();
            }
            if (val.Check() is bool)
            {
                hashtable["check"] = val.Check() ? 1 : 0;
            }
        }
        return scriptClassProperties;
    }

    public InputCheckBoxEngineJScript()
    {
    }
}
