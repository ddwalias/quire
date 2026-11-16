using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using Quire.EquationEditor.Tree;

namespace Quire.Controls.InputControls;

public partial class RadioButtonControl
{
    internal RadioButtonControl listView;
    private bool _contentLoaded;

    public RadioButtonControl(IInputControlDataCalculatedValue data, RadioButtonEventAggregator radioButtonEventAggregator = null)
    {
        DataContext = new RadioButtonViewModel(data, null);
        ViewModel.PropertyChanged += ViewModelPropertyChanged;
        InitializeComponent();
    }

    public IInputControlViewModel ViewModel => (RadioButtonViewModel)DataContext;

    private void ViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "IsDataSourceChanged")
        {
            Focus();
        }
    }

    private void OnPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        e.Handled = true;
    }

    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (listView.SelectedIndex != listView.Items.IndexOf(listView.SelectedItem) && !listView.Items.IsEmpty)
        {
            listView.SelectedItem = listView.Items[listView.SelectedIndex];
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
        Uri resourceLocator = new Uri("/Quire.Controls.EquationControl;component/inputcontrols/implementations/scriptableradiobutton/radiobuttoncontrol.xaml", UriKind.Relative);
        Application.LoadComponent(this, resourceLocator);
    }

    void IComponentConnector.Connect(int connectionId, object target)
    {
        if (connectionId == 1)
        {
            listView = (RadioButtonControl)target;
            listView.PreviewMouseRightButtonDown += OnPreviewMouseRightButtonDown;
            listView.SelectionChanged += OnSelectionChanged;
        }
        else
        {
            _contentLoaded = true;
        }
    }
}
