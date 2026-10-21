#nullable disable
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Threading;
using Quire.Mirror;
using Quire.Mirror;
using Quire.Services;

namespace Quire.Wpf;

public static partial class WpfUtils
{
    [MirrorVerified("PtcWpfWpfUtilsMechanicalDifferentialTests")]
    public static void QueueBackgroundTask(MethodInvoker work) => ExecuteTask(work, true);

    [MirrorVerified("PtcWpfWpfUtilsMechanicalDifferentialTests")]
    public static void Stall(int milliseconds) => Stall(milliseconds, (DispatcherPriority)9);

    [MirrorVerified("PtcWpfWpfUtilsMechanicalDifferentialTests")]
    public static DependencyObject GetVisualParent(this Visual visual) => VisualTreeHelper.GetParent(visual);

    [MirrorVerified("Release IL")]
    public static string DumpLogicalTree(object root) => string.Join(Environment.NewLine, DumpLogicalTreeLines(root));

    [MirrorVerified("Release IL")]
    public static IInputElement GetFocusedElementInSameScope(this UIElement element) => FocusManager.GetFocusedElement(FocusManager.GetFocusScope(element));

    [MirrorVerified("Release IL")]
    public static T GetFocusedAncestorOfTypeElement<T>(UIElement subElement, bool includeLogical = false) where T : DependencyObject => TryFindVisualAncestor<T>(GetFocusedElementInSameScope(subElement) as DependencyObject, includeLogical);

    [MirrorVerified("Release IL")]
    public static T TryFindVisualAncestorUntilGivenAncestorIsReached<T>(this DependencyObject child, DependencyObject topParent, bool includeLogical = false) where T : class => TryFindAncestorIncludingSelfUntilGivenAncestorIsReached<T>(GetParentObject(child, includeLogical), topParent, includeLogical);

    [MirrorVerified("Release IL")]
    public static T1 TryFindVisualAncestorLimited<T1, T2>(this DependencyObject child, bool includeLogical = false) where T1 : class where T2 : class => TryFindAncestorIncludingSelfLimited<T1, T2>(GetParentObject(child, includeLogical), includeLogical);

    [MirrorVerified("Release IL")]
    public static T TryFindVisualAncestor<T>(this DependencyObject child, bool includeLogical = false) where T : class => TryFindAncestorIncludingSelf<T>(GetParentObject(child, includeLogical), includeLogical);

    [MirrorVerified("Release IL")]
    public static T FindLogicalAncestor<T>(DependencyObject child) where T : DependencyObject => FindLogicalAncestorIncludingSelf<T>(LogicalTreeHelper.GetParent(child));

    [MirrorVerified("Release IL")]
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

    [MirrorVerified("Release IL")]
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

    [MirrorVerified("Release IL")]
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

    [MirrorVerified("Release IL")]
    public static double InchesToDips(double inches) => inches * 96.0;

    [MirrorVerified("Release IL")]
    public static double DistancePointToPoint(Point pt1, Point pt2)
    {
        return Math.Sqrt(Math.Pow(pt1.X - pt2.X, 2.0) + Math.Pow(pt1.Y - pt2.Y, 2.0));
    }

    [MirrorVerified("Release IL")]
    public static FrameworkElement GetClosestAncestor(FrameworkElement ancestor, FrameworkElement descendant)
    {
        while (descendant != null && (object)descendant.Parent != ancestor)
        {
            descendant = (FrameworkElement)(object)descendant.Parent;
        }
        return descendant;
    }

    [MirrorVerified("Release IL")]
    public static System.Drawing.Color WpfToGdi(System.Windows.Media.Color color)
    {
        return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
    }

    [MirrorVerified("Release IL")]
    public static System.Windows.Media.Color GdiToWpf(System.Drawing.Color color)
    {
        return System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
    }

    [MirrorVerified("Release IL")]
    public static uint WpfColorToRgb(System.Windows.Media.Color color)
    {
        return (uint)((color.R << 16) | (color.G << 8) | color.B);
    }

    [MirrorVerified("Release IL")]
    public static double DipsToInches(double dips) => dips / 96.0;

    [MirrorVerified("Release IL")]
    public static double CmToDips(double cm) => cm * 37.79527559055118;

    [MirrorVerified("Release IL")]
    public static double DipsToCm(double dips) => dips / 37.79527559055118;

