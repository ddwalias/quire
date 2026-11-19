using System.Collections;
using System.Globalization;

namespace Quire.Controls.InputControls;

internal partial class InputRadioButtonEngineJScript
{
    public const string DefaultUserScriptObjectName = "RadioBtn";

    private const string defaultScriptResourceKey = "Ptc.Controls.InputControls.implementations.ScriptableRadioButton.JsRadioButtonDefaultScript.js";

    private const string scriptInternalClassResourceKey = "Ptc.Controls.InputControls.implementations.ScriptableRadioButton.JsRadioButtonScriptInternalClass.js";

    protected static string _defaultScript;

    public new const string Event_Click = "_SelectionChange(Inputs, Outputs)\r\n";

    public const string leftText = "leftText";

    public const string orientation = "orientation";

    public override string GetUserScriptObjectName(string userScript, IScriptEngineParams engineParams = null)
    {
        return base.GetUserScriptObjectName(userScript, engineParams) ?? "RadioBtn";
    }

    public override string Script
    {
        get
        {
            if (_defaultScript == null)
            {
                _defaultScript = GetResourceScriptDecrypt("Ptc.Controls.InputControls.implementations.ScriptableRadioButton.JsRadioButtonDefaultScript.js");
            }
            return _currentUserScript ?? _defaultScript;
        }
    }

    public override string AddInternalScripts(string script, string controlObjectName, object[] inputs, IScriptEngineParams engineParams = null)
    {
        if (_scriptInternalClass == null)
        {
            _scriptInternalClass = GetResourceScriptDecrypt("Ptc.Controls.InputControls.implementations.ScriptableRadioButton.JsRadioButtonScriptInternalClass.js");
        }
        return base.AddInternalScripts(script, controlObjectName, inputs, engineParams);
    }

    protected override void UpdateScriptableClassValues(ref object value, object scriptControlObject, IScriptEngineParams engineParams = null)
    {
        if (scriptControlObject is Hashtable hashtable && !engineParams.IsInteractChanged && hashtable.ContainsKey("CurSel") && InputControlScriptingEngine.IsNumeric(hashtable["CurSel"]))
        {
            value = double.Parse(hashtable["CurSel"].ToString(), CultureInfo.InvariantCulture);
        }
        _scriptEngine.Script[ScriptClassObjectName].CurSel(double.Parse(value.ToString(), CultureInfo.InvariantCulture));
    }

    protected override object GetScriptClassProperties(object scriptObjectProperties = null)
    {
        dynamic val = _scriptEngine.Script[ScriptClassObjectName];
        object scriptClassProperties = base.GetScriptClassProperties(scriptObjectProperties);
        if (scriptClassProperties is Hashtable hashtable)
        {
            if (val.Orientation() is int)
            {
                hashtable["orientation"] = (object)val.Orientation();
            }
            if (IsNumeric(val.CurSel()))
            {
                hashtable["CurSel"] = (object)val.CurSel();
            }
            if (val.LeftText() is bool)
            {
                hashtable["leftText"] = (object)val.LeftText();
            }
            if (val.items() != null)
            {
                hashtable["items"] = (object)val.items();
            }
        }
        return scriptClassProperties;
    }

    protected override void GetExistingEvents(string convertUserScript)
    {
        base.GetExistingEvents(convertUserScript);
        EventClick = ScriptClassObjectName + "_SelectionChange(Inputs, Outputs)\r\n";
        if (!convertUserScript.Contains(EventClick.Split('(')[0]))
        {
            EventClick = string.Empty;
        }
    }

    public InputRadioButtonEngineJScript()
    {
    }
}
