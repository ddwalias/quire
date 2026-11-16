using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Quire.EquationEditor.Tree;

namespace Quire.Controls.InputControls;

internal partial class RadioButtonModel
{
    private string _name = "RadioBtn";
    private string _groupID = "1";
    private string _buttonID = "1";
    private string _imagePath;
    private string _itemsCache;

    private new RadioButtonScriptableControlData Data => (RadioButtonScriptableControlData)_data;

    public RadioButtonModel(IInputControlDataCalculatedValue data)
        : base(new InputRadioButtonEngineFactory(), data)
    {
    }

    public override object SelectedValue => Data.SelectedValueIndex;

    public override bool Apply(string userScript, out object resultObject, out string exception, object[] inputs, IScriptEngineParams engineParams = null)
    {
        return base.Apply(userScript, out resultObject, out exception, inputs ?? Data.InputsResultValue, engineParams ?? new ScriptEngineParams(Data)
        {
            IsApply = true
        });
    }

    protected override void HandlePostInputFieldResultChanged(ref object valueIndex, object scriptObject, string errorException, bool isRecalculate = false)
    {
        IsDataSourceChanged = false;
        if (scriptObject is Hashtable hashtable && errorException == null)
        {
            if (hashtable.ContainsKey("items") && hashtable["items"].ToString() != string.Empty && (_itemsCache == null || _itemsCache != hashtable["items"].ToString()))
            {
                _itemsCache = hashtable["items"].ToString();
                ((IInputControlDataWithValues)_data).Values = new Collection<string>(_itemsCache.Split('\v'));
                IsDataSourceChanged = true;
                IsDirty = !isRecalculate;
                OnPropertyChanged("DataSourceItems");
            }
            if (hashtable.ContainsKey("leftText") && bool.TryParse(hashtable["leftText"].ToString(), out var result))
            {
                FlowDirection = (result ? FlowDirection.RightToLeft : FlowDirection.LeftToRight);
            }
            if (hashtable.ContainsKey("orientation") && double.TryParse(hashtable["orientation"].ToString(), out var result2))
            {
                Orientation = (result2 == 0.0 ? Orientation.Horizontal : Orientation.Vertical);
            }
            if (hashtable.ContainsKey("CurSel"))
            {
                if (double.TryParse(hashtable["CurSel"].ToString(), out var result3))
                {
                    if (Data.SelectedValueIndex != result3)
                    {
                        Data.SelectedValueIndex = result3;
                        valueIndex = result3;
                        IsDirty = !isRecalculate;
                        OnPropertyChanged("SelectedIndex");
                    }
                }
                else if (IsDataSourceChanged)
                {
                    OnPropertyChanged("SelectedIndex");
                }
            }
        }
    }

    public double SelectedIndex
    {
        get => Data.SelectedValueIndex;
        set
        {
            Data.SelectedValueIndex = (int)value;
            string exception = null;
            object value2 = value;
            if (value >= 0.0)
            {
                _data.SelectedCalculatedValue = scriptableEngine.GetCalculatedValue(Script, ref value2, out object scriptControlObject, out exception, Data.InputsResultValue, new ScriptEngineParams(Data)
                {
                    IsInteractChanged = true
                });
                HandlePostInputFieldResultChanged(ref value2, scriptControlObject, exception);
            }
            if (exception != null)
            {
                EngineError = exception;
                OnPropertyChanged("EngineError");
                return;
            }
            if (EngineError != null)
            {
                EngineError = null;
                OnPropertyChanged("EngineError");
            }
            HandelValueIndexChangedAndCommit(Data.SelectedValueIndex);
            OnPropertyChanged("SelectedIndex");
        }
    }

    public override string DefaultScript(InputControlEngineTypes engineType)
    {
        return ((IInputControlScriptingEngine)_engineFactory.CreateEngine(engineType)).Script;
    }

    public Collection<string> DataSourceItems
    {
        get => Data.Values;
        set
        {
            Data.Values = value;
            OnPropertyChanged("DataSourceItems");
        }
    }

    public Orientation Orientation
    {
        get => Data.Orientation;
        set
        {
            Data.Orientation = value;
            OnPropertyChanged("Orientation");
        }
    }

    public string ImagePath
    {
        get => _imagePath;
        set
        {
            _imagePath = value;
            OnPropertyChanged("ImagePath");
        }
    }

    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            OnPropertyChanged("Name");
        }
    }

    public string GroupID
    {
        get => _groupID;
        set
        {
            _groupID = value;
            OnPropertyChanged("GroupID");
        }
    }

    public string ButtonID
    {
        get => _buttonID;
        set
        {
            _buttonID = value;
            OnPropertyChanged("ButtonID");
        }
    }

    public FlowDirection FlowDirection
    {
        get => Data.FlowDirection;
        set
        {
            Data.FlowDirection = value;
            OnPropertyChanged("FlowDirection");
        }
    }

    public bool IsDataSourceChanged { get; set; }

    public void HandleButtonsChanged(ObservableCollection<DataListItem> editorDataSourceItems)
    {
        EditorDataSourceItems = editorDataSourceItems;
        IsDataSourceChanged = true;
        Data.SelectedCalculatedValue = new List<object> { 0 };
        Data.SelectedValueIndex = -1.0;
    }

    public ObservableCollection<DataListItem> EditorDataSourceItems { get; set; }
}
