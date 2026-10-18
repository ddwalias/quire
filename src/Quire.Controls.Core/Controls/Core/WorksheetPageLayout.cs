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
}
