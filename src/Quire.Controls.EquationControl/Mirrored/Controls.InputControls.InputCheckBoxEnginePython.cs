using System;
using System.Collections;
using System.Globalization;
using Microsoft.ClearScript;

namespace Quire.Controls.InputControls;

internal partial class InputCheckBoxEnginePython
{
    public const string DefaultUserScriptObjectName = "CheckBox";

    public const string text = "text";

    public const string leftText = "leftText";

    public const string check = "check";

    private const string defaultScriptResourceKey = "Ptc.Controls.InputControls.implementations.ScriptableCheckBox.PythonCheckBoxDefaultScript.py";

    private const string scriptInternalClassResourceKey = "Ptc.Controls.InputControls.implementations.ScriptableCheckBox.PythonCheckBoxScriptInternalClass.py";

    protected static string _defaultScript;

    public override string Script
    {
        get
        {
            if (_defaultScript == null)
            {
                _defaultScript = GetResourceScriptDecrypt("Ptc.Controls.InputControls.implementations.ScriptableCheckBox.PythonCheckBoxDefaultScript.py");
            }
            return _currentUserScript ?? _defaultScript;
        }
    }

    public override string AddInternalScripts(string script, string controlObjectName, object[] inputs, IScriptEngineParams engineParams = null)
    {
        if (_scriptInternalClass == null)
        {
            _scriptInternalClass = GetResourceScriptDecrypt("Ptc.Controls.InputControls.implementations.ScriptableCheckBox.PythonCheckBoxScriptInternalClass.py");
        }
        return base.AddInternalScripts(script, controlObjectName, inputs, engineParams);
    }

    protected override void GetExistingEvents(string convertUserScript)
    {
        base.GetExistingEvents(convertUserScript);
        EventClick = ScriptClassObjectName + "_Click";
        if (!convertUserScript.Contains(EventClick.Split('(')[0]))
        {
            EventClick = string.Empty;
        }
    }

    protected override object GetScriptClassProperties(object scriptObjectProperties = null)
    {
        dynamic pyObject = _pyObject;
        object scriptClassProperties = base.GetScriptClassProperties(scriptObjectProperties);
        if (scriptClassProperties is Hashtable hashtable)
        {
            if (pyObject.Text != null && (pyObject.Text is string || pyObject.Text is DateTime || pyObject.Text != Undefined.Value))
            {
                hashtable["text"] = (object)pyObject.Text;
            }
            if (pyObject.LeftText != null && IsPythonBool(pyObject.LeftText))
            {
                hashtable["leftText"] = (object)pyObject.LeftText;
            }
            if (pyObject.Check != null && IsPythonBool(pyObject.Check))
            {
                hashtable["check"] = pyObject.Check ? 1 : 0;
            }
        }
        return scriptClassProperties;
    }

    protected override void UpdateScriptableClassValues(ref object value, object scriptControlObject, IScriptEngineParams engineParams = null)
    {
        if (scriptControlObject is Hashtable hashtable && !engineParams.IsInteractChanged && hashtable.ContainsKey("check"))
        {
            value = double.Parse(hashtable["check"].ToString(), CultureInfo.InvariantCulture);
        }
        ((dynamic)_pyObject).Check = ((double)value).Equals(1.0);
    }

    public InputCheckBoxEnginePython()
    {
    }
}
