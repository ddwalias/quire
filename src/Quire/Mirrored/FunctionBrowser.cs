#nullable disable
#pragma warning disable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Threading;
using Quire.Controls.Core;
using Quire.Controls;

namespace Quire;

public partial class FunctionBrowser
{
    public static Type _typeOfthis = typeof(FunctionBrowser);
    public static readonly string FunctionViewByCategory = "FunctionViewByCategory";
    public static readonly string FunctionViewByName = "FunctionViewByName";
    public static readonly string FunctionLabelName = "PART_FunctionLabel";
    public static readonly string HeaderName = "PART_Header";
    private DispatcherTimer _refreshTimer;
    public static readonly DependencyProperty DocumentActiveItemProperty = DependencyProperty.Register("DocumentActiveItem", typeof(IWhiteboardItem), _typeOfthis, new PropertyMetadata());

    public IWhiteboardItem DocumentActiveItem
    {
        get => (IWhiteboardItem)GetValue(DocumentActiveItemProperty);
        set => SetValue(DocumentActiveItemProperty, value);
    }

    public FunctionBrowser()
    {
        InitializeComponent();
        _refreshTimer = new DispatcherTimer();
        _refreshTimer.Stop();
        _refreshTimer.Tick += UpdateView;
        FunctionsTreeView.ItemContainerGenerator.StatusChanged += ItemContainerGeneratorOnStatusChanged;
        CommandBindings.Add(new CommandBinding(EditingCommands.EnterParagraphBreak, SearchCommandExecuted));
        CommandBindings.Add(new CommandBinding(EditingCommands.TabForward, ForwardTabNavigate, CanTabNavigate));
    }

    private void ItemContainerGeneratorOnStatusChanged(object sender, EventArgs e)
    {
        ItemContainerGenerator generator = sender as ItemContainerGenerator;
        if (generator == null || generator.Status != GeneratorStatus.ContainersGenerated)
        {
            return;
        }
        UpdateSelectedItemsOnCollectionChanged();
        Activate();
        FunctionsTreeView.ItemContainerGenerator.StatusChanged -= ItemContainerGeneratorOnStatusChanged;
    }

    private void UpdateSelectedItemsOnCollectionChanged()
    {
        if (FunctionsTreeView.SelectedItem == null && !FunctionsTreeView.Items.IsEmpty && FunctionsTreeView.ItemContainerGenerator.ContainerFromItem(FunctionsTreeView.Items[0]) is TreeViewItem treeViewItem)
        {
            treeViewItem.IsSelected = true;
        }
        if (FunctionsListView != null && FunctionsListView.SelectedItem == null && FunctionsListView.Items.Count > 0)
        {
            FunctionsListView.SelectedItem = FunctionsListView.Items[0];
        }
    }

