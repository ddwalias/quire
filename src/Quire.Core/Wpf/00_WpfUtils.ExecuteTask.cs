#nullable disable
using System;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Threading;

namespace Quire.Wpf;

public static partial class WpfUtils
{
    public static object TryFindFirstAncestorIncludingSelfLimited<T>(DependencyObject child, global::System.Collections.Generic.IList<Type> types, bool includeLogical = false) where T : class
    {
        if (child == null)
        {
            return null;
        }
        Type type = ((object)child).GetType();
        Type[] interfaces = type.GetInterfaces();
        foreach (Type type3 in types)
        {
            Type type1 = type3;
            if (interfaces.Any((Type @interface) => @interface == type1) || type == type1)
            {
                return child;
            }
        }
        if (child is T)
        {
            return null;
        }
        return TryFindFirstAncestorIncludingSelfLimited<T>(GetParentObject(child, includeLogical), types, includeLogical);
    }

    public static T TryFindAncestorIncludingSelfUntilGivenAncestorIsReached<T>(this DependencyObject child, DependencyObject topParent, bool includeLogical = false) where T : class
    {
        if (child == null)
        {
            return null;
        }
        if (child is T result)
        {
            return result;
        }
        if (topParent == child)
        {
            return null;
        }
        return child.TryFindVisualAncestorUntilGivenAncestorIsReached<T>(topParent, includeLogical);
    }

    public static T TryFindVisualAncestorUntilGivenAncestorIsReached<T>(this DependencyObject child, DependencyObject topParent, bool includeLogical = false) where T : class => TryFindAncestorIncludingSelfUntilGivenAncestorIsReached<T>(GetParentObject(child, includeLogical), topParent, includeLogical);

    public static T1 TryFindAncestorIncludingSelfLimited<T1, T2>(this DependencyObject child, bool includeLogical = false) where T1 : class where T2 : class
    {
        if (child == null)
        {
            return null;
        }
        if (child is T1 result)
        {
            return result;
        }
        if (child is T2)
        {
            return null;
        }
        return child.TryFindVisualAncestorLimited<T1, T2>(includeLogical);
    }

    public static T1 TryFindVisualAncestorLimited<T1, T2>(this DependencyObject child, bool includeLogical = false) where T1 : class where T2 : class => TryFindAncestorIncludingSelfLimited<T1, T2>(GetParentObject(child, includeLogical), includeLogical);

    public static T TryFindAncestorIncludingSelf<T>(this DependencyObject child, bool includeLogical = false) where T : class
    {
        if (child == null)
        {
            return null;
        }
        if (child is T result)
        {
            return result;
        }
        return child.TryFindVisualAncestor<T>(includeLogical);
    }

    public static T TryFindVisualAncestor<T>(this DependencyObject child, bool includeLogical = false) where T : class => TryFindAncestorIncludingSelf<T>(GetParentObject(child, includeLogical), includeLogical);

    public static DependencyObject TryFindVisualAncestorByString(string ObjectName, DependencyObject child)
    {
        DependencyObject current = child;
        while (current != null)
        {
            current = VisualTreeHelper.GetParent(current);
            if (current?.GetType().Name == ObjectName)
            {
                return current;
            }
        }
        return null;
    }

    public static DependencyObject GetParentObject(DependencyObject child, bool includeLogical = false)
    {
        if (child == null)
        {
            return null;
        }
        if (child is ContentElement contentElement)
        {
            DependencyObject parent = ContentOperations.GetParent(contentElement);
            if (parent != null)
            {
                return parent;
            }
            if (!(contentElement is FrameworkContentElement frameworkContentElement))
            {
                return null;
            }
            return frameworkContentElement.Parent;
        }
        try
        {
            DependencyObject parent2 = VisualTreeHelper.GetParent(child);
            if ((parent2 == null) & includeLogical)
            {
                parent2 = LogicalTreeHelper.GetParent(child);
            }
            return parent2;
        }
        catch
        {
            return null;
        }
    }

    public static T FindLogicalAncestorIncludingSelf<T>(DependencyObject currentObject) where T : DependencyObject
    {
        if (currentObject == null)
        {
            return default;
        }
        T val = currentObject as T;
        if (val != null)
        {
            return val;
        }
        return FindLogicalAncestor<T>(currentObject);
    }

    public static T FindLogicalAncestor<T>(DependencyObject child) where T : DependencyObject => FindLogicalAncestorIncludingSelf<T>(LogicalTreeHelper.GetParent(child));

    public static T FindVisualDescendantIncludingSelf<T>(DependencyObject currentObject) where T : DependencyObject
    {
        if (currentObject == null)
        {
            return default;
        }
        T val = currentObject as T;
        if (val != null)
        {
            return val;
        }
        return FindVisualDescendant<T>(currentObject);
    }

    public static T FindVisualDescendant<T>(DependencyObject parent) where T : DependencyObject
    {
        if (parent == null)
        {
            return default;
        }
        int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
        for (int i = 0; i < childrenCount; i++)
        {
            DependencyObject child = VisualTreeHelper.GetChild(parent, i);
            T val = child as T;
            if (val != null)
            {
                return val;
            }
            T val2 = FindVisualDescendant<T>(child);
            if (val2 != null)
            {
                return val2;
            }
        }
        return default;
    }

    [global::Quire.Mirror.MirrorVerified("PtcWpfWpfUtilsMechanicalDifferentialTests")]
    public static void QueueBackgroundTask(MethodInvoker work) => ExecuteTask(work, true);

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

    public static string ApplicationName => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public static string ProductName
    {
        get
        {
            if (global::Quire.License.LicenseManager.Instance != null)
            {
                if (global::Quire.Services.ApplicationVariant.IsEngNbPlus.GetValueOrDefault())
                {
                    return "Creo+ Engineering Notebook";
                }

                if (global::Quire.Services.ApplicationVariant.IsEngNb.GetValueOrDefault())
                {
                    return "Creo Engineering Notebook";
                }
            }

            return "PTC Mathcad";
        }
    }

    public static string ApplicationFullName => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    // Keep the tree checks at the loop head to preserve Mathcad's Release branch order.
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
            flag = DisplayQuestionMessage(message) == MessageBoxResult.Yes;
            if (flag == true)
            {
                nLoadingAttempts = 1;
                stopwatch.Start();
            }
        }
        return flag ?? true;
    }

    public static bool ShouldSuspendInputMethod()
    {
        if (SystemInfo.OsBeforeXp())
        {
            return EastAsianLanguagesInstalled;
        }
        return false;
    }

    public static void DoWork(Action work)
    {
        global::System.Windows.Input.Mouse.OverrideCursor = global::System.Windows.Input.Cursors.Wait;
        try
        {
            work();
        }
        finally
        {
            global::System.Windows.Input.Mouse.OverrideCursor = null;
        }
    }

}
