using System.Collections;

namespace Quire.Controls.InputControls;

internal partial class InputButtonEnginePython
{
    public const string DefaultUserScriptObjectName = "PushBtn";

    public const string text = "text";

    private const string defaultScriptResourceKey = "Ptc.Controls.InputControls.implementations.ScriptableButton.PythonButtonDefaultScript.py";

    private const string scriptInternalClassResourceKey = "Ptc.Controls.InputControls.implementations.ScriptableButton.PythonButtonScriptInternalClass.py";

    protected static string _defaultScript;

    public override string Script
    {
        get
        {
            if (_defaultScript == null)
            {
                _defaultScript = GetResourceScriptDecrypt("Ptc.Controls.InputControls.implementations.ScriptableButton.PythonButtonDefaultScript.py");
            }
            return _currentUserScript ?? _defaultScript;
        }
    }

    public override string AddInternalScripts(string script, string controlObjectName, object[] inputs, IScriptEngineParams engineParams = null)
    {
        if (_scriptInternalClass == null)
        {
            _scriptInternalClass = GetResourceScriptDecrypt("Ptc.Controls.InputControls.implementations.ScriptableButton.PythonButtonScriptInternalClass.py");
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
            if (pyObject.Text != null)
            {
                hashtable["text"] = (object)pyObject.Text;
            }
        }
        return scriptClassProperties;
    }

    protected override void UpdateScriptableClassValues(ref object value, object scriptControlObject, IScriptEngineParams engineParams = null)
    {
    }

    public InputButtonEnginePython()
    {
    }
}
