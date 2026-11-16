using Quire.Core;

namespace Quire.Controls.InputControls;

public abstract partial class PrimeControlClearScriptEngine
{
    public override bool CanExecEvent(string eventCommand)
    {
        return IsScriptingItemExists(ScriptClassObjectName + "_" + eventCommand);
    }

    protected override bool ExecuteScript(string script)
    {
        _scriptEngine.Execute(script);
        return true;
    }

    protected override bool CanExecuteScript(out string exception)
    {
        exception = null;
        return true;
    }

    protected override void RunEvent(string eventName)
    {
        if (eventName.IsNotEmpty())
        {
            _scriptEngine.Execute(eventName);
        }
    }

    protected override void RunEventWithInputsOutputs(string eventName)
    {
        RunEvent(eventName);
    }

    protected PrimeControlClearScriptEngine()
    {
    }
}
