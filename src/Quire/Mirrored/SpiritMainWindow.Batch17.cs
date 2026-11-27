#nullable disable

namespace Quire;

public partial class SpiritMainWindow
{
    private bool ClassifyWriteFailure(
        global::System.Exception exception,
        string fileName,
        out string displayedMessage)
    {
        displayedMessage = null;
        bool handled;
        global::System.Exception exceptionCopy = exception;
        switch (exceptionCopy)
        {
            case global::Quire.IO.FileIsUsedByAnotherProcessException:
            {
                displayedMessage = exception.InnerException?.Message;
                if (displayedMessage.IsNullOrEmpty())
                {
                    displayedMessage = TryFindResource("Application.InUseByAnother") as string;
                }

                handled = true;
                break;
            }
            case global::System.UnauthorizedAccessException:
            {
                global::System.Nullable<bool> isReadonly = IsReadonly(fileName);
                if (isReadonly.GetValueOrDefault())
                {
                    displayedMessage = TryFindResource("Application.FileIsReadOnly") as string;
                    handled = true;
                }
                else
                {
                    // Mathcad sends a false nullable result through the shared default assignment.
                    goto default;
                }

                break;
            }
            default:
                handled = false;
                break;
        }

        return handled;
    }

    private nint WndProc(nint hwnd, int msg, nint wparam, nint lparam, ref bool handled)
    {
        if ((long)msg == 271)
        {
            global::System.Windows.IInputElement focusedElementInSameScope =
                global::Quire.Wpf.WpfUtils.GetFocusedElementInSameScope(this);
            global::Quire.Controls.Core.IImeClient imeClient =
                global::Quire.Wpf.WpfUtils.TryFindAncestorIncludingSelf<global::Quire.Controls.Core.IImeClient>(
                    focusedElementInSameScope as global::System.Windows.DependencyObject,
                    includeLogical: false);
            if (imeClient != null)
            {
                global::Quire.Controls.Core.ImeHandler.PositioningIme(imeClient);
            }
        }

        return global::System.IntPtr.Zero;
    }
}
