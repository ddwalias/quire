using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Quire.EquationEditor.Tree;

namespace Quire.Controls.InputControls;

internal partial class RadioButtonModel
{
    private RadioButtonScriptableControlData Data => (RadioButtonScriptableControlData)_data;

    public Orientation Orientation
    {
        get => Data.Orientation;
        set
        {
            Data.Orientation = value;
            OnPropertyChanged(nameof(Orientation));
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

    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            OnPropertyChanged(nameof(Name));
        }
    }

    public string GroupID
    {
        get => _groupID;
        set
        {
            _groupID = value;
            OnPropertyChanged(nameof(GroupID));
        }
    }

    public string ButtonID
    {
        get => _buttonID;
        set
        {
            _buttonID = value;
            OnPropertyChanged(nameof(ButtonID));
        }
    }

    public FlowDirection FlowDirection
    {
        get => Data.FlowDirection;
        set
        {
            Data.FlowDirection = value;
            OnPropertyChanged(nameof(FlowDirection));
        }
    }

    public bool IsDataSourceChanged { get; set; }

    public ObservableCollection<DataListItem> EditorDataSourceItems { get; set; }

    public Collection<string> DataSourceItems
    {
        get => Data.Values;
        set
        {
            Data.Values = value;
            OnPropertyChanged(nameof(DataSourceItems));
        }
    }

    public void HandleButtonsChanged(ObservableCollection<DataListItem> editorDataSourceItems)
    {
        EditorDataSourceItems = editorDataSourceItems;
        IsDataSourceChanged = true;
        Data.SelectedCalculatedValue = new List<object> { 0 };
        Data.SelectedValueIndex = -1;
    }
}
