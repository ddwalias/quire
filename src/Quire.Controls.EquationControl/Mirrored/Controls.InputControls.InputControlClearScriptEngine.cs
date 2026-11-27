using System;
using System.Windows;
using Microsoft.ClearScript;
using Microsoft.ClearScript.Windows;

namespace Quire.Controls.InputControls;

public abstract partial class InputControlClearScriptEngine
{
    internal partial class ClearScriptEngineWindow : IHostWindow
    {
        public nint OwnerHandle => IntPtr.Zero;

        public void EnableModeless(bool enable)
        {
        }

        public ClearScriptEngineWindow()
        {
        }
    }

    protected const string ClearScriptError = "'Microsoft.ClearScript.Undefined'";
    protected const string ClearScriptLineError = "[unknown]:";
    protected WindowsScriptEngine _scriptEngine;
    protected int _linesCountErrorCurrection;
    private const int MinPropertiesHashSize = 7;

    protected override bool ShouldEngineInit()
    {
        if (_scriptEngine != null)
        {
            return ((string[])_scriptEngine.Script.PropertyNames).Length < MinPropertiesHashSize;
        }
        return true;
    }

    protected override string ParseScriptEngineException(Exception ex)
    {
        if (ex is ScriptEngineException ex2)
        {
            return ParseClearScriptEngineException(ex2);
        }
        return ParseClearScriptEngineGenericException(ex);
    }

    protected string ParseClearScriptEngineGenericException(Exception ex)
    {
        return ex.Message.Replace(ClearScriptError, "Script");
    }

    protected string ParseClearScriptEngineException(ScriptEngineException ex)
    {
        string[] array = ex.ErrorDetails.Split(new string[2] { ClearScriptLineError, "\n" }, StringSplitOptions.None);
        if (array.Length > 1)
        {
            int num = 1;
            if (array.Length > 2)
            {
                num = 2;
            }
            string[] errorLocation = array[num].Split(':');
            errorLocation[0] = Math.Max(int.Parse(errorLocation[0]) - _linesCountErrorCurrection / 2 + 1, 1).ToString();
            return array[0] + array[1] + errorLocation[0] + ":" + errorLocation[1];
        }
        return ex.ErrorDetails;
    }

    protected bool IsScriptingItemExists(string item)
    {
        if (_scriptEngine != null && _scriptEngine.Script != null)
        {
            item = item.Split('(')[0];
            foreach (string value in _scriptEngine.Script.PropertyNames)
            {
                if (item.Equals(value))
                {
                    return true;
                }
            }
        }
        return false;
    }

    protected override void CreateScriptEngine(ScriptEngines clearScriptEngine)
    {
        if (_scriptEngine != null)
        {
            _scriptEngine.Dispose();
        }
        if (clearScriptEngine == ScriptEngines.VBScript)
        {
            _scriptEngine = new VBScriptEngine();
        }
        else
        {
            _scriptEngine = new JScriptEngine();
        }
        _scriptEngine.AddHostType("MessageBox", typeof(MessageBox));
        _scriptEngine.AddHostType("Console", typeof(Console));
        _scriptEngine.AddHostObject("host", new HostFunctions());
        _scriptEngine.AddHostObject("lib", new HostTypeCollection("mscorlib", "System.Core"));
        _scriptEngine.HostWindow = new ClearScriptEngineWindow();
    }

    public override void Dispose()
    {
        _scriptEngine?.Dispose();
    }

    protected InputControlClearScriptEngine()
    {
    }
}
