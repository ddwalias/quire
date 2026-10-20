#nullable disable
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
    public static double InchesToDips(double inches) => inches * 96.0;

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
}
