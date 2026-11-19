using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Quire.EquationEditor.Tree;

namespace Quire.Controls.InputControls;

internal partial class InputListBoxEngineJScript
{
    public const char ListBoxSplitCharForItems = '\v';

    public const string DefaultUserScriptObjectName = "ListBox";

    public const string sort = "sort";

    public const string curSel = "CurSel";

    public const string rightAlignedText = "rightAlignedText";

    public const string items = "items";

    private string _itemsCache;

    public new const string Event_Click = "_SelChanged(Inputs, Outputs)\r\n";

    private const string defaultScriptResourceKey = "Ptc.Controls.InputControls.implementations.ScriptableListBox.JsListBoxDefaultScript.js";

    private const string scriptInternalClassResourceKey = "Ptc.Controls.InputControls.implementations.ScriptableListBox.JsListBoxScriptInternalClass.js";

    protected static string _defaultScript;

    public override string AddInternalScripts(string script, string controlObjectName, object[] inputs, IScriptEngineParams engineParams = null)
    {
        if (_scriptInternalClass == null)
        {
            _scriptInternalClass = GetResourceScriptDecrypt("Ptc.Controls.InputControls.implementations.ScriptableListBox.JsListBoxScriptInternalClass.js");
        }
        return base.AddInternalScripts(script, controlObjectName, inputs, engineParams);
    }

    public override string ScriptInternalClassInit(string controlObjectName, IScriptEngineParams engineParams = null)
    {
        ListBoxScriptableControlData listBoxScriptableControlData = engineParams.Data as ListBoxScriptableControlData;
        return "\r\n" + controlObjectName + ".AddString(\"" + string.Join('\v'.ToString(), listBoxScriptableControlData.Values) + "\");\r\n" + controlObjectName + ".CurSel(" + listBoxScriptableControlData.SelectedValueIndex + ");\r\n";
    }

    protected override void GetExistingEvents(string convertUserScript)
    {
        base.GetExistingEvents(convertUserScript);
        EventClick = ScriptClassObjectName + "_SelChanged(Inputs, Outputs)\r\n";
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
            if (!engineParams.IsInteractChanged && hashtable.ContainsKey("CurSel") && hashtable["CurSel"] is int)
            {
                value = double.Parse(hashtable["CurSel"].ToString(), CultureInfo.InvariantCulture);
            }
            if (hashtable.ContainsKey("sort") && hashtable["sort"] is bool)
            {
                flag = bool.Parse(hashtable["sort"].ToString());
            }
        }
        if (flag)
        {
            _scriptEngine.Script[ScriptClassObjectName].sortedItems(string.Join('\v'.ToString(), new SortedSet<string>(((ListBoxScriptableControlData)engineParams.Data).Values)));
        }
        _scriptEngine.Script[ScriptClassObjectName].Sort(flag);
        _scriptEngine.Script[ScriptClassObjectName].CurSel(value);
    }

    protected override object GetScriptClassProperties(object scriptObjectProperties = null)
    {
        dynamic val = _scriptEngine.Script[ScriptClassObjectName];
        object scriptClassProperties = base.GetScriptClassProperties(scriptObjectProperties);
        if (scriptClassProperties is Hashtable hashtable)
        {
            if (val.CurSel() is int)
            {
                hashtable["CurSel"] = (object?)val.CurSel();
            }
            if (val.Sort() is bool)
            {
                hashtable["sort"] = (object?)val.Sort();
            }
            if (val.RightAlignedText() is bool)
            {
                hashtable["rightAlignedText"] = (object?)val.RightAlignedText();
            }
            hashtable["items"] = (object?)val.items();
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
                _defaultScript = GetResourceScriptDecrypt("Ptc.Controls.InputControls.implementations.ScriptableListBox.JsListBoxDefaultScript.js");
            }
            return _currentUserScript ?? _defaultScript;
        }
    }

    public InputListBoxEngineJScript()
    {
    }
}
