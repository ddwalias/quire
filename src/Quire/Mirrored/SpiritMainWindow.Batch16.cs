#nullable disable

namespace Quire;

public partial class SpiritMainWindow
{
    private void UnhandledDispatcherExceptionHandler(
        object sender,
        global::System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
    {
        if (_handlingException)
        {
            e.Handled = true;
            return;
        }

        _handlingException = true;
        string[] message =
        [
            "An Unhandled exception occurred on the dispatcher thread. See",
            "{0}",
            "for more information. Prime will attempt to save any modified files and close.",
            "Please restart the application."
        ];
        if (!e.Handled)
        {
            try
            {
                HandleException(e.Exception, string.Join(global::System.Environment.NewLine, message));
                global::Quire.Controls.Core.IEngineeringDocument activeDocument = ActiveDocument;
                if (AttemptToSaveWorksheetCopy(activeDocument))
                {
                    CloseDocument(activeDocument, forceClose: true);
                }
            }
            catch (global::System.Exception exception)
            {
                global::Quire.DebugTools.FailAssertion(exception.Message);
            }
        }

        e.Handled = true;
        Close();
        _handlingException = false;
    }

    private void UnhandledExceptionHandler(object sender, global::System.UnhandledExceptionEventArgs args)
    {
        if (_handlingException)
        {
            return;
        }

        _handlingException = true;
        string[] message =
        [
            "An Unhandled exception occurred. See",
            "{0}",
            "for more information. Prime will attempt to save any modified files and close.",
            "Please restart the application."
        ];
        global::System.Exception exception = args.ExceptionObject as global::System.Exception;
        HandleException(exception, string.Join(global::System.Environment.NewLine, message));
        if (((global::System.Windows.Threading.DispatcherObject)this).Dispatcher.CheckAccess())
        {
            Close();
        }
        else
        {
            ((global::System.Windows.Threading.DispatcherObject)this).Dispatcher.BeginInvoke(
                (global::System.Delegate)new global::System.Action(Close),
                (global::System.Windows.Threading.DispatcherPriority)10,
                global::System.Array.Empty<object>());
        }

        _handlingException = false;
    }
}
