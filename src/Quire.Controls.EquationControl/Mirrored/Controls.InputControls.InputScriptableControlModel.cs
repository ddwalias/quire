using System;
using System.ComponentModel;
using Quire.EquationEditor.Tree;

namespace Quire.Controls.InputControls;

public partial class InputScriptableControlModel
{
    protected string _script;

    protected new IInputScriptableControlData Data => (IInputScriptableControlData)_data;

    protected IInputControlScriptingEngine scriptableEngine => (IInputControlScriptingEngine)InputEngine;

    public InputScriptableControlModel(IInputControlEngineFactory factory, IInputControlDataCalculatedValue data)
        : base(factory, data)
    {
        Init();
    }

    protected virtual void Init()
    {
        _script = StringCompressor.DecompressString(Data.Script);
    }

    public virtual string DefaultScript(InputControlEngineTypes engineType)
    {
        return string.Empty;
    }

    ~InputScriptableControlModel()
    {
        Dispose();
    }

    public virtual object SelectedValue { get; }

    public virtual bool Apply(string userScript, out object scriptObject, out string exception, object[] inputs, IScriptEngineParams engineParams = null)
    {
        Script = userScript;
        IsDirty = true;
        Data.IsNeedsCalculate = false;
        scriptObject = null;
        exception = null;
        bool flag = false;
        if (IsEnabled && IsFunctionalityEnabled)
        {
            if (InputEngine == null || Data.EngineType != InputEngine.EngineType)
            {
                CreateEngine(EngineType);
            }
            flag = scriptableEngine.Apply(userScript, out scriptObject, out exception, inputs, engineParams);
            if (flag)
            {
                object valueIndex = SelectedValue;
                HandlePostInputFieldResultChanged(ref valueIndex, scriptObject, exception);
                _data.SelectedCalculatedValue = scriptableEngine.GetCalculatedValue(userScript, ref valueIndex, out scriptObject, out exception, inputs, engineParams);
                HandlePostInputFieldResultChanged(ref valueIndex, scriptObject, exception);
            }
        }
        if (flag)
        {
            return exception == null;
        }
        return false;
    }

    public string Script
    {
        get
        {
            return _script;
        }
        set
        {
            _script = value;
            Data.Script = StringCompressor.CompressString(value);
        }
    }

    public override InputControlEngineTypes EngineType
    {
        get
        {
            return base.EngineType;
        }
        set
        {
            base.EngineType = value;
            Script = ((IInputControlScriptingEngine)InputEngine).Script;
        }
    }

    public override bool AutoRecalculate
    {
        get
        {
            return Data.IsNeedsCalculate;
        }
        set
        {
            Data.IsNeedsCalculate = value;
        }
    }

    public void HandlePostError(string error)
    {
        EngineError = error;
        OnPropertyChanged("EngineError");
    }
}
