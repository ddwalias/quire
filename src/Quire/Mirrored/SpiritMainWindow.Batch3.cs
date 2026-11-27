#nullable disable

namespace Quire;

public partial class SpiritMainWindow
{
    private void HandleDocumentZoomIn(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        int steps = (int)e.Parameter;
        ZoomControl.ChangeZoomBySteps(steps);
    }

    private void DocumentPropertyChanged(object sender, global::System.ComponentModel.PropertyChangedEventArgs eventArgs)
    {
        if (eventArgs.PropertyName == "ReadonlyOptions")
        {
            AnyDocumentRequestToUpdateInputsEnabledCheck();
        }
    }

    private void HelpManagerPropertyChanged(object sender, global::System.ComponentModel.PropertyChangedEventArgs args)
    {
        if (args.PropertyName == "HelpServerPath")
        {
            foreach (global::Quire.Controls.Core.IEngineeringDocument openDocument in OpenDocumentsList)
            {
                openDocument.HelpServerPath = _helpManager.HelpServerPath;
            }
        }
    }

    private void OnEngNotebookConnectionStatusChanged()
    {
        global::Quire.AppCommands.Exit.Execute(null, this);
    }

    private void HandleException(Exception exception, string message)
    {
        if (exception != null)
        {
            string logFile = global::Quire.Services.AppServiceRegistry.GetService<global::Quire.Controls.Core.IPrimeExceptionLogger>().LogException(exception);
            try
            {
                global::Quire.Wpf.WpfUtils.DoEvents();
            }
            catch (Exception ex)
            {
                global::Quire.DebugTools.FailAssertion(ex.Message);
            }
            MessageBoxManager.ShowExceptionMessage(string.Format(message, logFile));
        }
    }

    private void ContextMenuOpeningEventHandler(object sender, global::System.Windows.Controls.ContextMenuEventArgs e)
    {
        global::System.Windows.FrameworkElement source = e.Source as global::System.Windows.FrameworkElement;
        if (source == null)
        {
            return;
        }
        global::System.Windows.Controls.ContextMenu contextMenu = source.ContextMenu;
        if (contextMenu != null && contextMenu.Name == "SystemQatCustomizeContextMenu")
        {
            for (int i = 0; i < contextMenu.Items.Count; i++)
            {
                global::System.Windows.Controls.MenuItem menuItem = contextMenu.Items.GetItemAt(i) as global::System.Windows.Controls.MenuItem;
                if (menuItem != null && menuItem.Name == "SystemCustomizeQatMenuItem")
                {
                    contextMenu.Items.RemoveAt(i);
                    ContextMenuOpening -= ContextMenuOpeningEventHandler;
                    break;
                }
            }
        }
    }
}
