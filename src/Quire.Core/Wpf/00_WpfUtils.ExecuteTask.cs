#nullable disable
using System;
using System.Windows.Forms;
using System.Windows.Threading;

namespace Quire.Wpf;

public static partial class WpfUtils
{
    // Keep this file before WpfUtils.cs so ExecuteTask retains Mathcad's callback ordinal.
    public static void ExecuteTask(MethodInvoker work, bool async)
    {
        DispatcherFrame nestedFrame = new DispatcherFrame();
        Dispatcher.CurrentDispatcher.BeginInvoke((DispatcherPriority)4, (Delegate)(DispatcherOperationCallback)delegate
        {
            work();
            nestedFrame.Continue = false;
            return (object)null;
        }, (object)nestedFrame);
        if (!async)
        {
            Dispatcher.PushFrame(nestedFrame);
        }
    }
}
