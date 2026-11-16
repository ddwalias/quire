using System.Collections;
using System.Collections.Generic;
using Quire.EquationEditor.Tree;

namespace Quire.Controls.InputControls;

internal partial class InputRadioButtonEngineDefaultScript
{
    private const string defaultScriptValues = "One\vTwo\vThree";

    public InputRadioButtonEngineDefaultScript()
    {
        _engineType = InputControlEngineTypes.DefaultScript;
    }

    public override bool Apply(string userScript, out object scriptObject, out string exception, object[] inputs, IScriptEngineParams engineParams = null)
    {
        Hashtable hashtable = new Hashtable();
        hashtable["items"] = "One\vTwo\vThree";
        hashtable["CurSel"] = 0;
        scriptObject = hashtable;
        exception = null;
        return true;
    }

    public override IList<object> GetCalculatedValue(string script, ref object value, out object scriptObject, out string exception, object[] inputs, IScriptEngineParams engineParams = null)
    {
        exception = null;
        scriptObject = null;
        return new List<object>(new object[1] { double.Parse(value.ToString()) + 1.0 });
    }
}
