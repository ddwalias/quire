#nullable disable
#pragma warning disable

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
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
    private static readonly PropertyGroupDescription _groupByRegion = new PropertyGroupDescription("Region");

    private bool _handling;

    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("Quire base is AvalonDock LayoutAnchorable: no Name property, DockWindow ctor or FloatingRect")]
    public InputOutputDesignationWindow() { }

    private FrameworkElement GetRootElement()
    {
        return _rootElement;
    }

    private void DefineFloatingRectangle() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private bool IsThisDockWindowSelected(object dockControl) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public void HandleAfterDockedEvent(object sender, RoutedEventArgs args) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public bool SetFocus()
    {
        return GetRootElement().Focus();
    }

    public void OpenWindow(CommandBinding helpCommandBinding) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private bool IsDisconnectedItem(FrameworkElement frameworkElement)
    {
        return frameworkElement.DataContext.ToString() == "{DisconnectedItem}";
    }

    private void PreMouseDown(TextBox box)
    {
        IOItem ioItem = box.DataContext as IOItem;
        if (IsDisconnectedItem(box) || DebugTools.AssertNotNull(ioItem, null).Failed)
        {
            return;
        }

        ListView listView = ioItem.IsInput ? InputsListView : OutputsListView;
        if (!box.IsKeyboardFocusWithin)
        {
            listView.Focus();
        }

        if (listView.SelectedItem != ioItem)
        {
            listView.SelectedItem = ioItem;
        }
        else
        {
            ShowIOItemInWorksheet(ioItem);
        }
    }

    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("Release IL: leave target differs by one instruction in the nested Alias branch")]
    private void LostKeyboardFocusHandler(object sender, KeyboardFocusChangedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private static void EngineeringDocumentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        InputOutputDesignationWindow window = (InputOutputDesignationWindow)d;
        if (DebugTools.AssertNotNull(e.NewValue, null).Failed)
        {
            return;
        }

        IEngineeringDocument engineeringDocument = (IEngineeringDocument)e.NewValue;
        window.InputCollection = new ListCollectionView(engineeringDocument.Inputs);
        window.OutputCollection = new ListCollectionView(engineeringDocument.Outputs);
        if (window.InputCollection.GroupDescriptions != null)
        {
            window.InputCollection.GroupDescriptions.Add(_groupByRegion);
        }

        if (window.OutputCollection.GroupDescriptions != null)
        {
            window.OutputCollection.GroupDescriptions.Add(_groupByRegion);
        }
    }

    public static readonly DependencyProperty EngineeringDocumentProperty = DependencyProperty.Register("EngineeringDocument", typeof(IEngineeringDocument), typeof(InputOutputDesignationWindow), new PropertyMetadata(EngineeringDocumentChanged));

    private static readonly DependencyPropertyKey _inputCollectionPropertyKey = DependencyProperty.RegisterReadOnly("InputCollection", typeof(ListCollectionView), typeof(InputOutputDesignationWindow), new FrameworkPropertyMetadata());

    public static readonly DependencyProperty InputCollectionProperty = _inputCollectionPropertyKey.DependencyProperty;

    private static readonly DependencyPropertyKey _outputCollectionPropertyKey = DependencyProperty.RegisterReadOnly("OutputCollection", typeof(ListCollectionView), typeof(InputOutputDesignationWindow), new FrameworkPropertyMetadata());

    public static readonly DependencyProperty OutputCollectionProperty = _outputCollectionPropertyKey.DependencyProperty;

    public IEngineeringDocument EngineeringDocument
    {
        get
        {
            return (IEngineeringDocument)GetValue(EngineeringDocumentProperty);
        }
        set
        {
            SetValue(EngineeringDocumentProperty, value);
        }
    }

    public ListCollectionView InputCollection
    {
        get
        {
            return (ListCollectionView)GetValue(InputCollectionProperty);
        }
        private set
        {
            SetValue(_inputCollectionPropertyKey, value);
        }
    }

    public ListCollectionView OutputCollection
    {
        get
        {
            return (ListCollectionView)GetValue(OutputCollectionProperty);
        }
        private set
        {
            SetValue(_outputCollectionPropertyKey, value);
        }
    }

    private void ShowSelectedRegion()
    {
        if (_selectedRegion != null)
        {
            _selectedRegion.SetValue(DocumentAttachedProperties.ShowInputOutputDesignationProperty, true);
        }
    }

    private void HideSelectedRegion()
    {
        if (_selectedRegion != null)
        {
            _selectedRegion.ClearValue(DocumentAttachedProperties.ShowInputOutputDesignationProperty);
        }
    }

    protected virtual void OnActivated(RoutedEventArgs args) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    protected virtual void OnDeactivated(RoutedEventArgs args) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

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
        IOItem ioItem = RetrieveAssociatedIOItem(sender, out element);
        if (DebugTools.AssertNotNull(ioItem, null).Passed && ioItem.Region == _selectedRegion)
        {
            ShowIOItemInWorksheet(ioItem);
        }
    }

    private void ListViewItemSelected(object sender, RoutedEventArgs e)
    {
        FrameworkElement element;
        IOItem ioItem = RetrieveAssociatedIOItem(sender, out element);
        if (DebugTools.AssertNotNull(ioItem, null).Failed)
        {
            return;
        }

        element.BringIntoView();
        ioItem.ShowInWorksheet();
        HideSelectedRegion();
        _selectedRegion = ioItem.Region;
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

    private void ColumnHeaderClickedHandler(object sender, RoutedEventArgs e)
    {
        GridViewColumnHeader columnHeader = sender as GridViewColumnHeader;
        if (DebugTools.AssertNotNull(columnHeader, null).Failed || DebugTools.AssertNotNull(columnHeader.Column, null).Failed || DebugTools.AssertNotNull(columnHeader.Column.Header, null).Failed)
        {
            return;
        }

        ColumnViewModel columnViewModel = columnHeader.Column.Header as ColumnViewModel;
        if (DebugTools.AssertNotNull(columnViewModel, null).Failed)
        {
            return;
        }

        columnViewModel.SortByNextOrder();
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "10.0.11.0")]
    public void InitializeComponent()
    {
        if (_contentLoaded)
        {
            return;
        }

        _contentLoaded = true;
        Uri resourceLocator = new Uri("/Quire;component/inputoutputdesignationwindow.xaml", UriKind.Relative);
        Application.LoadComponent(this, resourceLocator);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "10.0.11.0")]
    internal Delegate _CreateDelegate(Type delegateType, string handler)
    {
        return Delegate.CreateDelegate(delegateType, this, handler);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "10.0.11.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
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

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "10.0.11.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IStyleConnector.Connect(int connectionId, object target)
    {
        EventSetter eventSetter;
        switch (connectionId)
        {
            case 2:
            {
                eventSetter = new EventSetter();
                eventSetter.Event = UIElement.MouseDownEvent;
                eventSetter.Handler = new MouseButtonEventHandler(ColumnHeaderClickedHandler);
                ((Style)target).Setters.Add(eventSetter);
                break;
            }
            case 3:
            {
                eventSetter = new EventSetter();
                eventSetter.Event = UIElement.MouseDownEvent;
                eventSetter.Handler = new MouseButtonEventHandler(HandleMouseDown);
                ((Style)target).Setters.Add(eventSetter);
                break;
            }
            case 4:
            {
                eventSetter = new EventSetter();
                eventSetter.Event = ListBoxItem.SelectedEvent;
                eventSetter.Handler = new RoutedEventHandler(ListViewItemSelected);
                ((Style)target).Setters.Add(eventSetter);
                break;
            }
        }
    }

    private DependencyObject _selectedRegion;

    internal InputOutputDesignationWindow InputOutputDesignation;

    private ScrollViewer _rootElement;

    internal ListView InputsListView;

    internal ListView OutputsListView;

    private bool _contentLoaded;
}
