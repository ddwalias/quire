#nullable disable
using System.Windows;
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class RestrictedAreaAlignmentGrid
{
    [MirrorVerified("PtcControlsCoreRestrictedAreaAlignmentGridMechanicalDifferentialTests")]
    public Point GetNextIntersectionIgnoringRestrictedAreas(Point startPoint, Direction direction) => base.GetNextIntersection(startPoint, direction);

    [MirrorVerified("PtcControlsCoreRestrictedAreaAlignmentGridMechanicalDifferentialTests")]
    public Point GetNearestIntersectionInDirectionIgnoringRestrictedAreas(Point startPoint, Direction direction) => base.GetNearestIntersectionInDirection(startPoint, direction);

    [MirrorVerified("PtcControlsCoreRestrictedAreaAlignmentGridMechanicalDifferentialTests")]
    private readonly Dictionary<object, Pair<double, double>> _restrictedAreas;

    [MirrorVerified("PtcControlsCoreRestrictedAreaAlignmentGridMechanicalDifferentialTests")]
    public Dictionary<object, Pair<double, double>> RestrictedAreas => _restrictedAreas;

    [MirrorVerified("PtcControlsCoreRestrictedAreaAlignmentGridMechanicalDifferentialTests")]
    private readonly Dictionary<object, Pair<double, double>> _protectedAreas;

    [MirrorVerified("PtcControlsCoreRestrictedAreaAlignmentGridMechanicalDifferentialTests")]
    public Dictionary<object, Pair<double, double>> ProtectedAreas => _protectedAreas;
}
