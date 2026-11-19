using System.Collections;
using System.Globalization;

namespace Quire.Controls.InputControls;

internal partial class InputRadioButtonEngineVBScript
{
    private string _scriptItemsAsString;

    private const string scriptInternalClassResourceKey = "Ptc.Controls.InputControls.implementations.ScriptableRadioButton.VbRadioButtonScriptInternalClass.vbs";

    private const string defaultScriptResourceKey = "Ptc.Controls.InputControls.implementations.ScriptableRadioButton.VbRadioButtonDefaultScript.vbs";

    public new const string Event_Click = "_SelectionChange()\r\n";

    protected static string _defaultScript;

    public override string Script
    {
        get
        {
            if (_defaultScript == null)
            {
                _defaultScript = GetResourceScriptDecrypt("Ptc.Controls.InputControls.implementations.ScriptableRadioButton.VbRadioButtonDefaultScript.vbs");
            }
            return _currentUserScript ?? _defaultScript;
        }
    }

    public override string AddInternalScripts(string script, string controlObjectName, object[] inputs, IScriptEngineParams engineParams = null)
    {
        if (_scriptInternalClass == null)
        {
            _scriptInternalClass = GetResourceScriptDecrypt("Ptc.Controls.InputControls.implementations.ScriptableRadioButton.VbRadioButtonScriptInternalClass.vbs");
        }
        return base.AddInternalScripts(script, controlObjectName, inputs, engineParams);
    }

    public override string GetUserScriptObjectName(string userScript, IScriptEngineParams engineParams = null)
    {
        return base.GetUserScriptObjectName(userScript, engineParams) ?? "RadioBtn";
    }

    protected override void UpdateScriptableClassValues(ref object value, object scriptControlObject, IScriptEngineParams engineParams = null)
    {
        if (scriptControlObject is Hashtable hashtable && !engineParams.IsInteractChanged && hashtable.ContainsKey("CurSel") && InputControlScriptingEngine.IsNumeric(hashtable["CurSel"]))
        {
            value = double.Parse(hashtable["CurSel"].ToString(), CultureInfo.InvariantCulture);
        }
        _scriptEngine.Script[ScriptClassObjectName].CurSel = double.Parse(value.ToString(), CultureInfo.InvariantCulture);
    }

    protected override void GetExistingEvents(string convertUserScript)
    {
        base.GetExistingEvents(convertUserScript);
        EventClick = ScriptClassObjectName + "_SelectionChange()\r\n";
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
            if (val.Orientation is short)
            {
                hashtable["orientation"] = (object)val.Orientation;
            }
            if (IsNumeric(val.CurSel))
            {
                hashtable["CurSel"] = (object)val.CurSel;
            }
            if (val.LeftText is bool)
            {
                hashtable["leftText"] = (object)val.LeftText;
            }
            hashtable["items"] = string.Empty;
            if (val.items is string)
            {
                hashtable["items"] = (object)val.items;
            }
        }
        return scriptClassProperties;
    }

    public InputRadioButtonEngineVBScript()
    {
    }
}
