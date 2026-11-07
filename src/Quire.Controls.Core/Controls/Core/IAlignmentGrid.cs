#nullable disable
using System.Windows;

namespace Quire.Controls.Core;

public partial interface IAlignmentGrid
{
    Rect GridArea { get; set; }
    Size GridCellSize { get; set; }
    Point GetNextIntersection(Point startPoint, Direction direction);
    Point GetNearestIntersectionInDirection(Point startPoint, Direction direction);
    Point GetNearestIntersectionPoint(Point startPoint);
    Point GetNearestIntersectionPointDisregardingBounds(Point startPoint, GridRestriction restriction, params Direction[] directionsToIgnore);
    Point ConvertDipsToGridCoordinates(Point dipCoordinates);
    Point ConvertGridCoordinatesToDips(Point value);
}
