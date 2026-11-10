using System.Collections.Generic;
using Quire.EquationEditor.Tree;

namespace Quire.Controls.InputControls;

internal partial class InputTextBoxEngineDefaultScript
{
    public InputTextBoxEngineDefaultScript()
    {
        _engineType = InputControlEngineTypes.DefaultScript;
    }

    public override bool Apply(string script, out object scriptObject, out string exception, object[] inputs, IScriptEngineParams engineParams = default)
    {
        scriptObject = string.Empty;
        exception = null;
        return true;
    }

    public override IList<object> GetCalculatedValue(string script, ref object value, out object scriptObject, out string exception, object[] inputs, IScriptEngineParams engineParams = default)
    {
        exception = null;
        scriptObject = value;
        return new List<object>(new object[] { scriptObject });
    }
}
