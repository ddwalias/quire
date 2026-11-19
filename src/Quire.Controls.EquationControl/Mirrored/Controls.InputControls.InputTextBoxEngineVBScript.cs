using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.ClearScript;

namespace Quire.Controls.InputControls;

internal partial class InputTextBoxEngineVBScript
{
    private const string scriptInternalClassResourceKey = "Ptc.Controls.InputControls.implementations.ScriptableTextBox.VbTextBoxScriptInternalClass.vbs";

    private const string defaultScriptResourceKey = "Ptc.Controls.InputControls.implementations.ScriptableTextBox.VbTextBoxDefaultScript.vbs";

    protected static string _defaultScript;

    private static readonly List<Type> ValidTextTypes = new List<Type>
    {
        typeof(string),
        typeof(DateTime),
        typeof(short),
        typeof(int),
        typeof(double),
        typeof(long),
        typeof(float)
    };

    public override string Script
    {
        get
        {
            if (_defaultScript == null)
            {
                _defaultScript = GetResourceScriptDecrypt("Ptc.Controls.InputControls.implementations.ScriptableTextBox.VbTextBoxDefaultScript.vbs");
            }
            return _currentUserScript ?? _defaultScript;
        }
    }

    public override string AddInternalScripts(string script, string controlObjectName, object[] inputs, IScriptEngineParams engineParams = null)
    {
        if (_scriptInternalClass == null)
        {
            _scriptInternalClass = GetResourceScriptDecrypt("Ptc.Controls.InputControls.implementations.ScriptableTextBox.VbTextBoxScriptInternalClass.vbs");
        }
        return base.AddInternalScripts(script, controlObjectName, inputs, engineParams);
    }

    protected override void UpdateScriptableClassValues(ref object value, object scriptObjectProperties, IScriptEngineParams engineParams = null)
    {
        if (!engineParams.IsInteractChanged && scriptObjectProperties is Hashtable hashtable && hashtable.ContainsKey("text"))
        {
            value = hashtable["text"];
        }
        _scriptEngine.Script[ScriptClassObjectName].Text = value;
    }

    protected override object GetScriptClassProperties(object scriptObjectProperties = null)
    {
        dynamic val = _scriptEngine.Script[ScriptClassObjectName];
        object scriptClassProperties = base.GetScriptClassProperties(scriptObjectProperties);
        if (scriptClassProperties is Hashtable hashtable)
        {
            if (ValidTextTypes.Contains(val.text.GetType()) || val.Text != Undefined.Value)
            {
                hashtable["text"] = (object)val.Text;
            }
            if (val.BackColor is int)
            {
                hashtable["backColor"] = (object)ConvertVbColorToColor(val.BackColor);
            }
            if (val.ForeColor is int)
            {
                hashtable["foreColor"] = (object)ConvertVbColorToColor(val.ForeColor);
            }
            if (val.RightAlignedText is bool)
            {
                hashtable["textAlignment"] = (object)val.RightAlignedText;
            }
            if (val.Multiline is bool)
            {
                hashtable["acceptsReturn"] = (object)val.Multiline;
            }
            if (val.ReadOnly is bool)
            {
                hashtable["readOnly"] = (object)val.ReadOnly;
            }
            if (IsNumeric(val.Font.Size))
            {
                hashtable["fontSize"] = (object)val.Font.Size;
            }
        }
        return scriptClassProperties;
    }

    public override string GetUserScriptObjectName(string userScript, IScriptEngineParams engineParams = null)
    {
        return base.GetUserScriptObjectName(userScript, engineParams) ?? "TextBox";
    }

    public InputTextBoxEngineVBScript()
    {
    }
}
