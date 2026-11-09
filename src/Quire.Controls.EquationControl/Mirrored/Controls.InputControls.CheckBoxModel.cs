using System.Collections;
using System.Collections.ObjectModel;
using System.Windows;
using Quire.EquationEditor.Tree;

namespace Quire.Controls.InputControls;

internal partial class CheckBoxModel
{
    private string _imagePath;

    private CheckBoxScriptableControlData Data => (CheckBoxScriptableControlData)_data;

    public CheckBoxModel(IInputControlDataCalculatedValue data)
        : base(new InputCheckBoxEngineFactory(), data)
    {
        if (Data.Values.Count == 0)
        {
            Data.Values = new Collection<string> { "" };
        }
    }

    public override object SelectedValue => Data.SelectedValueIndex;

    public FlowDirection FlowDirection
    {
        get => Data.FlowDirection;
        set
        {
            Data.FlowDirection = value;
            OnPropertyChanged(nameof(FlowDirection));
        }
    }

    public string ImagePath
    {
        get => _imagePath;
        set
        {
            _imagePath = value;
            OnPropertyChanged(nameof(ImagePath));
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
            if (value)
            {
                Data.SelectedValueIndex = 1.0;
            }
            else
            {
                Data.SelectedValueIndex = 0.0;
            }

            string error;
            object valueIndex = Data.SelectedValueIndex;
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
            }
            else
            {
                if (EngineError != null)
                {
                    EngineError = null;
                    OnPropertyChanged(nameof(EngineError));
                }

                HandelValueIndexChangedAndCommit(Data.SelectedValueIndex);
            }

            OnPropertyChanged(nameof(IsChecked));
        }
    }

    public string Text
    {
        get => Data.Values[0];
        set
        {
            Data.Values[0] = value;
            OnPropertyChanged(nameof(Text));
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
        if (scriptObject is Hashtable hashtable && errorException == null)
        {
            if (hashtable.ContainsKey("text"))
            {
                ((IInputControlDataWithValues)_data).Values = new Collection<string> { hashtable["text"].ToString() };
                OnPropertyChanged(nameof(Text));
            }

            if (hashtable.ContainsKey("leftText") && bool.TryParse(hashtable["leftText"].ToString(), out bool isRightToLeft))
            {
                FlowDirection = isRightToLeft ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
            }

            if (hashtable.ContainsKey("check") && double.TryParse(hashtable["check"].ToString(), out double check))
            {
                Data.SelectedValueIndex = check;
                valueIndex = check;
                IsDirty = !isRecalculate;
                OnPropertyChanged(nameof(IsChecked));
            }
        }
    }

    public override string DefaultScript(InputControlEngineTypes engineType)
    {
        return ((IInputControlScriptingEngine)_engineFactory.CreateEngine(engineType)).Script;
    }
}
