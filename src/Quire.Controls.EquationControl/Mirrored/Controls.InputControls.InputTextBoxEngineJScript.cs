using System.Collections;

namespace Quire.Controls.InputControls;

internal partial class InputTextBoxEngineJScript
{
    public const string text = "text";

    public const string backColor = "backColor";

    public const string foreColor = "foreColor";

    public const string fontSize = "fontSize";

    public const string readOnly = "readOnly";

    public const string textAlignment = "textAlignment";

    public const string acceptsReturn = "acceptsReturn";

    public const string DefaultUserScriptObjectName = "TextBox";

    private const string scriptInternalClassResourceKey = "Ptc.Controls.InputControls.implementations.ScriptableTextBox.JsTextBoxScriptInternalClass.js";

    private const string defaultScriptResourceKey = "Ptc.Controls.InputControls.implementations.ScriptableTextBox.JsTextBoxDefaultScript.js";

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
                _defaultScript = GetResourceScriptDecrypt("Ptc.Controls.InputControls.implementations.ScriptableTextBox.JsTextBoxDefaultScript.js");
            }
            return _currentUserScript ?? _defaultScript;
        }
    }

    public override string AddInternalScripts(string script, string controlObjectName, object[] inputs, IScriptEngineParams engineParams = null)
    {
        if (_scriptInternalClass == null)
        {
            _scriptInternalClass = GetResourceScriptDecrypt("Ptc.Controls.InputControls.implementations.ScriptableTextBox.JsTextBoxScriptInternalClass.js");
        }
        return base.AddInternalScripts(script, controlObjectName, inputs, engineParams);
    }

    protected override void UpdateScriptableClassValues(ref object value, object scriptObjectProperties, IScriptEngineParams engineParams = null)
    {
        if (!engineParams.IsInteractChanged && scriptObjectProperties is Hashtable hashtable && hashtable.ContainsKey("text"))
        {
            value = hashtable["text"];
        }
        _scriptEngine.Script[ScriptClassObjectName].Text(value);
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
            if (val.BackColor() is int)
            {
                hashtable["backColor"] = (object)ConvertJsColorToColor(val.BackColor());
            }
            if (val.ForeColor() is int)
            {
                hashtable["foreColor"] = (object)ConvertJsColorToColor(val.ForeColor());
            }
            if (val.RightAlignedText() is bool)
            {
                hashtable["textAlignment"] = (object)val.RightAlignedText();
            }
            if (val.Multiline() is bool)
            {
                hashtable["acceptsReturn"] = (object)val.Multiline();
            }
            if (val.ReadOnly() is bool)
            {
                hashtable["readOnly"] = (object)val.ReadOnly();
            }
            if (IsNumeric(val.FontSize()))
            {
                hashtable["fontSize"] = (object)val.FontSize();
            }
            if (IsNumeric(val.Font.Size))
            {
                hashtable["fontSize"] = (object)val.Font.Size;
            }
        }
        return scriptClassProperties;
    }

    public InputTextBoxEngineJScript()
    {
    }
}
