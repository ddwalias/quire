using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Quire.Core;
using Quire.EquationEditor.Tree;
using Quire.Wpf;

namespace Quire.Controls.InputControls;

public partial class RadioButtonViewModel : InputScriptableControlViewModelWithInputs, IInputControlViewModelWithEditor
{
    private FlowDirection _flowDirection;
    private bool _isChecked;
    private PropertyInfo _backgroundColor;
    private string _imagePath;
    private string _groupID = "1";
    private double _selectedValue;
    private IList<string> _dataItems;
    private ObservableCollection<DataListItem> _dataSourceItems;
    private Orientation _orientation;
    private readonly RadioButtonEventAggregator _radioButtonEventAggregator;

    private new RadioButtonModel Model => (RadioButtonModel)_model;

    public RadioButtonViewModel(IInputControlDataCalculatedValue data, RadioButtonEventAggregator radioButtonEventAggregator = null)
        : base(new RadioButtonModel(data))
    {
        _dataSourceItems = new ObservableRangeCollection<DataListItem>(Model.DataSourceItems.Select((string name, int item) => new DataListItem
        {
            Name = name,
            Value = (item + 1).ToString(),
            GroupName = "id" + GetHashCode()
        }));
    }

    public virtual void CommitEditorChanges()
    {
        if (Model.IsDataSourceChanged)
        {
            if (Model.EditorDataSourceItems != null)
            {
                Model.DataSourceItems = new Collection<string>(Model.EditorDataSourceItems.Where(x => x.Name.IsNotEmpty()).Select(x => x.Name).ToArray());
            }
            Model.IsDataSourceChanged = false;
            OnPropertyChanged("IsDataSourceChanged");
        }
    }

    public bool IsDataSourceChanged => Model.IsDataSourceChanged;

    public double SelectedIndex
    {
        get => Model.SelectedIndex;
        set
        {
            if (Model.SelectedIndex != value && value > -1.0)
            {
                Model.SelectedIndex = value;
            }
        }
    }

    public ObservableCollection<DataListItem> DataSourceItems => _dataSourceItems;

    public Orientation Orientation
    {
        get => Model.Orientation;
        set => Model.Orientation = value;
    }

    protected override void HandleModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        DataListItem dataListItem;
        switch (e.PropertyName)
        {
            case "FlowDirection":
            case "Orientation":
            case "SelectedIndex":
                OnPropertyChanged(e.PropertyName);
                break;
            case "DataSourceItems":
                _dataSourceItems = new ObservableRangeCollection<DataListItem>(Model.DataSourceItems.Select((string name, int item) => new DataListItem
                {
                    Name = name,
                    Value = (item + 1).ToString(),
                    GroupName = "id" + GetHashCode()
                }));
                OnPropertyChanged(e.PropertyName);
                break;
            case "Text":
            {
                dataListItem = _dataSourceItems[(int)SelectedIndex];
                dataListItem.Name = Model.DataSourceItems[(int)SelectedIndex];
                _dataSourceItems[(int)SelectedIndex] = new DataListItem
                {
                    GroupName = dataListItem.GroupName,
                    Name = dataListItem.Name,
                    Value = dataListItem.Value
                };
                break;
            }
            case "GroupID":
                if (IsChecked)
                {
                    IsChecked = true;
                }
                break;
        }
    }

    public PropertyInfo BackgroundColor
    {
        get => _backgroundColor;
        set => _backgroundColor = value;
    }

    public bool IsChecked
    {
        get => _isChecked;
        set
        {
            _isChecked = value;
            OnPropertyChanged("IsChecked");
        }
    }

    public string RadioButtons => "RadioButtons" + GetHashCode();

    public string GroupID
    {
        get => _groupID;
        set
        {
            _groupID = value;
            OnPropertyChanged("GroupID");
        }
    }

    public string ImagePath
    {
        get => _imagePath;
        set => _imagePath = value;
    }

    public FlowDirection FlowDirection
    {
        get => Model.FlowDirection;
        set => Model.FlowDirection = value;
    }
}
