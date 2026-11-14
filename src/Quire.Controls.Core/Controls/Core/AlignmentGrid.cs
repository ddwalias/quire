#nullable disable
using System;
using System.Windows;
using Quire.Mirror;
using Quire.Wpf;

namespace Quire.Controls.Core;

public partial class AlignmentGrid
{
    private static readonly Type TypeOfThis = typeof(AlignmentGrid);
    public static readonly Size DefaultGridCellSize = WorksheetPageLayout.BuiltInPageSizes[WorksheetPageLayout.GetDefaultPageSizeForLocale()].GridCellSize;
    [MirrorVerified("PtcControlsCoreAlignmentGridMechanicalDifferentialTests")]
    public static Rect DefaultGridArea = new Rect(new Size(WpfUtils.InchesToDips(8.5), WpfUtils.InchesToDips(11.0)));
    public static readonly DependencyProperty GridCellSizeProperty = DependencyProperty.Register("GridCellSize", typeof(Size), TypeOfThis, new FrameworkPropertyMetadata(DefaultGridCellSize));
    public static readonly DependencyProperty GridAreaProperty = DependencyProperty.Register("GridArea", typeof(Rect), TypeOfThis, new PropertyMetadata(DefaultGridArea));
    private const double ZeroThreshold = 1E-06;
    public static IAlignmentGrid Default = new AlignmentGrid();

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

    public Pair<Point, Point>[] GetHorizontalLines()
    {
        int count = (int)Math.Round(GridArea.Height / GridCellSize.Height + 1.0);
        Pair<Point, Point>[] lines = new Pair<Point, Point>[count];
        for (int i = 0; i < count; i++)
        {
            double y = GridArea.Top + i * GridCellSize.Height;
            Point first = new Point(GridArea.Left, y);
            Point second = new Point(GridArea.Right, y);
            lines[i] = new Pair<Point, Point>(first, second);
        }
        return lines;
    }

    public Pair<Point, Point>[] GetVerticalLines()
    {
        int count = (int)(GridArea.Width / GridCellSize.Width) + 1;
        Pair<Point, Point>[] lines = new Pair<Point, Point>[count];
        for (int i = 0; i < count; i++)
        {
            double x = GridArea.Left + i * GridCellSize.Width;
            Point first = new Point(x, GridArea.Top);
            Point second = new Point(x, GridArea.Bottom);
            lines[i] = new Pair<Point, Point>(first, second);
        }
        return lines;
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

    public virtual Point GetNearestIntersectionInDirection(Point startPoint, Direction direction)
    {
        Point point = startPoint;
        double offsetX = 0.0;
        double offsetY = 0.0;
        switch (direction)
        {
            case Direction.Up:
                offsetY = IsModZero(startPoint.Y, GridCellSize.Height)
                    ? -GridCellSize.Height
                    : -Math.Abs(startPoint.Y % GridCellSize.Height);
                break;
            case Direction.Down:
                offsetY = IsModZero(startPoint.Y, GridCellSize.Height)
                    ? GridCellSize.Height
                    : GridCellSize.Height - Math.Abs(startPoint.Y % GridCellSize.Height);
                break;
            case Direction.Left:
                offsetX = IsModZero(startPoint.X, GridCellSize.Width)
                    ? -GridCellSize.Width
                    : -Math.Abs(startPoint.X % GridCellSize.Width);
                break;
            case Direction.Right:
                offsetX = IsModZero(startPoint.X, GridCellSize.Width)
                    ? GridCellSize.Width
                    : GridCellSize.Width - Math.Abs(startPoint.X % GridCellSize.Width);
                break;
        }
        point.Offset(offsetX, offsetY);
        return FixRoundingError(point);
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

    public AlignmentGrid(Size? gridSize)
        : this()
    {
        if (gridSize.HasValue)
        {
            GridCellSize = gridSize.Value;
            return;
        }
        double num = WpfUtils.InchesToDips(0.2);
        GridCellSize = new Size(num, num);
    }
}
