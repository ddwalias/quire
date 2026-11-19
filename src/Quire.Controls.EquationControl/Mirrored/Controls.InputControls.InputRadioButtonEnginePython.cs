using System.Collections;
using System.Globalization;

namespace Quire.Controls.InputControls;

internal partial class InputRadioButtonEnginePython
{
    public const string DefaultUserScriptObjectName = "RadioBtn";

    protected static string _defaultScript;

    public new const string Event_Click = "_SelectionChange";

    public const string leftText = "leftText";

    public const string orientation = "orientation";

    private const string defaultScriptResourceKey = "Ptc.Controls.InputControls.implementations.ScriptableRadioButton.PythonRadioButtonDefaultScript.py";

    private const string scriptInternalClassResourceKey = "Ptc.Controls.InputControls.implementations.ScriptableRadioButton.PythonRadioButtonScriptInternalClass.py";

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
                _defaultScript = GetResourceScriptDecrypt("Ptc.Controls.InputControls.implementations.ScriptableRadioButton.PythonRadioButtonDefaultScript.py");
            }
            return _currentUserScript ?? _defaultScript;
        }
    }

    public override string AddInternalScripts(string script, string controlObjectName, object[] inputs, IScriptEngineParams engineParams = null)
    {
        if (_scriptInternalClass == null)
        {
            _scriptInternalClass = GetResourceScriptDecrypt("Ptc.Controls.InputControls.implementations.ScriptableRadioButton.PythonRadioButtonScriptInternalClass.py");
        }
        return base.AddInternalScripts(script, controlObjectName, inputs, engineParams);
    }

    protected override void UpdateScriptableClassValues(ref object value, object scriptControlObject, IScriptEngineParams engineParams = null)
    {
        if (scriptControlObject is Hashtable hashtable && !engineParams.IsInteractChanged && hashtable.ContainsKey("CurSel") && IsPythonNumber(hashtable["CurSel"]))
        {
            value = double.Parse(hashtable["CurSel"].ToString(), CultureInfo.InvariantCulture);
        }
        ((dynamic)_pyObject).CurSel = int.Parse(value.ToString(), CultureInfo.InvariantCulture);
    }

    protected override object GetScriptClassProperties(object scriptObjectProperties = null)
    {
        dynamic pyObject = _pyObject;
        object scriptClassProperties = base.GetScriptClassProperties(scriptObjectProperties);
        if (scriptClassProperties is Hashtable hashtable)
        {
            if (pyObject.Orientation != null && IsPythonInt(pyObject.Orientation))
            {
                hashtable["orientation"] = (object)pyObject.Orientation;
            }
            if (pyObject.CurSel != null && IsPythonInt(pyObject.CurSel))
            {
                hashtable["CurSel"] = (object)pyObject.CurSel;
            }
            if (pyObject.LeftText != null && IsPythonBool(pyObject.LeftText))
            {
                hashtable["leftText"] = (object)pyObject.LeftText;
            }
            if (pyObject.items != null)
            {
                hashtable["items"] = (object)pyObject.items;
            }
        }
        return scriptClassProperties;
    }

    protected override void GetExistingEvents(string convertUserScript)
    {
        base.GetExistingEvents(convertUserScript);
        EventClick = ScriptClassObjectName + "_SelectionChange";
        if (!convertUserScript.Contains(EventClick.Split('(')[0]))
        {
            EventClick = string.Empty;
        }
    }

    public InputRadioButtonEnginePython()
    {
    }
}
