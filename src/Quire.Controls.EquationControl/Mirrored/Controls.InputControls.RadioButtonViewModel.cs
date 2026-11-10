using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace Quire.Controls.InputControls;

public partial class RadioButtonViewModel
{
    private RadioButtonModel Model => (RadioButtonModel)_model;

    public bool IsDataSourceChanged => Model.IsDataSourceChanged;

    public double SelectedIndex
    {
        get => Model.SelectedIndex;
        set
        {
            if (Model.SelectedIndex != value && value > -1)
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
            OnPropertyChanged(nameof(IsChecked));
        }
    }

    public string RadioButtons => "RadioButtons" + GetHashCode().ToString();

    public string GroupID
    {
        get => _groupID;
        set
        {
            _groupID = value;
            OnPropertyChanged(nameof(GroupID));
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
