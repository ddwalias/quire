#nullable disable
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using Quire.Mirror;
using Quire.Wpf;

namespace Quire.Controls.Core;

public partial class ImeHandler
{
    public static void SetIsInputMethodSuspended(DependencyObject target)
    {
        if (WpfUtils.ShouldSuspendInputMethod())
        {
            InputMethod.SetIsInputMethodSuspended(target, true);
        }
    }

    public static void HandleImeFocus()
    {
        if (!(ControlSettings.Instance.HandleFocusForIme && WpfUtils.ShouldSuspendInputMethod()))
        {
            return;
        }

        UnsafeNativeMethods.ITfThreadMgr threadManager;
        UnsafeNativeMethods.TF_CreateThreadMgr(out threadManager);
        if (threadManager != null)
        {
            threadManager.SetFocus(null);
            Marshal.ReleaseComObject(threadManager);
        }
    }

    public static void PositioningIme(IImeClient client)
    {
        FrameworkElement element = client as FrameworkElement;
        if (element == null)
        {
            return;
        }

        if (WpfUtils.ShouldSuspendInputMethod())
        {
            Window window = Application.Current.MainWindow;
            WindowInteropHelper helper = new WindowInteropHelper(window);
            nint context = UnsafeNativeMethods.ImmGetContext(helper.Handle);
            if (UnsafeNativeMethods.ImmGetOpenStatus(context))
            {
                Point position = client.GetImeWindowPosition();
                element.BringIntoView(new Rect(position, position));
                Point point = element.TransformToAncestor(window).Transform(position);
                // Release IL keeps the form local ahead of the matrix local.
                UnsafeNativeMethods.COMPOSITIONFORM form;
                Matrix matrix = PresentationSource.FromVisual(window).CompositionTarget.TransformToDevice;
                point = matrix.Transform(point);
                form = default;
                form.dwStyle = 2;
                form.ptCurrentPos.x = (int)point.X;
                form.ptCurrentPos.y = (int)point.Y;
                UnsafeNativeMethods.ImmSetCompositionWindow(context, ref form);
            }

            UnsafeNativeMethods.ImmReleaseContext(context, helper.Handle);
        }
    }
}
