#nullable disable
using System;
using System.Windows.Forms;
using System.Windows.Threading;

namespace Quire.Wpf;

public static partial class WpfUtils
{
    private static bool IsWaitingForPrintoutReady(global::System.Diagnostics.Stopwatch stopwatch, in long maxUpdateLayoutTimeMsec, in bool isTreeLoaded, in int attemptNumber, ref int nLoadingAttempts, ref int totalLoadingAttempts, string message)
    {
        if (!isTreeLoaded)
        {
            totalLoadingAttempts++;
        }
        if (attemptNumber <= 4)
        {
            return true;
        }
        bool? flag = null;
        if (stopwatch.ElapsedMilliseconds > maxUpdateLayoutTimeMsec)
        {
            stopwatch.Reset();
            flag = DisplayQuestionMessage(message) == System.Windows.MessageBoxResult.Yes;
            if (flag == true)
            {
                nLoadingAttempts = 1;
                stopwatch.Start();
            }
        }
        return flag ?? true;
    }

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
