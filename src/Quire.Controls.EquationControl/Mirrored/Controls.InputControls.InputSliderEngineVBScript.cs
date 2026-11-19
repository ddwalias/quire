using System.Collections;
using System.Globalization;

namespace Quire.Controls.InputControls;

internal partial class InputSliderEngineVBScript
{
    private const string scriptInternalClassResourceKey = "Ptc.Controls.InputControls.implementations.ScriptableSlider.VbSliderScriptInternalClass.vbs";

    private const string defaultScriptResourceKey = "Ptc.Controls.InputControls.implementations.ScriptableSlider.VbSliderDefaultScript.vbs";

    public new const string Event_Click = "_ValueChanged()\r\n";

    protected static string _defaultScript;

    public override string Script
    {
        get
        {
            if (_defaultScript == null)
            {
                _defaultScript = GetResourceScriptDecrypt("Ptc.Controls.InputControls.implementations.ScriptableSlider.VbSliderDefaultScript.vbs");
            }
            return _currentUserScript ?? _defaultScript;
        }
    }

    public override string AddInternalScripts(string script, string controlObjectName, object[] inputs, IScriptEngineParams engineParams = null)
    {
        if (_scriptInternalClass == null)
        {
            _scriptInternalClass = GetResourceScriptDecrypt("Ptc.Controls.InputControls.implementations.ScriptableSlider.VbSliderScriptInternalClass.vbs");
        }
        return base.AddInternalScripts(script, controlObjectName, inputs, engineParams);
    }

    public override string GetUserScriptObjectName(string userScript, IScriptEngineParams engineParams = null)
    {
        return base.GetUserScriptObjectName(userScript, engineParams) ?? "Slider";
    }

    protected override void GetExistingEvents(string convertUserScript)
    {
        base.GetExistingEvents(convertUserScript);
        EventClick = ScriptClassObjectName + "_ValueChanged()\r\n";
        if (!convertUserScript.Contains(EventClick.Split('(')[0]))
        {
            EventClick = string.Empty;
        }
    }

    protected override void UpdateScriptableClassValues(ref object value, object scriptObjectProperties, IScriptEngineParams engineParams = null)
    {
        if (!engineParams.IsInteractChanged && scriptObjectProperties is Hashtable hashtable && hashtable.ContainsKey("position"))
        {
            value = double.Parse(hashtable["position"].ToString(), CultureInfo.InvariantCulture);
        }
        _scriptEngine.Script[ScriptClassObjectName].Position = value;
    }

    protected override object GetScriptClassProperties(object scriptObjectProperties = null)
    {
        dynamic val = _scriptEngine.Script[ScriptClassObjectName];
        object scriptClassProperties = base.GetScriptClassProperties(scriptObjectProperties);
        if (scriptClassProperties is Hashtable hashtable)
        {
            if (IsNumeric(val.tickfrequency))
            {
                hashtable["tickfrequency"] = (object?)val.tickfrequency;
            }
            if (IsNumeric(val.orientation))
            {
                hashtable["orientation"] = (object?)val.orientation;
            }
            if (IsNumeric(val.minrange))
            {
                hashtable["minrange"] = (object?)val.minrange;
            }
            if (IsNumeric(val.maxrange))
            {
                hashtable["maxrange"] = (object?)val.maxrange;
            }
            if (IsNumeric(val.pagesize))
            {
                hashtable["pagesize"] = (object?)val.pagesize;
            }
            if (IsNumeric(val.linesize))
            {
                hashtable["linesize"] = (object?)val.linesize;
            }
            if (val.ShowMinMax is bool)
            {
                hashtable["showminmax"] = (object?)val.showminmax;
            }
            if (val.Invert is bool)
            {
                hashtable["invert"] = (object?)val.invert;
            }
            if (val.ticks is bool)
            {
                hashtable["ticks"] = (object?)val.ticks;
            }
            if (val.Border is bool)
            {
                hashtable["border"] = (object?)val.Border;
            }
            if (val.autoticks is bool)
            {
                hashtable["autoticks"] = (object?)val.autoticks;
            }
            if (val.point is short)
            {
                hashtable["point"] = (object?)val.point;
            }
            if (IsNumeric(val.position))
            {
                hashtable["position"] = (object?)val.position;
            }
        }
        return scriptClassProperties;
    }

    public InputSliderEngineVBScript()
    {
    }
}
