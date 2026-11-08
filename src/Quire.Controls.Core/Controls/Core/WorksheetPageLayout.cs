#nullable disable
using System.ComponentModel;
using System.Windows;
using Quire.Mirror;
using Quire.Wpf;

namespace Quire.Controls.Core;

public partial class WorksheetPageLayout
{
    [MirrorVerified("PtcControlsCoreWorksheetPageLayoutMechanicalDifferentialTests")]
    private static Thickness WideMargin(Thickness startingMargin) => ScaledMargin(startingMargin, 2.0, 2.0);

    [MirrorVerified("PtcControlsCoreWorksheetPageLayoutMechanicalDifferentialTests")]
    private static Thickness NarrowMargin(Thickness startingMargin) => ScaledMargin(startingMargin, 0.5, 0.5);

    [MirrorVerified("PtcControlsCoreWorksheetPageLayoutMechanicalDifferentialTests")]
    private static double MetricMajorGridLines => WpfUtils.CmToDips(2.0);

    [MirrorVerified("PtcControlsCoreWorksheetPageLayoutMechanicalDifferentialTests")]
    private static double USMajorGridLines => WpfUtils.InchesToDips(1.0);

    [MirrorVerified("PtcControlsCoreWorksheetPageLayoutMechanicalDifferentialTests")]
    private static Thickness USFooterMargin => USHeaderMargin;

    [MirrorVerified("PtcControlsCoreWorksheetPageLayoutMechanicalDifferentialTests")]
    private static Thickness MetricFooterMargin => MetricHeaderMargin;

    [MirrorVerified("PtcControlsCoreWorksheetPageLayoutMechanicalDifferentialTests")]
    public event Action<Size, Size> GridCellSizeChanged;

    [MirrorVerified("PtcControlsCoreWorksheetPageLayoutMechanicalDifferentialTests")]
    public event Action<Size, Size> HeaderFooterGridCellSizeChanged;

    [MirrorVerified("PtcControlsCoreWorksheetPageLayoutMechanicalDifferentialTests")]
    public event Action WorksheetRecreateSuggested;

    [MirrorVerified("PtcControlsCoreWorksheetPageLayoutMechanicalDifferentialTests")]
    public event PropertyChangedEventHandler PropertyChanged;

    [MirrorVerified("PtcControlsCoreWorksheetPageLayoutMechanicalDifferentialTests")]
    private static Size MetricGridCellSize => new Size(WpfUtils.CmToDips(0.5), WpfUtils.CmToDips(0.5));

    [MirrorVerified("PtcControlsCoreWorksheetPageLayoutMechanicalDifferentialTests")]
    private static Size MetricFineGridCellSize => ScaledSize(MetricGridCellSize, 0.5, 0.5);

    [MirrorVerified("PtcControlsCoreWorksheetPageLayoutMechanicalDifferentialTests")]
    private static Size MetricHeaderFooterGridCellSize => ScaledSize(MetricGridCellSize, 0.5, 0.5);

    [MirrorVerified("PtcControlsCoreWorksheetPageLayoutMechanicalDifferentialTests")]
    private static Size USGridCellSize => new Size(WpfUtils.InchesToDips(0.2), WpfUtils.InchesToDips(0.2));

    [MirrorVerified("PtcControlsCoreWorksheetPageLayoutMechanicalDifferentialTests")]
    private static Size USFineGridCellSize => ScaledSize(USGridCellSize, 0.5, 0.5);

    [MirrorVerified("PtcControlsCoreWorksheetPageLayoutMechanicalDifferentialTests")]
    private static Size USHeaderFooterGridCellSize => ScaledSize(USGridCellSize, 0.5, 0.5);

    [MirrorVerified("PtcControlsCoreWorksheetPageLayoutMechanicalDifferentialTests")]
    private static Thickness StandardMetricMargin => new Thickness(WpfUtils.CmToDips(2.5));

    [MirrorVerified("PtcControlsCoreWorksheetPageLayoutMechanicalDifferentialTests")]
    private static Thickness StandardUSMargin => new Thickness(WpfUtils.InchesToDips(1.0));

    [MirrorVerified("PtcControlsCoreWorksheetPageLayoutMechanicalDifferentialTests")]
    private static Thickness LegacyMargin => new Thickness(WpfUtils.InchesToDips(1.2));

    [MirrorVerified("PtcControlsCoreWorksheetPageLayoutMechanicalDifferentialTests")]
    private static Thickness USHeaderMargin
    {
        get
        {
            double horizontal = WpfUtils.InchesToDips(0.25);
            double vertical = WpfUtils.InchesToDips(0.125);
            return new Thickness(horizontal, vertical, horizontal, vertical);
        }
    }

    [MirrorVerified("PtcControlsCoreWorksheetPageLayoutMechanicalDifferentialTests")]
    private static Thickness MetricHeaderMargin
    {
        get
        {
            double horizontal = WpfUtils.CmToDips(0.5);
            double vertical = WpfUtils.CmToDips(0.25);
            return new Thickness(horizontal, vertical, horizontal, vertical);
        }
    }

    [MirrorVerified("PtcControlsCoreWorksheetPageLayoutMechanicalDifferentialTests")]
    private static Point MetricInitialInsertionPoint => new Point(0.0, MetricGridCellSize.Height);

    [MirrorVerified("PtcControlsCoreWorksheetPageLayoutMechanicalDifferentialTests")]
    private static Point USInitialInsertionPoint => new Point(0.0, USGridCellSize.Height);
}
