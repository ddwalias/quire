#nullable disable
using System.Windows;
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class VisualTableContents
{
    [MirrorVerified("PtcControlsCoreVisualTableContentsMechanicalDifferentialTests")]
    protected readonly TableLayoutManager _layoutManager;

    [MirrorVerified("PtcControlsCoreVisualTableContentsMechanicalDifferentialTests")]
    public double GetRowHeight(int row) => _layoutManager.GetRowHeight(row);

    [MirrorVerified("PtcControlsCoreVisualTableContentsMechanicalDifferentialTests")]
    public double GetColumnWidth(int column) => _layoutManager.GetColumnWidth(column);

    [MirrorVerified("PtcControlsCoreVisualTableContentsMechanicalDifferentialTests")]
    public int Columns { get => _layoutManager.Columns; set => _layoutManager.Columns = value; }

    [MirrorVerified("PtcControlsCoreVisualTableContentsMechanicalDifferentialTests")]
    public ColumnAlignments ColumnAlignment { get; set; }

    [MirrorVerified("PtcControlsCoreVisualTableContentsMechanicalDifferentialTests")]
    public double ColumnSpacing { get => _layoutManager.PaddingX; set => _layoutManager.PaddingX = value; }

    [MirrorVerified("PtcControlsCoreVisualTableContentsMechanicalDifferentialTests")]
    public double RowSpacing { get => _layoutManager.PaddingY; set => _layoutManager.PaddingY = value; }

    [MirrorVerified("PtcControlsCoreVisualTableContentsMechanicalDifferentialTests")]
    public Thickness Padding { get => _layoutManager.Padding; set => _layoutManager.Padding = value; }

    [MirrorVerified("PtcControlsCoreVisualTableContentsMechanicalDifferentialTests")]
    public double BaselineToMidPointAdjustment { get => _layoutManager.BaselineToMidPointAdjustment; set => _layoutManager.BaselineToMidPointAdjustment = value; }

    [MirrorVerified("PtcControlsCoreVisualTableContentsMechanicalDifferentialTests")]
    public double MatrixBaseline => _layoutManager.MatrixBaseline;
}
