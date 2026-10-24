#nullable disable
using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;

namespace Quire.Wpf;

public static partial class WpfUtils
{
    // Keep before ExecuteTask to preserve its generated callback ordinal.
    public static void CreateVisualTreeForPrinting(global::System.Windows.Controls.Primitives.Popup popup, long maxUpdateLayoutTimeMsec)
    {
        int attemptNumber;
        int nLoadingAttempts;
        int totalLoadingAttempts;
        global::System.Diagnostics.Stopwatch stopwatch;
        string text;
        bool isTreeFullyLoaded;
        global::Quire.DebugTools.LogData("WpfUtils.CreateVisualTreeForPrinting: Enter");
        global::Quire.DebugTools.DevAssert(!popup.IsOpen, null);
        DoEvents(Dispatcher.CurrentDispatcher, (DispatcherPriority)8);
        popup.IsOpen = true;
        stopwatch = new global::System.Diagnostics.Stopwatch();
        stopwatch.Start();
        text = (popup.Child as FrameworkElement)?.TryFindResource("WaitToPrintoutReadyQuestionMessage") as string;
        if (global::Quire.DebugTools.AssertNotNull(text, "WaitToPrintoutReadyQuestionMessage").Failed)
        {
            text = "WaitToPrintoutReadyQuestionMessage";
        }
        attemptNumber = (totalLoadingAttempts = nLoadingAttempts = 1);
        // Keep the tree checks at the loop head to preserve Mathcad's Release branch order.
        while (true)
        {
            if (IsVisualTreeFullyUpdated(popup.Child, out isTreeFullyLoaded))
            {
                break;
            }
            if (!IsWaitingForPrintoutReady(stopwatch, in maxUpdateLayoutTimeMsec, in isTreeFullyLoaded, in attemptNumber, ref nLoadingAttempts, ref totalLoadingAttempts, text))
            {
                break;
            }
            if (!isTreeFullyLoaded && nLoadingAttempts <= 2)
            {
                nLoadingAttempts++;
                popup.Width = popup.Width == 0.0 ? 1 : 0;
            }
            popup.UpdateLayout();
            attemptNumber++;
        }
        if (stopwatch.IsRunning)
        {
            stopwatch.Stop();
        }
        global::Quire.DebugTools.DevAssert(totalLoadingAttempts <= 2, null);
    }

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
