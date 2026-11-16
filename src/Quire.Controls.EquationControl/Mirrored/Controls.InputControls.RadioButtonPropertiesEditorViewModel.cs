using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Quire.Controls.InputControls;

internal partial class RadioButtonPropertiesEditorViewModel
{
    private ObservableCollection<DataListItem> _dataSourceItems;

    private IRadioButtonPropertiesEditorModel Model => (IRadioButtonPropertiesEditorModel)_model;

    public RadioButtonPropertiesEditorViewModel(IInputControlEditorModel model)
        : base(model)
    {
        _dataSourceItems = new ObservableCollection<DataListItem>(Model.DataSourceItems.Select((string name, int item) => new DataListItem
        {
            Name = name,
            Value = (item + 1).ToString()
        }));
        Model.IsDataSourceChanged = false;
        Model.EditorDataSourceItems = null;
    }

    protected override void HandleModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case "FlowDirection":
                OnPropertyChanged("RightToLeft");
                break;
            case "RightToLeft":
            case "Orientation":
                OnPropertyChanged(e.PropertyName);
                break;
            case "DataSourceItems":
                _dataSourceItems = new ObservableCollection<DataListItem>(Model.DataSourceItems.Select((string name, int item) => new DataListItem
                {
                    Name = name,
                    Value = (item + 1).ToString()
                }));
                OnPropertyChanged(e.PropertyName);
                break;
        }
    }

    public void HandleButtonsChanged()
    {
        Model.HandleButtonsChanged(_dataSourceItems);
    }

    public override void ApplyScript()
    {
        Model.EditorDataSourceItems = null;
        base.ApplyScript();
    }

    public Orientation Orientation
    {
        get => Model.Orientation;
        set
        {
            Model.Orientation = value;
            OnPropertyChanged("Orientation");
        }
    }

    public ObservableCollection<DataListItem> DataSourceItems => _dataSourceItems;

    public bool RightToLeft
    {
        get => Model.FlowDirection == FlowDirection.RightToLeft;
        set
        {
            if (value)
            {
                Model.FlowDirection = FlowDirection.RightToLeft;
            }
            else
            {
                Model.FlowDirection = FlowDirection.LeftToRight;
            }
            OnPropertyChanged("RightToLeft");
        }
    }

    public Collection<string> Values
    {
        get => Model.DataSourceItems;
        set => Model.DataSourceItems = value;
    }
}
