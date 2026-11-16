using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Quire.Core;
using Quire.EquationEditor.Tree;

namespace Quire.Controls.InputControls;

internal partial class ListBoxViewModel : InputScriptableControlViewModelWithInputs, IInputControlViewModelWithEditor
{
    public ListBoxViewModel(IInputControlDataCalculatedValue data)
        : base(new ListBoxModel(data))
    {
    }

    private ListBoxModel ListBoxModel
    {
        get
        {
            DebugTools.DevAssert(Model is ListBoxModel, "Model is not ListBoxModel");
            return Model as ListBoxModel;
        }
    }

    protected override void HandleModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case "IsSort":
            case "FlowDirection":
            case "DataSourceItems":
            case "SelectedIndex":
                OnPropertyChanged(e.PropertyName);
                break;
        }
    }

    public FlowDirection FlowDirection
    {
        get => ListBoxModel.FlowDirection;
        set => ListBoxModel.FlowDirection = value;
    }

    public double SelectedIndex
    {
        get => ListBoxModel.SelectedIndex;
        set
        {
            if (IsInitializeComponent && ListBoxModel.SelectedIndex != value)
            {
                ListBoxModel.SelectedIndex = value;
            }
        }
    }

    public virtual void CommitEditorChanges()
    {
    }

    public bool IsSort
    {
        get => ListBoxModel.IsSort;
        set
        {
            ListBoxModel.IsSort = value;
            OnPropertyChanged("IsSort");
        }
    }

    public void OnDoubleClick(MouseButtonEventArgs e)
    {
        ListBoxModel.OnDoubleClick(e);
    }

    public List<DataListItem> DataSourceItems => new List<DataListItem>(((ListBoxModel)Model).DataSourceItems?.Select((string name) => new DataListItem
    {
        Name = name
    }));
}
