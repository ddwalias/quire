#nullable disable

namespace Quire;

public partial class SpiritMainWindow
{
    private T ShowMessage<T>(global::System.Func<global::System.Windows.Window, string, T> messageDisplayDelegate)
    {
        if (!MessageBoxManager.IsDisabled)
        {
            Activate();
        }
        return messageDisplayDelegate(this, global::Quire.Wpf.WpfUtils.ApplicationFullName);
    }

    private void HandlePrepareSnapshotStart(global::Quire.Controls.EngineeringDocument _)
    {
        global::System.Windows.Input.Mouse.OverrideCursor = global::System.Windows.Input.Cursors.Wait;
    }

    private void HandlePrepareSnapshotEnd(global::Quire.Controls.EngineeringDocument _)
    {
        global::System.Windows.Input.Mouse.OverrideCursor = null;
    }

    private void HandleWorksheetControlDebuggerModeChange(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
    }

    private void OnDocumentLoaded(global::Quire.Controls.Core.IEngineeringDocument document)
    {
        if (global::Quire.DebugTools.AssertNotNull(document, null).Failed)
        {
            return;
        }
        if (document.IsDirty)
        {
            OnDocumentModified(document);
        }
        document.IsDirtyChanged += OnDocumentModified;
    }

    private string GetUntitledHeader(bool incrementIndex)
    {
        int index = UseDocumentTabs ? _nextUntitledIndex : 0;
        if (incrementIndex)
        {
            if (UseDocumentTabs || _nextUntitledIndex == 0)
            {
                _nextUntitledIndex++;
            }
            global::Quire.DebugTools.DevAssert(_nextUntitledIndex > -1, null);
        }
        if (index == 0)
        {
            return (string)TryFindResource("Application.Untitled");
        }
        return string.Format((string)TryFindResource("Application.UntitledWithIndex"), index);
    }

    private static void HandleToggleMultithreadingCalculationOption(
        object sender,
        global::System.Windows.Input.ExecutedRoutedEventArgs args)
    {
        global::Quire.Properties.Settings.Default.NumberOfThreads =
            BooleanToMultiThreadingValueConverter.Convert(
                !MultiThreadingValueToBooleanConverter.Convert(global::Quire.Properties.Settings.Default.NumberOfThreads));
        global::Quire.Properties.Settings.Default.Save();
    }

    private void UpdateWarningsDisplayFilterHandler(
        object sender,
        global::System.Windows.Input.ExecutedRoutedEventArgs args)
    {
        if (args.Parameter is global::Quire.Controls.Core.WarningsTypesToShow displayWarningsFilter)
        {
            DisplayWarningsFilter = displayWarningsFilter;
        }
        else
        {
            global::Quire.DebugTools.FailAssertion(null, global::System.Array.Empty<object>());
        }
    }

    private void ToggleShowWarningsHandler(
        object sender,
        global::System.Windows.Input.ExecutedRoutedEventArgs args)
    {
        IsShowWarningsEnabled = !IsShowWarningsEnabled;
    }

    private void ToolTipOpened(object sender, global::System.Windows.RoutedEventArgs e)
    {
        global::System.Windows.Controls.ToolTip toolTip = e.Source as global::System.Windows.Controls.ToolTip;
        if (global::Quire.DebugTools.AssertNotNull(toolTip, null).Failed)
        {
            return;
        }
        _helpManager.ActiveToolTipUid = toolTip.Uid;
        global::Quire.Wpf.DependencyPropertyChangeManager.AddPropertyChangedHandler(
            global::System.Windows.UIElement.IsVisibleProperty,
            (global::System.Windows.DependencyObject)toolTip,
            HandleTooltipIsVisibleChanged);
    }

    private void CanFind(object sender, global::System.Windows.Input.CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = !FindIsNotAllowedInFreemiumMode()
            && !global::Quire.EnumerableExtensions.IsNullOrEmpty<char>(FindProperties.FindText);
    }

    protected override global::System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer()
    {
        return new global::Quire.Automation.SpiritMainWindowAutomationPeer(this);
    }

}
