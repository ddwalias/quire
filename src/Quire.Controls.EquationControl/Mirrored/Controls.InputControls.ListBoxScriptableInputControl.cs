using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using Quire.EquationEditor.Tree;

namespace Quire.Controls.InputControls;

public partial class ListBoxScriptableInputControl
{
    internal ListBox internalListBox;

    private bool _contentLoaded;

    public ListBoxScriptableInputControl()
    {
        InitializeComponent();
    }

    public ListBoxScriptableInputControl(IInputControlDataCalculatedValue data)
    {
        DataContext = new ListBoxViewModel(data);
        ViewModel.PropertyChanged += ViewModelPropertyChanged;
        ViewModel.IsInitializeComponent = false;
        InitializeComponent();
        if (internalListBox.SelectedIndex == -1)
        {
            internalListBox.SelectedItem = null;
        }
        HandleToggleSort();
        ViewModel.IsInitializeComponent = true;
    }

    private void ViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "IsSort")
        {
            HandleToggleSort();
        }
    }

    private void HandleToggleSort()
    {
        if (((ListBoxViewModel)ViewModel).IsSort)
        {
            internalListBox.Items.SortDescriptions.Clear();
            if (internalListBox.Items.SortDescriptions.Count == 0)
            {
                internalListBox.Items.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
                ((CollectionViewSource)Resources["Items"]).View.MoveCurrentToPosition((int)((ListBoxViewModel)ViewModel).SelectedIndex);
                internalListBox.SelectedItem = ((CollectionViewSource)Resources["Items"]).View.CurrentItem;
            }
        }
        else
        {
            internalListBox.Items.SortDescriptions.Clear();
        }
    }

    public IInputControlViewModel ViewModel => (IInputControlViewModel)DataContext;

    private void OnPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        e.Handled = true;
    }

    private void OnDoubleClick(object sender, MouseButtonEventArgs e)
    {
        ((ListBoxViewModel)ViewModel).OnDoubleClick(e);
    }

    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        int num = (int)((ListBoxViewModel)ViewModel).SelectedIndex;
        ListCollectionView listCollectionView = ((CollectionViewSource)Resources["Items"]).View as ListCollectionView;
        if (num != listCollectionView.CurrentPosition && listCollectionView.Count > num)
        {
            listCollectionView.MoveCurrentToPosition(num);
            internalListBox.SelectedItem = listCollectionView.CurrentItem;
        }
        e.Handled = true;
    }

    public void InitializeComponent()
    {
        if (_contentLoaded)
        {
            return;
        }

        _contentLoaded = true;
        Uri resourceLocator = new Uri("/Quire.Controls.EquationControl;component/inputcontrols/implementations/scriptablelistbox/listboxscriptableinputcontrol.xaml", UriKind.Relative);
        Application.LoadComponent(this, resourceLocator);
    }

    void IComponentConnector.Connect(int connectionId, object target)
    {
        switch (connectionId)
        {
        case 1:
            ((ListBoxScriptableInputControl)target).PreviewMouseRightButtonDown += OnPreviewMouseRightButtonDown;
            break;
        case 2:
            internalListBox = (ListBox)target;
            internalListBox.SelectionChanged += OnSelectionChanged;
            internalListBox.MouseDoubleClick += OnDoubleClick;
            break;
        default:
            _contentLoaded = true;
            break;
        }
    }
}
