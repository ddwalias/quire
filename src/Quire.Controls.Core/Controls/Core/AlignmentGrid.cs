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

    public Point ConvertDipsToGridCoordinates(Point dipCoordinates)
    {
        return new Point(Math.Round(dipCoordinates.X / GridCellSize.Width), Math.Round(dipCoordinates.Y / GridCellSize.Height));
    }

    public Point ConvertGridCoordinatesToDips(Point value)
    {
        return new Point(value.X * GridCellSize.Width, value.Y * GridCellSize.Height);
    }

    protected Point FixRoundingError(Point pt)
    {
        if (pt.X.IsEqualTo(0.0))
        {
            pt.X = 0.0;
        }
        if (pt.Y.IsEqualTo(0.0))
        {
            pt.Y = 0.0;
        }
        if (pt.X.IsEqualTo(HighestValidX))
        {
            pt.X = HighestValidX;
        }
        if (pt.Y.IsEqualTo(HighestValidY))
        {
            pt.Y = HighestValidY;
        }
        return pt;
    }

    private static bool IsModZero(double dividend, double divisor)
    {
        double remainder = dividend % divisor;
        if (!(remainder <= ZeroThreshold))
        {
            return Math.Abs(divisor) - Math.Abs(remainder) <= ZeroThreshold;
        }
        return true;
    }

    public Point GetNearestIntersectionPoint(Point startPoint)
    {
        return GetNearestIntersectionPointDisregardingBounds(startPoint, GridRestriction.Skip);
    }

    private Point GetCandidateIntersectionPoint(Point startPoint, double cellWidth, double cellHeight)
    {
        double remainderX = startPoint.X % cellWidth;
        double remainderY = startPoint.Y % cellHeight;
        Point result = startPoint;
        double offsetX;
        double offsetY;
        if (startPoint.Y < 0.0)
        {
            offsetY = remainderY >= cellHeight / 2.0 ? -remainderY : -cellHeight - remainderY;
        }
        else
        {
            offsetY = remainderY <= cellHeight / 2.0 ? -remainderY : cellHeight - remainderY;
        }
        if (startPoint.X < 0.0)
        {
            offsetX = remainderX >= cellWidth / 2.0 ? -remainderX : -cellWidth - remainderX;
        }
        else
        {
            offsetX = remainderX <= cellWidth / 2.0 ? -remainderX : cellWidth - remainderX;
        }
        result.Offset(offsetX, offsetY);
        return result;
    }
}