    [MirrorVerified("Release IL")]
    public static int DipsToHimetric(double dips) => DoubleExtension.IntRound(Math.Round(dips, 5) / 37.79527559055118 * 1000.0);

    [MirrorVerified("Release IL")]
    public static double HimetricToDips(int himetric) => himetric / 1000.0 * 37.79527559055118;

    [MirrorVerified("Release IL")]
    public static double PointsToDips(double points) => points / 72.0 * 96.0;

    [MirrorVerified("Release IL")]
    public static double DipsToPoints(double dips) => dips / 96.0 * 72.0;

    [MirrorVerified("Release IL")]
    public static Color WpfColorFromName(string name) => (Color)ColorConverter.ConvertFromString(name);

    [MirrorVerified("Release IL")]
    public static Brush CreateSolidWpfBrush(global::System.Drawing.Color color) => new SolidColorBrush(GdiToWpf(color));

    [MirrorVerified("Release IL")]
    public static void Blink(UIElement element, int period) => Blink(EnumerableExtensions.Yield<UIElement>(element), period);

    [MirrorVerified("Release IL")]
    public static void DoEvents() => DoEvents(Dispatcher.CurrentDispatcher, (DispatcherPriority)4);

    [MirrorVerified("Release IL")]
    public static bool WaitForIdle(Thread thread) => WaitForIdle(thread, TimeSpan.FromMilliseconds(-1L));

    [MirrorVerified("Release IL")]
    public static void ForceRendering(UIElement pageVisual, Size sz)
    {
        pageVisual.Measure(sz);
        pageVisual.Arrange(new Rect(new Point(0.0, 0.0), sz));
        pageVisual.UpdateLayout();
    }

    [MirrorVerified("Release IL")]
    public static object GetDefaultValueFromAttribute(object component, string propertyName) => ((DefaultValueAttribute)TypeDescriptor.GetProperties(component)[propertyName].Attributes[typeof(DefaultValueAttribute)]).Value;

    [MirrorVerified("Release IL")]
    private static MessageBoxResult DisplayQuestionMessage(string message) => AppServiceRegistry.GetService<IMessageBoxManager>().ShowCriticalQuestionMessage.Invoke(global::System.Windows.Application.Current.MainWindow, message, ApplicationFullName, (MessageBoxButton)4);

    public static global::System.Collections.Generic.IList<global::System.Windows.Documents.Adorner> GetAdornersOfDescendants(UIElement element)
    {
        global::System.Collections.Generic.IList<global::System.Windows.Documents.Adorner> list = new global::System.Collections.Generic.List<global::System.Windows.Documents.Adorner>();
        GetAdornersOfDescendants(element, list);
        return list;
    }

    private static void GetAdornersOfDescendants(UIElement element, global::System.Collections.Generic.ICollection<global::System.Windows.Documents.Adorner> adorners)
    {
        int childrenCount;
        global::System.Windows.Documents.AdornerLayer adornerLayer;
        global::System.Windows.Documents.Adorner[] adornersOfElement;
        int i;
        if (element == null)
        {
            return;
        }
        childrenCount = VisualTreeHelper.GetChildrenCount(element);
        i = 0;
        while (i < childrenCount)
        {
            GetAdornersOfDescendants(VisualTreeHelper.GetChild(element, i) as UIElement, adorners);
            i++;
        }
        adornerLayer = global::System.Windows.Documents.AdornerLayer.GetAdornerLayer(element);
        if (adornerLayer == null)
        {
            return;
        }
        adornersOfElement = adornerLayer.GetAdorners(element);
        if (adornersOfElement == null)
        {
            return;
        }
        adornersOfElement.Each(adorners.Add);
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
        Mouse.OverrideCursor = global::System.Windows.Input.Cursors.Wait;
        try
        {
            work();
        }
        finally
        {
            Mouse.OverrideCursor = null;
        }
    }

    public static bool IsAncestorOf(DependencyObject possibleAncestor, DependencyObject element)
    {
        try
        {
            while (element != possibleAncestor && element != null)
            {
                element = VisualTreeHelper.GetParent(element);
            }
            return element != null;
        }
        catch
        {
            return false;
        }
    }

    public static Brush ChangeBrushColor(Brush oldBrush, global::System.Drawing.Color newColor)
    {
        SolidColorBrush solidColorBrush = new SolidColorBrush(GdiToWpf(newColor));
        if (oldBrush != null)
        {
            solidColorBrush.Opacity = oldBrush.Opacity;
        }
        return solidColorBrush;
    }
}
