using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Quire.EquationEditor.Tree;

namespace Quire.Controls.InputControls;

internal partial class TextBoxModel : InputScriptableControlModelWithInputs, ITextBoxPropertiesEditorModel, IInputControlEditorScriptableModel, IInputControlEditorModel, INotifyPropertyChanged
{
    protected string _text;

    private new TextBoxScriptableControlData Data => (TextBoxScriptableControlData)_data;

    public TextBoxModel(IInputControlDataCalculatedValue data)
        : base(new InputTextBoxEngineFactory(), data)
    {
    }

    public override bool Apply(string userScript, out object resultObject, out string exception, object[] inputs, IScriptEngineParams engineParams = null)
    {
        return base.Apply(userScript, out resultObject, out exception, inputs ?? Data.InputsResultValue ?? scriptableEngine.GetLabelsFromSExpresstion(Data.InputFieldSExpression)?.ToArray<object>(), engineParams ?? new ScriptEngineParams(Data)
        {
            IsApply = true
        });
    }

    public override object SelectedValue => Text;

    protected override void RunScript(bool isRecalculate = false)
    {
        object value = Data.Values[0];
        RunScriptInternal(ref value, isRecalculate);
    }

    public override void HandelValueIndexChanged(object valueIndex)
    {
    }

    protected override void HandlePostInputFieldResultChanged(ref object textShownInTheBox, object scriptObject, string errorException, bool isRecalculate = false)
    {
        if (scriptObject is Hashtable hashtable && errorException == null)
        {
            if (hashtable.ContainsKey("text") && hashtable["text"] != null)
            {
                string text = hashtable["text"].ToString();
                textShownInTheBox = text;
                ((IInputControlDataWithValues)_data).Values = new Collection<string> { text };
                IsDirty = !isRecalculate;
                OnPropertyChanged("Text");
            }
            if (hashtable.ContainsKey("fontSize"))
            {
                FontSize = double.Parse(hashtable["fontSize"].ToString(), CultureInfo.InvariantCulture);
            }
            if (hashtable.ContainsKey("foreColor"))
            {
                Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom(hashtable["foreColor"].ToString());
            }
            if (hashtable.ContainsKey("textAlignment"))
            {
                TextAlignment = (bool.Parse(hashtable["textAlignment"].ToString()) ? TextAlignment.Right : TextAlignment.Left);
            }
            if (hashtable.ContainsKey("acceptsReturn"))
            {
                AcceptsReturn = bool.Parse(hashtable["acceptsReturn"].ToString());
            }
            if (hashtable.ContainsKey("readOnly"))
            {
                IsReadOnly = bool.Parse(hashtable["readOnly"].ToString());
            }
            if (hashtable.ContainsKey("backColor"))
            {
                Background = (SolidColorBrush)new BrushConverter().ConvertFrom(hashtable["backColor"].ToString());
            }
        }
    }

    public override string DefaultScript(InputControlEngineTypes engineType)
    {
        return ((IInputControlScriptingEngine)_engineFactory.CreateEngine(engineType)).Script;
    }

    public Brush Foreground
    {
        get
        {
            return (SolidColorBrush)new BrushConverter().ConvertFrom(Data.Foreground);
        }
        set
        {
            Data.Foreground = ((object)value).ToString();
            OnPropertyChanged("Foreground");
        }
    }

    public Brush Background
    {
        get
        {
            return (SolidColorBrush)new BrushConverter().ConvertFrom(Data.Background);
        }
        set
        {
            Data.Background = ((object)value).ToString();
            OnPropertyChanged("Background");
        }
    }

    public override double FontSize
    {
        get
        {
            return Data.FontSize;
        }
        set
        {
            Data.FontSize = value;
            OnPropertyChanged("FontSize");
        }
    }

    public bool IsReadOnly
    {
        get
        {
            return Data.IsReadOnly;
        }
        set
        {
            Data.IsReadOnly = value;
            OnPropertyChanged("IsReadOnly");
        }
    }

    public bool AcceptsReturn
    {
        get
        {
            return Data.AcceptsReturn;
        }
        set
        {
            Data.AcceptsReturn = value;
            OnPropertyChanged("AcceptsReturn");
        }
    }

    public string Text
    {
        get
        {
            return Data.Values[0];
        }
        set
        {
            Data.Values[0] = value;
            Data.IsNeedsCalculate = true;
        }
    }

    public TextAlignment TextAlignment
    {
        get
        {
            return Data.TextAlignment;
        }
        set
        {
            Data.TextAlignment = value;
            OnPropertyChanged("TextAlignment");
        }
    }

    public void OnLostFocus()
    {
        if (Data.IsNeedsCalculate)
        {
            string exception;
            object value = Text;
            object scriptControlObject;
            _data.SelectedCalculatedValue = scriptableEngine.GetCalculatedValue(Script, ref value, out scriptControlObject, out exception, Data.InputsResultValue, new ScriptEngineParams(Data)
            {
                IsInteractChanged = true
            });
            HandlePostInputFieldResultChanged(ref value, scriptControlObject, exception);
            if (exception != null)
            {
                EngineError = exception;
                OnPropertyChanged("EngineError");
            }
            else
            {
                if (EngineError != null)
                {
                    EngineError = null;
                    OnPropertyChanged("EngineError");
                }
                HandelValueIndexChangedAndCommit(Data.SelectedValueIndex);
            }
            Data.IsNeedsCalculate = false;
        }
    }
}
