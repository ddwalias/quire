using System.Collections;
using System.Collections.Generic;
using Quire.EquationEditor.Tree;

namespace Quire.Controls.InputControls;

internal partial class InputButtonEngineDefaultScript
{
    public InputButtonEngineDefaultScript()
    {
        _engineType = InputControlEngineTypes.DefaultScript;
    }

    public override bool Apply(string userScript, out object scriptObject, out string exception, object[] inputs, IScriptEngineParams engineParams = default)
    {
        Hashtable hashtable = new Hashtable();
        hashtable["text"] = "Click Here";
        scriptObject = hashtable;
        exception = null;
        return true;
    }

    public override IList<object> GetCalculatedValue(string userScript, ref object value, out object scriptControlObject, out string exception, object[] inputs, IScriptEngineParams engineParams = default)
    {
        exception = null;
        scriptControlObject = null;
        return new List<object>(new object[] { value });
    }
}
