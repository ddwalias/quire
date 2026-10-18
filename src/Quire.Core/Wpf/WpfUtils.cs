#nullable disable
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Threading;
using Quire.Mirror;

namespace Quire.Wpf;

public static partial class WpfUtils
{
    [MirrorVerified("PtcWpfWpfUtilsMechanicalDifferentialTests")]
    public static void QueueBackgroundTask(MethodInvoker work) => ExecuteTask(work, true);

    [MirrorVerified("PtcWpfWpfUtilsMechanicalDifferentialTests")]
    public static void Stall(int milliseconds) => Stall(milliseconds, (DispatcherPriority)9);

    [MirrorVerified("PtcWpfWpfUtilsMechanicalDifferentialTests")]
    public static DependencyObject GetVisualParent(this Visual visual) => VisualTreeHelper.GetParent(visual);
}
