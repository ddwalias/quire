using System;
using System.Collections;
using Microsoft.ClearScript;

namespace Quire.Controls.InputControls;

internal partial class InputButtonEngineVBScript
{
    protected string scriptInternalClassResourceKey = "Ptc.Controls.InputControls.implementations.ScriptableButton.VbButtonScriptInternalClass.vbs";

    private const string defaultScriptResourceKey = "Ptc.Controls.InputControls.implementations.ScriptableButton.VbButtonDefaultScript.vbs";

    protected static string _defaultScript;

    public override string Script
    {
        get
        {
            if (_defaultScript == null)
            {
                _defaultScript = GetResourceScriptDecrypt("Ptc.Controls.InputControls.implementations.ScriptableButton.VbButtonDefaultScript.vbs");
            }
            return _currentUserScript ?? _defaultScript;
        }
    }

    public override string AddInternalScripts(string script, string controlObjectName, object[] inputs, IScriptEngineParams engineParams = null)
    {
        if (_scriptInternalClass == null)
        {
            _scriptInternalClass = GetResourceScriptDecrypt(scriptInternalClassResourceKey);
        }
        return base.AddInternalScripts(script, controlObjectName, inputs, engineParams);
    }

    protected override void UpdateScriptableClassValues(ref object value, object scriptControlObject, IScriptEngineParams engineParams = default)
    {
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
        }
        return scriptClassProperties;
    }

    public InputButtonEngineVBScript()
    {
    }
}
