using System.Collections;
using System.Collections.ObjectModel;
using Quire.EquationEditor.Tree;

namespace Quire.Controls.InputControls;

internal partial class ButtonModel
{
    private ButtonScriptableControlData Data => (ButtonScriptableControlData)_data;

    public ButtonModel(IInputControlDataCalculatedValue data)
        : base(new InputButtonEngineFactory(), data)
    {
        if (Data.Values.Count == 0)
        {
            Data.Values = new Collection<string> { "" };
        }
    }

    public override object SelectedValue => Data.SelectedValueIndex;

    public string Text
    {
        get => Data.Values[0];
        set
        {
            Data.Values[0] = value;
            OnPropertyChanged(nameof(Text));
        }
    }

    public bool IsChecked
    {
        get
        {
            if (Data.SelectedValueIndex == 0.0)
            {
                return false;
            }

            return true;
        }
        set
        {
            object valueIndex;
            if (value)
            {
                valueIndex = 1.0;
            }
            else
            {
                valueIndex = 0.0;
            }

            string error;
            _data.SelectedCalculatedValue = scriptableEngine.GetCalculatedValue(
                Script,
                ref valueIndex,
                out object scriptControlObject,
                out error,
                Data.InputsResultValue,
                new ScriptEngineParams(Data) { IsInteractChanged = true });
            HandlePostInputFieldResultChanged(ref valueIndex, scriptControlObject, error, false);
            if (error != null)
            {
                EngineError = error;
                OnPropertyChanged(nameof(EngineError));
                return;
            }

            if (EngineError != null)
            {
                EngineError = null;
                OnPropertyChanged(nameof(EngineError));
            }

            HandelValueIndexChangedAndCommit(valueIndex);
        }
    }

    public override bool Apply(string userScript, out object resultObject, out string exception, object[] inputs, IScriptEngineParams engineParams = default)
    {
        return base.Apply(
            userScript,
            out resultObject,
            out exception,
            inputs ?? Data.InputsResultValue ?? scriptableEngine.GetLabelsFromSExpresstion(Data.InputFieldSExpression)?.ToArray<object>(),
            engineParams ?? new ScriptEngineParams(Data) { IsApply = true });
    }

    protected override void HandlePostInputFieldResultChanged(ref object valueIndex, object scriptObject, string errorException, bool isRecalculate = false)
    {
        if (scriptObject is Hashtable hashtable && errorException == null && hashtable.ContainsKey("text"))
        {
            ((IInputControlDataWithValues)_data).Values = new Collection<string> { hashtable["text"].ToString() };
            OnPropertyChanged(nameof(Text));
        }
    }

    public override string DefaultScript(InputControlEngineTypes engineType)
    {
        return ((IInputControlScriptingEngine)_engineFactory.CreateEngine(engineType)).Script;
    }
}
