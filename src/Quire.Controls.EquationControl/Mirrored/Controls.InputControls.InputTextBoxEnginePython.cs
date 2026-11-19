using System.Collections;
using System.Text.RegularExpressions;

namespace Quire.Controls.InputControls;

internal partial class InputTextBoxEnginePython
{
    public const string text = "text";

    public const string backColor = "backColor";

    public const string foreColor = "foreColor";

    public const string fontSize = "fontSize";

    public const string readOnly = "readOnly";

    public const string textAlignment = "textAlignment";

    public const string acceptsReturn = "acceptsReturn";

    public const string DefaultUserScriptObjectName = "TextBox";

    private const string HexColorRegexPattern = "^#(?:[0-9A-Fa-f]{3}){1,2}$";

    private const string scriptInternalClassResourceKey = "Ptc.Controls.InputControls.implementations.ScriptableTextBox.PythonTextBoxScriptInternalClass.py";

    private const string defaultScriptResourceKey = "Ptc.Controls.InputControls.implementations.ScriptableTextBox.PythonTextBoxDefaultScript.py";

    protected static string _defaultScript;

    public override string GetUserScriptObjectName(string userScript, IScriptEngineParams engineParams = null)
    {
        return base.GetUserScriptObjectName(userScript, engineParams) ?? "TextBox";
    }

    public override string Script
    {
        get
        {
            if (_defaultScript == null)
            {
                _defaultScript = GetResourceScriptDecrypt("Ptc.Controls.InputControls.implementations.ScriptableTextBox.PythonTextBoxDefaultScript.py");
            }
            return _currentUserScript ?? _defaultScript;
        }
    }

    public override string AddInternalScripts(string script, string controlObjectName, object[] inputs, IScriptEngineParams engineParams = null)
    {
        if (_scriptInternalClass == null)
        {
            _scriptInternalClass = GetResourceScriptDecrypt("Ptc.Controls.InputControls.implementations.ScriptableTextBox.PythonTextBoxScriptInternalClass.py");
        }
        return base.AddInternalScripts(script, controlObjectName, inputs, engineParams);
    }

    protected override void UpdateScriptableClassValues(ref object value, object scriptObjectProperties, IScriptEngineParams engineParams = null)
    {
        if (!engineParams.IsInteractChanged && scriptObjectProperties is Hashtable hashtable && hashtable.ContainsKey("text"))
        {
            value = hashtable["text"];
        }
        ((dynamic)_pyObject).Text = value;
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
            if (pyObject.BackColor != null && IsPythonString(pyObject.BackColor) && IsHexColor(pyObject.BackColor.ToString()))
            {
                hashtable["backColor"] = (object)pyObject.BackColor;
            }
            if (pyObject.ForeColor != null && IsPythonString(pyObject.ForeColor) && IsHexColor(pyObject.ForeColor.ToString()))
            {
                hashtable["foreColor"] = (object)pyObject.ForeColor;
            }
            if (pyObject.RightAlignedText != null && IsPythonBool(pyObject.RightAlignedText))
            {
                hashtable["textAlignment"] = (object)pyObject.RightAlignedText;
            }
            if (pyObject.Multiline != null && IsPythonBool(pyObject.Multiline))
            {
                hashtable["acceptsReturn"] = (object)pyObject.Multiline;
            }
            if (pyObject.ReadOnly != null && IsPythonBool(pyObject.ReadOnly))
            {
                hashtable["readOnly"] = (object)pyObject.ReadOnly;
            }
            if (pyObject.Font.Size != null && IsPythonNumber(pyObject.Font.Size))
            {
                hashtable["fontSize"] = (object)pyObject.Font.Size;
            }
        }
        return scriptClassProperties;
    }

    public override void Dispose()
    {
    }

    public bool IsHexColor(string input)
    {
        return Regex.IsMatch(input, "^#(?:[0-9A-Fa-f]{3}){1,2}$");
    }

    public InputTextBoxEnginePython()
    {
    }
}
