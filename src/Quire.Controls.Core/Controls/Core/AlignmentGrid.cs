#nullable disable
using System.Windows;
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class AlignmentGrid
{
    [MirrorVerified("PtcControlsCoreAlignmentGridMechanicalDifferentialTests")]
    public static Rect DefaultGridArea;

    [MirrorVerified("PtcControlsCoreAlignmentGridMechanicalDifferentialTests")]
    public AlignmentGrid() : this(DefaultGridArea) { }

    [MirrorVerified("PtcControlsCoreAlignmentGridMechanicalDifferentialTests")]
    public virtual void Dispose() { }

    public Rect GridArea
    {
        get => (Rect)GetValue(GridAreaProperty);
        set => SetValue(GridAreaProperty, value);
    }

    public Size GridCellSize
    {
        get => (Size)GetValue(GridCellSizeProperty);
        set => SetValue(GridCellSizeProperty, value);
    }

    protected double HighestValidY => Math.Floor(GridArea.Height / GridCellSize.Height) * GridCellSize.Height;

    protected double HighestValidX => Math.Floor(GridArea.Width / GridCellSize.Width) * GridCellSize.Width;

    public AlignmentGrid(Rect rect)
    {
        SetValue(GridAreaProperty, rect);
    }
}
