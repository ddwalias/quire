#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Quire;
using Quire.Core;
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class RestrictedAreaAlignmentGrid : AlignmentGrid, IRestrictedAreaAlignmentGrid, IAlignmentGrid, IDisposable
{
    private readonly Dictionary<object, Pair<double, double>> _restrictedAreas = new Dictionary<object, Pair<double, double>>();

    private readonly Dictionary<object, Pair<double, double>> _protectedAreas = new Dictionary<object, Pair<double, double>>();

    [MirrorVerified("PtcControlsCoreRestrictedAreaAlignmentGridMechanicalDifferentialTests")]
    public Dictionary<object, Pair<double, double>> RestrictedAreas => _restrictedAreas;

    [MirrorVerified("PtcControlsCoreRestrictedAreaAlignmentGridMechanicalDifferentialTests")]
    public Dictionary<object, Pair<double, double>> ProtectedAreas => _protectedAreas;

    private IEnumerable<Pair<double, double>> AreasToSkip => RestrictedAreas.Values.Union(ProtectedAreas.Values);

    public RestrictedAreaAlignmentGrid()
    {
    }

    public RestrictedAreaAlignmentGrid(Size? gridSize)
        : base(gridSize)
    {
    }

    public override Point GetNearestIntersectionInDirection(Point startPoint, Direction direction)
    {
        Point candidate = base.GetNearestIntersectionInDirection(startPoint, direction);
        if (DirectionOps.IsVertical(direction))
        {
            while (AreasToSkip.Any((Pair<double, double> pair) => candidate.Y.IsBetween(pair.First, pair.Second)))
            {
                candidate = base.GetNearestIntersectionInDirection(candidate, direction);
            }
        }
        return candidate;
    }

    public override Point GetNextIntersection(Point startPoint, Direction direction)
    {
        Point candidate = base.GetNextIntersection(startPoint, direction);
        if (DirectionOps.IsVertical(direction))
        {
            while (AreasToSkip.Any((Pair<double, double> pair) => candidate.Y.IsBetween(pair.First, pair.Second)))
            {
                candidate = base.GetNextIntersection(candidate, direction);
            }
        }
        return candidate;
    }

    public override Point GetNearestIntersectionPointDisregardingBounds(Point startPoint, GridRestriction restriction, params Direction[] directionsToIgnore)
    {
        Point candidate = base.GetNearestIntersectionPointDisregardingBounds(startPoint, restriction, directionsToIgnore);
        switch (restriction)
        {
            case GridRestriction.Ignore:
                return candidate;
            case GridRestriction.Skip:
            {
                if (!AreasToSkip.Any((Pair<double, double> bounds) => candidate.Y.IsBetween(bounds.First, bounds.Second)))
                {
                    return candidate;
                }
                Point nearestIntersectionInDirection = GetNearestIntersectionInDirection(candidate, Direction.Up);
                Point nearestIntersectionInDirection2 = GetNearestIntersectionInDirection(candidate, Direction.Down);
                Func<Point, bool> func = (Point pt) => (pt.Y.IsGreaterThanOrEqual(0.0) || directionsToIgnore.Contains(Direction.Up, null)) && (pt.Y.IsLessThanOrEqual(HighestValidY) || directionsToIgnore.Contains(Direction.Down, null));
                bool flag = func(nearestIntersectionInDirection2);
                bool flag2 = func(nearestIntersectionInDirection);
                Point result;
                if (!flag)
                {
                    result = nearestIntersectionInDirection;
                }
                else if (!flag2)
                {
                    result = nearestIntersectionInDirection2;
                }
                else
                {
                    result = (Math.Abs(candidate.Y - nearestIntersectionInDirection.Y) < Math.Abs(nearestIntersectionInDirection2.Y - candidate.Y)) ? nearestIntersectionInDirection : nearestIntersectionInDirection2;
                }
                return result;
            }
            default:
                DebugTools.FailAssertion("Unsupported operation: {0}", restriction);
                break;
            case GridRestriction.Default:
            case GridRestriction.Assert:
                break;
        }
        DebugTools.DevAssert(!AreasToSkip.Any((Pair<double, double> bounds) => candidate.Y.IsBetween(bounds.First, bounds.Second)), null);
        return candidate;
    }

    [MirrorVerified("PtcControlsCoreRestrictedAreaAlignmentGridMechanicalDifferentialTests")]
    public Point GetNextIntersectionIgnoringRestrictedAreas(Point startPoint, Direction direction) => base.GetNextIntersection(startPoint, direction);

    [MirrorVerified("PtcControlsCoreRestrictedAreaAlignmentGridMechanicalDifferentialTests")]
    public Point GetNearestIntersectionInDirectionIgnoringRestrictedAreas(Point startPoint, Direction direction) => base.GetNearestIntersectionInDirection(startPoint, direction);

    public Point GetNearestInsertionPointIgnoringRestrictedAreas(Point startPoint)
    {
        return GetNearestIntersectionPointDisregardingBounds(startPoint, GridRestriction.Ignore);
    }

    public bool IsWithinRestrictedArea(Point point)
    {
        Point candidate = GetNearestInsertionPointIgnoringRestrictedAreas(point);
        return AreasToSkip.Any((Pair<double, double> bounds) => candidate.Y.IsBetween(bounds.First, bounds.Second));
    }

    public override void Dispose()
    {
        _restrictedAreas.Clear();
        base.Dispose();
    }
}