    public void Activate()
    {
        IInputElement focusedElement = FocusManager.GetFocusedElement(this);
        if (focusedElement == null)
        {
            FocusManager.SetFocusedElement(this, matchingTextBox);
            matchingTextBox.Focus();
        }
        else
        {
            focusedElement.Focus();
        }
    }

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        base.OnMouseDown(e);
        if (!IsKeyboardFocusWithin)
        {
            Activate();
        }
    }

    private void CanTabNavigate(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = GetActiveFocusedElement(out var _);
    }

    private void ForwardTabNavigate(object sender, ExecutedRoutedEventArgs e)
    {
        if (GetActiveFocusedElement(out var activeFocusedElement))
        {
            e.Handled = activeFocusedElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }
    }

    private bool GetActiveFocusedElement(out UIElement activeFocusedElement)
    {
        activeFocusedElement = (IsKeyboardFocusWithin && FocusManager.GetFocusedElement(this) is UIElement uIElement) ? uIElement : null;
        return activeFocusedElement != null;
    }

    private void matchingTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        Console.WriteLine("Text Change: {0}", matchingTextBox.Text);
        _refreshTimer.Stop();
        _refreshTimer.Interval = new TimeSpan((matchingTextBox.Text.Length < 3) ? 20000000 : 5000000);
        Console.WriteLine("Timer int. : {0}", _refreshTimer.Interval);
        _refreshTimer.Start();
    }

    private void UpdateView(object o, EventArgs e)
    {
        Console.WriteLine("Update     : {0}", matchingTextBox.Text);
        _refreshTimer.Stop();
        UpdateView();
    }

    private void UpdateView()
    {
        ((CollectionViewSource)Resources[FunctionViewByCategory]).View.Refresh();
        ((CollectionViewSource)Resources[FunctionViewByName]).View.Refresh();
        UpdateSelectedItemsOnCollectionChanged();
    }

    private void FunctionFilter(object sender, FilterEventArgs e)
    {
        VisualFunctionDescriptor visualFunctionDescriptor = (VisualFunctionDescriptor)((DictionaryEntry)e.Item).Value;
        string text = visualFunctionDescriptor.DisplayName.ToLower();
        string text2 = matchingTextBox.Text.ToLower();
        string text3 = visualFunctionDescriptor.Description.ToLower();
        e.Accepted = text.Contains(text2) || text3.Contains(text2) || ContainedInCategories(text2, visualFunctionDescriptor.Categories);
    }

    private void SearchCommandExecuted(object sender, RoutedEventArgs e)
    {
        UpdateView();
        if (!matchingTextBox.IsKeyboardFocused)
        {
            matchingTextBox.Focus();
        }
    }

    private void RestoreFocusOnActiveDocument()
    {
        DocumentActiveItem?.Activate(isActive: true);
    }

    private void HandleItemCommandExecuted(object sender, RoutedEventArgs e)
    {
        RestoreFocusOnActiveDocument();
    }

    private void MoveFocusToLabel(object sender, KeyboardFocusChangedEventArgs e)
    {
        if (sender is Control control && (sender is ListViewItem || sender is TreeViewItem))
        {
            FunctionLabel notFocusedChildLabel = GetNotFocusedChildLabel(control);
            if (notFocusedChildLabel != null)
            {
                Dispatcher.CurrentDispatcher.BeginInvoke(new Action<FunctionLabel, Control>(DoMoveFocus), DispatcherPriority.Render, new object[2] { notFocusedChildLabel, control });
            }
        }
    }

    private void TabControl_OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if ((int)e.Key == 6)
        {
            e.Handled = !(e.OriginalSource is FunctionLabel);
        }
    }

    void global::System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target)
    {
        switch (connectionId)
        {
            case 1:
                ((CollectionViewSource)target).Filter += FunctionFilter;
                return;
            case 2:
                ((CollectionViewSource)target).Filter += FunctionFilter;
                return;
            case 5:
                matchingTextBox = (TextBox)target;
                matchingTextBox.TextChanged += matchingTextBox_TextChanged;
                return;
            case 6:
                ((global::DevComponents.WpfRibbon.ButtonEx)target).Click += SearchCommandExecuted;
                return;
            case 7:
                ((TabControl)target).PreviewKeyDown += TabControl_OnPreviewKeyDown;
                return;
            case 8:
                FunctionsTreeView = (TreeView)target;
                return;
            case 9:
                FunctionsListView = (ListView)target;
                return;
        }
        _contentLoaded = true;
    }

    [global::System.Diagnostics.DebuggerNonUserCode]
    public void InitializeComponent()
    {
        if (_contentLoaded)
        {
            return;
        }
        _contentLoaded = true;
        global::System.Uri resourceLocator = new global::System.Uri("/Quire;component/functionbrowser.xaml", global::System.UriKind.Relative);
        global::System.Windows.Application.LoadComponent(this, resourceLocator);
    }

    [global::System.Diagnostics.DebuggerNonUserCode]
    internal global::System.Delegate _CreateDelegate(global::System.Type delegateType, string handler)
    {
        return global::System.Delegate.CreateDelegate(delegateType, this, handler);
    }
}
