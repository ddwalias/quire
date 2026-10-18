#nullable disable
using System.Windows;
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class TableLayoutManager
{
    [MirrorVerified("PtcControlsCoreTableLayoutManagerMechanicalDifferentialTests")]
    private int _columns;

    [MirrorVerified("PtcControlsCoreTableLayoutManagerMechanicalDifferentialTests")]
    public int Columns { get => _columns; set => _columns = value; }

    [MirrorVerified("PtcControlsCoreTableLayoutManagerMechanicalDifferentialTests")]
    private double _paddingX;

    [MirrorVerified("PtcControlsCoreTableLayoutManagerMechanicalDifferentialTests")]
    public double PaddingX { get => _paddingX; set => _paddingX = value; }

    [MirrorVerified("PtcControlsCoreTableLayoutManagerMechanicalDifferentialTests")]
    private double _paddingY;

    [MirrorVerified("PtcControlsCoreTableLayoutManagerMechanicalDifferentialTests")]
    public double PaddingY { get => _paddingY; set => _paddingY = value; }

    [MirrorVerified("PtcControlsCoreTableLayoutManagerMechanicalDifferentialTests")]
    private Thickness _padding;

    [MirrorVerified("PtcControlsCoreTableLayoutManagerMechanicalDifferentialTests")]
    public Thickness Padding { get => _padding; set => _padding = value; }

    [MirrorVerified("PtcControlsCoreTableLayoutManagerMechanicalDifferentialTests")]
    private double _matrixBaseline;

    [MirrorVerified("PtcControlsCoreTableLayoutManagerMechanicalDifferentialTests")]
    public double MatrixBaseline => _matrixBaseline;

    [MirrorVerified("PtcControlsCoreTableLayoutManagerMechanicalDifferentialTests")]
    private List<Rect> _cellArrangements;

    [MirrorVerified("PtcControlsCoreTableLayoutManagerMechanicalDifferentialTests")]
    public IList<Rect> CellArrangements => _cellArrangements;

    [MirrorVerified("PtcControlsCoreTableLayoutManagerMechanicalDifferentialTests")]
    private double _baselineToMidPointAdjustment;

    [MirrorVerified("PtcControlsCoreTableLayoutManagerMechanicalDifferentialTests")]
    public double BaselineToMidPointAdjustment { get => _baselineToMidPointAdjustment; set => _baselineToMidPointAdjustment = value; }

    [MirrorVerified("PtcControlsCoreTableLayoutManagerMechanicalDifferentialTests")]
    private Size _matrixSize;

    [MirrorVerified("PtcControlsCoreTableLayoutManagerMechanicalDifferentialTests")]
    public Size MatrixSize => _matrixSize;
}
