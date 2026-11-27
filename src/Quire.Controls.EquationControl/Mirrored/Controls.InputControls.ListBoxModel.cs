using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Quire.EquationEditor.Tree;

namespace Quire.Controls.InputControls;

public partial class ListBoxModel
{
    private string _itemsCache;

    private bool _isSortChanged;

    public ListBoxModel(IInputControlDataCalculatedValue data)
        : base(new InputListBoxEngineFactory(), data)
    {
        if (((IInputControlDataWithValues)data).Values != null)
        {
            _itemsCache = string.Join('\v'.ToString(), ((IInputControlDataWithValues)data).Values);
        }
    }

    public override object SelectedValue => Data.SelectedValueIndex;

    public bool IsSort
    {
        get
        {
            return ((IListBoxScriptableControlData)_data).Sort;
        }
        set
        {
            if (((IListBoxScriptableControlData)_data).Sort != value)
            {
                ((IListBoxScriptableControlData)_data).Sort = value;
                OnPropertyChanged("IsSort");
            }
        }
    }

    public void OnDoubleClick(MouseButtonEventArgs e)
    {
        if (!scriptableEngine.CanExecEvent("DblClick"))
        {
            return;
        }

        _data.SelectedCalculatedValue = scriptableEngine.RunEventStandalone("DblClick", out var exception);
        if (exception != null)
        {
            EngineError = exception;
            OnPropertyChanged("EngineError");
        }
        else if (EngineError != null)
        {
            EngineError = null;
            OnPropertyChanged("EngineError");
        }
        HandelValueIndexChangedAndCommit(Data.SelectedValueIndex);
    }

    public double SelectedIndex
    {
        get
        {
            return Data.SelectedValueIndex;
        }
        set
        {
            if (Data.SelectedValueIndex != value && IsInitializeComponent)
            {
                Data.SelectedValueIndex = value;
                if (!_isSortChanged)
                {
                    HandleSelectionChanged();
                }
            }
        }
    }

    private void HandleObjectProperties(ref object valueIndex, object objectProperties, bool isRecalculate = false)
    {
        bool flag = false;
        Hashtable hashtable = objectProperties as Hashtable;
        if (hashtable != null)
        {
            if (hashtable.ContainsKey("items") && (_itemsCache == null || _itemsCache != hashtable["items"].ToString()))
            {
                flag = true;
                _itemsCache = hashtable["items"].ToString();
                ((IInputControlDataWithValues)_data).Values = new Collection<string>(_itemsCache.Split('\v'));
                IsInitializeComponent = false;
                OnPropertyChanged("DataSourceItems");
                IsInitializeComponent = true;
            }
            if (hashtable.ContainsKey("sort") && bool.TryParse(hashtable["sort"].ToString(), out var result))
            {
                _isSortChanged = IsSort != result;
                IsSort = result;
                _isSortChanged = false;
            }
            if (hashtable.ContainsKey("rightAlignedText") && bool.TryParse(hashtable["rightAlignedText"].ToString(), out var isRightAligned))
            {
                FlowDirection = (isRightAligned ? FlowDirection.RightToLeft : FlowDirection.LeftToRight);
            }
            if (hashtable.ContainsKey("CurSel") && double.TryParse(hashtable["CurSel"].ToString(), out var selectedValueIndex))
            {
                if (Data.SelectedValueIndex != selectedValueIndex || IsSort)
                {
                    Data.SelectedValueIndex = selectedValueIndex;
                    valueIndex = selectedValueIndex;
                    OnPropertyChanged("SelectedIndex");
                }
                else if (((double)valueIndex != selectedValueIndex) | flag)
                {
                    valueIndex = selectedValueIndex;
                    OnPropertyChanged("SelectedIndex");
                }
            }
        }
    }

    private void HandleSelectionChanged()
    {
        string exception = null;
        object value = Data.SelectedValueIndex;
        if (Data.SelectedValueIndex >= 0.0)
        {
            _data.SelectedCalculatedValue = scriptableEngine.GetCalculatedValue(Script, ref value, out object scriptControlObject, out exception, Data.InputsResultValue, new ScriptEngineParams(Data)
            {
                IsInteractChanged = true
            });
            HandleObjectProperties(ref value, scriptControlObject);
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
    }

    protected override void HandlePostInputFieldResultChanged(ref object valueIndex, object scriptObjectproperties, string errorException, bool isRecalculate = false)
    {
        HandleObjectProperties(ref valueIndex, scriptObjectproperties, isRecalculate);
    }

    public override string DefaultScript(InputControlEngineTypes engineType)
    {
        return ((IInputControlScriptingEngine)_engineFactory.CreateEngine(engineType)).Script;
    }

    public Collection<string> DataSourceItems => ((IInputControlDataWithValues)_data).Values;

    public override bool Apply(string userScript, out object resultObjectProperties, out string exception, object[] inputs, IScriptEngineParams engineParams = null)
    {
        return base.Apply(userScript, out resultObjectProperties, out exception, inputs ?? Data.InputsResultValue ?? ((IEnumerable<object>)scriptableEngine.GetLabelsFromSExpresstion(Data.InputFieldSExpression))?.ToArray(), engineParams ?? new ScriptEngineParams(Data)
        {
            IsApply = true
        });
    }

    public FlowDirection FlowDirection
    {
        get
        {
            return ((IListBoxScriptableControlData)_data).FlowDirection;
        }
        set
        {
            if (((IListBoxScriptableControlData)_data).FlowDirection != value)
            {
                ((IListBoxScriptableControlData)_data).FlowDirection = value;
                OnPropertyChanged("FlowDirection");
            }
        }
    }
}
