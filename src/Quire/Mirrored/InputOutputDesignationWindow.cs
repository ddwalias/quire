#nullable disable
#pragma warning disable

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using Quire.Controls.Core;
using Quire.Controls.IO;
using Quire.Wpf;

namespace Quire;

public partial class InputOutputDesignationWindow
{
    private bool _handling;

    private DependencyObject _selectedRegion;

    private ScrollViewer _rootElement;

    public void InitializeComponent()
    {
        if (_contentLoaded)
        {
            return;
        }
        _contentLoaded = true;
        global::System.Uri resourceLocator = new global::System.Uri("/Quire;component/inputoutputdesignationwindow.xaml", global::System.UriKind.Relative);
        global::System.Windows.Application.LoadComponent(this, resourceLocator);
    }

    internal global::System.Delegate _CreateDelegate(global::System.Type delegateType, string handler)
    {
        return global::System.Delegate.CreateDelegate(delegateType, this, handler);
    }

    void global::System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target)
    {
        switch (connectionId)
        {
            case 1:
                InputOutputDesignation = (InputOutputDesignationWindow)target;
                return;
            case 5:
                _rootElement = (ScrollViewer)target;
                return;
            case 6:
                InputsListView = (ListView)target;
                InputsListView.SelectionChanged += InputsSelectionChanged;
                return;
            case 7:
                OutputsListView = (ListView)target;
                OutputsListView.SelectionChanged += OutputsSelectionChanged;
                return;
        }
        _contentLoaded = true;
    }

    private FrameworkElement GetRootElement()
    {
        return _rootElement;
    }

    public bool SetFocus()
    {
        return GetRootElement().Focus();
    }

    private bool IsDisconnectedItem(FrameworkElement frameworkElement)
    {
        return frameworkElement.DataContext.ToString() == "{DisconnectedItem}";
    }

    private void ShowSelectedRegion()
    {
        if (_selectedRegion != null)
        {
            _selectedRegion.SetValue(global::Quire.Controls.Core.DocumentAttachedProperties.ShowInputOutputDesignationProperty, true);
        }
    }

    private void HideSelectedRegion()
    {
        if (_selectedRegion != null)
        {
            _selectedRegion.ClearValue(global::Quire.Controls.Core.DocumentAttachedProperties.ShowInputOutputDesignationProperty);
        }
    }

    private static IOItem RetrieveAssociatedIOItem(object visual, out FrameworkElement element)
    {
        element = visual as FrameworkElement;
        if (DebugTools.AssertNotNull(element, null).Failed)
        {
            return null;
        }
        return element.DataContext as IOItem;
    }

    private void ShowIOItemInWorksheet(IOItem ioItem)
    {
        ioItem.ShowInWorksheet();
    }

    private void HandleMouseDown(object sender, RoutedEventArgs e)
    {
        FrameworkElement element;
        IOItem iOItem = RetrieveAssociatedIOItem(sender, out element);
        if (DebugTools.AssertNotNull(iOItem, null).Passed && iOItem.Region == _selectedRegion)
        {
            ShowIOItemInWorksheet(iOItem);
        }
    }

    private void ListViewItemSelected(object sender, RoutedEventArgs e)
    {
        FrameworkElement element;
        IOItem iOItem = RetrieveAssociatedIOItem(sender, out element);
        if (DebugTools.AssertNotNull(iOItem, null).Failed)
        {
            return;
        }
        element.BringIntoView();
        iOItem.ShowInWorksheet();
        HideSelectedRegion();
        _selectedRegion = iOItem.Region;
        ShowSelectedRegion();
    }

    private void InputsSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count > 0)
        {
            OutputsListView.UnselectAll();
        }
    }

    private void OutputsSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count > 0)
        {
            InputsListView.UnselectAll();
        }
    }

}
