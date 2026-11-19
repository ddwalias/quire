using System.Collections;
using System.Globalization;

namespace Quire.Controls.InputControls;

internal partial class InputSliderEnginePython
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

    public const string orientation = "orientation";

    public const string position = "position";

    public new const string Event_Click = "_ValueChanged";

    private const string scriptInternalClassResourceKey = "Ptc.Controls.InputControls.implementations.ScriptableSlider.PythonSliderScriptInternalClass.py";

    private const string defaultScriptResourceKey = "Ptc.Controls.InputControls.implementations.ScriptableSlider.PythonSliderDefaultScript.py";

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
                _defaultScript = GetResourceScriptDecrypt("Ptc.Controls.InputControls.implementations.ScriptableSlider.PythonSliderDefaultScript.py");
            }
            return _currentUserScript ?? _defaultScript;
        }
    }

    public override string AddInternalScripts(string script, string controlObjectName, object[] inputs, IScriptEngineParams engineParams = null)
    {
        if (_scriptInternalClass == null)
        {
            _scriptInternalClass = GetResourceScriptDecrypt("Ptc.Controls.InputControls.implementations.ScriptableSlider.PythonSliderScriptInternalClass.py");
        }
        return base.AddInternalScripts(script, controlObjectName, inputs, engineParams);
    }

    protected override void UpdateScriptableClassValues(ref object value, object scriptObjectProperties, IScriptEngineParams engineParams = null)
    {
        if (!engineParams.IsInteractChanged && scriptObjectProperties is Hashtable hashtable && hashtable.ContainsKey("position"))
        {
            value = double.Parse(hashtable["position"].ToString(), CultureInfo.InvariantCulture);
        }
        ((dynamic)_pyObject).Position = value;
    }

    protected override void GetExistingEvents(string convertUserScript)
    {
        base.GetExistingEvents(convertUserScript);
        EventClick = ScriptClassObjectName + "_ValueChanged";
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
            if (pyObject.TickFrequency != null && IsPythonNumber(pyObject.TickFrequency))
            {
                hashtable["tickfrequency"] = (object?)pyObject.TickFrequency;
            }
            if (pyObject.Orientation != null && IsPythonInt(pyObject.Orientation))
            {
                hashtable["orientation"] = (object?)pyObject.Orientation;
            }
            if (pyObject.MinRange != null && IsPythonNumber(pyObject.MinRange))
            {
                hashtable["minrange"] = (object?)pyObject.MinRange;
            }
            if (pyObject.MaxRange != null && IsPythonNumber(pyObject.MaxRange))
            {
                hashtable["maxrange"] = (object?)pyObject.MaxRange;
            }
            if (pyObject.ShowMinMax != null && IsPythonBool(pyObject.ShowMinMax))
            {
                hashtable["showminmax"] = (object?)pyObject.ShowMinMax;
            }
            if (pyObject.Invert != null && IsPythonBool(pyObject.Invert))
            {
                hashtable["invert"] = (object?)pyObject.Invert;
            }
            if (pyObject.PageSize != null && IsPythonNumber(pyObject.PageSize))
            {
                hashtable["pagesize"] = (object?)pyObject.PageSize;
            }
            if (pyObject.LineSize != null && IsPythonNumber(pyObject.LineSize))
            {
                hashtable["linesize"] = (object?)pyObject.LineSize;
            }
            if (pyObject.Ticks != null && IsPythonBool(pyObject.Ticks))
            {
                hashtable["ticks"] = (object?)pyObject.Ticks;
            }
            if (pyObject.Border != null && IsPythonBool(pyObject.Border))
            {
                hashtable["border"] = (object?)pyObject.Border;
            }
            if (pyObject.AutoTicks != null && IsPythonBool(pyObject.AutoTicks))
            {
                hashtable["autoticks"] = (object?)pyObject.AutoTicks;
            }
            if (pyObject.Point != null && IsPythonInt(pyObject.Point))
            {
                hashtable["point"] = (object?)pyObject.Point;
            }
            if (pyObject.Position != null && IsPythonNumber(pyObject.Position))
            {
                hashtable["position"] = (object?)pyObject.Position;
            }
        }
        return scriptClassProperties;
    }

    public override void Dispose()
    {
    }

    public InputSliderEnginePython()
    {
    }
}
