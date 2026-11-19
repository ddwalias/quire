using System;
using System.Collections;
using System.Globalization;
using Microsoft.ClearScript;

namespace Quire.Controls.InputControls;

internal partial class InputCheckBoxEngineVBScript
{
    private const string scriptInternalClassResourceKey = "Ptc.Controls.InputControls.implementations.ScriptableCheckBox.VbCheckBoxScriptInternalClass.vbs";

    private const string defaultScriptResourceKey = "Ptc.Controls.InputControls.implementations.ScriptableCheckBox.VbCheckBoxDefaultScript.vbs";

    protected static string _defaultScript;

    public override string Script
    {
        get
        {
            if (_defaultScript == null)
            {
                _defaultScript = GetResourceScriptDecrypt("Ptc.Controls.InputControls.implementations.ScriptableCheckBox.VbCheckBoxDefaultScript.vbs");
            }
            return _currentUserScript ?? _defaultScript;
        }
    }

    public override string AddInternalScripts(string script, string controlObjectName, object[] inputs, IScriptEngineParams engineParams = null)
    {
        if (_scriptInternalClass == null)
        {
            _scriptInternalClass = GetResourceScriptDecrypt("Ptc.Controls.InputControls.implementations.ScriptableCheckBox.VbCheckBoxScriptInternalClass.vbs");
        }
        return base.AddInternalScripts(script, controlObjectName, inputs, engineParams);
    }

    protected override void UpdateScriptableClassValues(ref object value, object scriptControlObject, IScriptEngineParams engineParams = null)
    {
        if (scriptControlObject is Hashtable hashtable && !engineParams.IsInteractChanged && hashtable.ContainsKey("check"))
        {
            value = double.Parse(hashtable["check"].ToString(), CultureInfo.InvariantCulture);
        }
        _scriptEngine.Script[ScriptClassObjectName].check = value;
    }

    public override string GetUserScriptObjectName(string userScript, IScriptEngineParams engineParams = null)
    {
        return base.GetUserScriptObjectName(userScript, engineParams) ?? "CheckBox";
    }

    public override string ConvertUserScript(string userScript)
    {
        return base.ConvertUserScript(userScript).Replace("Event_Click(", "_Click(");
    }

    protected override void GetExistingEvents(string convertUserScript)
    {
        base.GetExistingEvents(convertUserScript);
        EventClick = ScriptClassObjectName + "_Click()\r\n";
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
            bool flag = val.Text is string || val.Text is DateTime;
            if (flag || val.Text != Undefined.Value)
            {
                hashtable["text"] = (object)val.Text;
            }
            if (val.LeftText is bool)
            {
                hashtable["leftText"] = (object)val.LeftText;
            }
            if (val.Check is bool)
            {
                hashtable["check"] = val.Check ? 1 : 0;
            }
        }
        return scriptClassProperties;
    }

    public InputCheckBoxEngineVBScript()
    {
    }
}
