using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Quire.EquationEditor.Tree;

namespace Quire.Controls.InputControls;

internal partial class InputListBoxEnginePython
{
    public const char ListBoxSplitCharForItems = '\v';

    public const string DefaultUserScriptObjectName = "ListBox";

    public const string rightAlignedText = "rightAlignedText";

    public new const string Event_Click = "_SelChanged";

    private const string defaultScriptResourceKey = "Ptc.Controls.InputControls.implementations.ScriptableListBox.PythonListBoxDefaultScript.py";

    private const string scriptInternalClassResourceKey = "Ptc.Controls.InputControls.implementations.ScriptableListBox.PythonListBoxScriptInternalClass.py";

    protected static string _defaultScript;

    public override string AddInternalScripts(string script, string controlObjectName, object[] inputs, IScriptEngineParams engineParams = null)
    {
        if (_scriptInternalClass == null)
        {
            _scriptInternalClass = GetResourceScriptDecrypt("Ptc.Controls.InputControls.implementations.ScriptableListBox.PythonListBoxScriptInternalClass.py");
        }
        return base.AddInternalScripts(script, controlObjectName, inputs, engineParams);
    }

    public override string ScriptInternalClassInit(string controlObjectName, IScriptEngineParams engineParams = null)
    {
        ListBoxScriptableControlData listBoxScriptableControlData = engineParams.Data as ListBoxScriptableControlData;
        return "\r\n" + controlObjectName + ".AddString(\"" + string.Join('\v'.ToString(), listBoxScriptableControlData.Values) + "\")\r\n" + controlObjectName + ".CurSel = " + listBoxScriptableControlData.SelectedValueIndex + "\r\n";
    }

    protected override void GetExistingEvents(string convertUserScript)
    {
        base.GetExistingEvents(convertUserScript);
        EventClick = ScriptClassObjectName + "_SelChanged";
        if (!convertUserScript.Contains(EventClick.Split('(')[0]))
        {
            EventClick = string.Empty;
        }
    }

    protected override void UpdateScriptableClassValues(ref object value, object scriptControlObject, IScriptEngineParams engineParams = null)
    {
        bool flag = ((ListBoxScriptableControlData)engineParams.Data).Sort;
        if (scriptControlObject is Hashtable hashtable)
        {
            if (!engineParams.IsInteractChanged && hashtable.ContainsKey("CurSel") && IsPythonInt(hashtable["CurSel"]))
            {
                value = double.Parse(hashtable["CurSel"].ToString(), CultureInfo.InvariantCulture);
            }
            if (hashtable.ContainsKey("sort") && IsPythonBool(hashtable["sort"]))
            {
                flag = bool.Parse(hashtable["sort"].ToString());
            }
        }
        ((dynamic)_pyObject).Sort = flag;
        ((dynamic)_pyObject).CurSel = int.Parse(value.ToString(), CultureInfo.InvariantCulture);
        if (((dynamic)_pyObject).Sort)
        {
            ((dynamic)_pyObject).SortedItems = string.Join('\v'.ToString(), new SortedSet<string>(((ListBoxScriptableControlData)engineParams.Data).Values));
        }
    }

    protected override object GetScriptClassProperties(object scriptObjectProperties = null)
    {
        dynamic pyObject = _pyObject;
        object scriptClassProperties = base.GetScriptClassProperties(scriptObjectProperties);
        if (scriptClassProperties is Hashtable hashtable)
        {
            if (pyObject.CurSel != null && IsPythonInt(pyObject.CurSel))
            {
                hashtable["CurSel"] = (object?)pyObject.CurSel;
            }
            if (pyObject.Sort != null && IsPythonBool(pyObject.Sort))
            {
                hashtable["sort"] = (object?)pyObject.Sort;
            }
            if (pyObject.RightAlignedText != null && IsPythonBool(pyObject.RightAlignedText))
            {
                hashtable["rightAlignedText"] = (object?)pyObject.RightAlignedText;
            }
            hashtable["items"] = (object?)pyObject.items;
        }
        return scriptClassProperties;
    }

    public override string GetUserScriptObjectName(string userScript, IScriptEngineParams engineParams = null)
    {
        return base.GetUserScriptObjectName(userScript, engineParams) ?? "ListBox";
    }

    public override string Script
    {
        get
        {
            if (_defaultScript == null)
            {
                _defaultScript = GetResourceScriptDecrypt("Ptc.Controls.InputControls.implementations.ScriptableListBox.PythonListBoxDefaultScript.py");
            }
            return _currentUserScript ?? _defaultScript;
        }
    }

    public InputListBoxEnginePython()
    {
    }
}
