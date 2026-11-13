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

    public virtual Point GetNextIntersection(Point startPoint, Direction direction)
    {
        Point pt = startPoint;
        double num = 0.0;
        double num2 = 0.0;
        Size gridCellSize;
        switch (direction)
        {
            case Direction.Up:
                gridCellSize = GridCellSize;
                num2 = -gridCellSize.Height;
                break;
            case Direction.Down:
                gridCellSize = GridCellSize;
                num2 = gridCellSize.Height;
                break;
            case Direction.Left:
                gridCellSize = GridCellSize;
                num = -gridCellSize.Width;
                break;
            case Direction.Right:
                gridCellSize = GridCellSize;
                num = gridCellSize.Width;
                break;
        }
        pt.Offset(num, num2);
        return FixRoundingError(pt);
    }

    public virtual Point GetNearestIntersectionPointDisregardingBounds(Point startPoint, GridRestriction restriction, params Direction[] directionsToIgnore)
    {
        return Clip(GetCandidateIntersectionPoint(startPoint, GridCellSize.Width, GridCellSize.Height), directionsToIgnore);
    }

    private Point Clip(Point point, params Direction[] directionsToIgnore)
    {
        Point result;
        Rect gridArea = GridArea;
        if (new Rect(gridArea.Size).ContainsAccountingForMeasurementError(point))
        {
            return point;
        }
        result = point;
        if (!directionsToIgnore.Contains(Direction.Right, null))
        {
            result.X = Math.Min(result.X, HighestValidX);
        }
        if (!directionsToIgnore.Contains(Direction.Down, null))
        {
            result.Y = Math.Min(result.Y, HighestValidY);
        }
        if (!directionsToIgnore.Contains(Direction.Left, null))
        {
            result.X = Math.Max(result.X, 0.0);
        }
        if (!directionsToIgnore.Contains(Direction.Up, null))
        {
            result.Y = Math.Max(result.Y, 0.0);
        }
        return result;
    }
}
