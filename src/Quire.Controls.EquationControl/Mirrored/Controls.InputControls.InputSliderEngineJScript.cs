using System.Collections;
using System.Globalization;

namespace Quire.Controls.InputControls;

internal partial class InputSliderEngineJScript
{
    public const string DefaultUserScriptObjectName = "Slider";

    public const string tickfrequency = "tickfrequency";

    public const string minrange = "minrange";

    public const string maxrange = "maxrange";

    public const string showminmax = "showminmax";

    public const string invert = "invert";

    public const string pagesize = "pagesize";

    public const string linesize = "linesize";

    public const string ticks = "ticks";

    public const string border = "border";

    public const string autoticks = "autoticks";

    public const string point = "point";

    public const string numTicks = "numticks";

    public const string orientation = "orientation";

    public const string position = "position";

    public new const string Event_Click = "_ValueChanged(Inputs, Outputs)\r\n";

    private const string scriptInternalClassResourceKey = "Ptc.Controls.InputControls.implementations.ScriptableSlider.JsSliderScriptInternalClass.js";

    private const string defaultScriptResourceKey = "Ptc.Controls.InputControls.implementations.ScriptableSlider.JsSliderDefaultScript.js";

    protected static string _defaultScript;

    public override string GetUserScriptObjectName(string userScript, IScriptEngineParams engineParams = null)
    {
        return base.GetUserScriptObjectName(userScript, engineParams) ?? "Slider";
    }

    public override string Script
    {
        get
        {
            if (_defaultScript == null)
            {
                _defaultScript = GetResourceScriptDecrypt("Ptc.Controls.InputControls.implementations.ScriptableSlider.JsSliderDefaultScript.js");
            }
            return _currentUserScript ?? _defaultScript;
        }
    }

    public override string AddInternalScripts(string script, string controlObjectName, object[] inputs, IScriptEngineParams engineParams = null)
    {
        if (_scriptInternalClass == null)
        {
            _scriptInternalClass = GetResourceScriptDecrypt("Ptc.Controls.InputControls.implementations.ScriptableSlider.JsSliderScriptInternalClass.js");
        }
        return base.AddInternalScripts(script, controlObjectName, inputs, engineParams);
    }

    protected override void UpdateScriptableClassValues(ref object value, object scriptObjectProperties, IScriptEngineParams engineParams = null)
    {
        if (!engineParams.IsInteractChanged && scriptObjectProperties is Hashtable hashtable && hashtable.ContainsKey("position"))
        {
            value = double.Parse(hashtable["position"].ToString(), CultureInfo.InvariantCulture);
        }
        _scriptEngine.Script[ScriptClassObjectName].Position(value);
    }

    protected override void GetExistingEvents(string convertUserScript)
    {
        base.GetExistingEvents(convertUserScript);
        EventClick = ScriptClassObjectName + "_ValueChanged(Inputs, Outputs)\r\n";
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
            if (IsNumeric(val.TickFrequency()))
            {
                hashtable["tickfrequency"] = (object?)val.TickFrequency();
            }
            if (val.Orientation() is int)
            {
                hashtable["orientation"] = (object?)val.Orientation();
            }
            if (IsNumeric(val.MinRange()))
            {
                hashtable["minrange"] = (object?)val.MinRange();
            }
            if (IsNumeric(val.MaxRange()))
            {
                hashtable["maxrange"] = (object?)val.MaxRange();
            }
            if (IsNumeric(val.PageSize()))
            {
                hashtable["pagesize"] = (object?)val.PageSize();
            }
            if (IsNumeric(val.LineSize()))
            {
                hashtable["linesize"] = (object?)val.LineSize();
            }
            if (val.ShowMinMax() is bool)
            {
                hashtable["showminmax"] = (object?)val.ShowMinMax();
            }
            if (val.Invert() is bool)
            {
                hashtable["invert"] = (object?)val.Invert();
            }
            if (val.Ticks() is bool)
            {
                hashtable["ticks"] = (object?)val.Ticks();
            }
            if (val.Border() is bool)
            {
                hashtable["border"] = (object?)val.Border();
            }
            if (val.AutoTicks() is bool)
            {
                hashtable["autoticks"] = (object?)val.AutoTicks();
            }
            if (val.Point() is int)
            {
                hashtable["point"] = (object?)val.Point();
            }
            if (IsNumeric(val.Position()))
            {
                hashtable["position"] = (object?)val.Position();
            }
        }
        return scriptClassProperties;
    }

    public InputSliderEngineJScript()
    {
    }
}
