#nullable disable
using System.Collections.Generic;
using System.Windows;

namespace Quire.Controls.Core;

public partial interface IRestrictedAreaAlignmentGrid
{
    Dictionary<object, Pair<double, double>> RestrictedAreas { get; }
    Dictionary<object, Pair<double, double>> ProtectedAreas { get; }
    bool IsWithinRestrictedArea(Point point);
    Point GetNextIntersectionIgnoringRestrictedAreas(Point startPoint, Direction direction);
    Point GetNearestIntersectionInDirectionIgnoringRestrictedAreas(Point startPoint, Direction direction);
    Point GetNearestInsertionPointIgnoringRestrictedAreas(Point startPoint);
}
