#nullable disable
using Quire.Mirror;

namespace Quire;

public partial struct DiscretePoint
{
    [MirrorVerified("PtcDiscretePointMechanicalDifferentialTests")]
    private Discrete2DCoordinate _coordinate;

    [MirrorVerified("PtcDiscretePointMechanicalDifferentialTests")]
    public DiscretePoint(Discrete2DCoordinate coordinate)
    {
        _coordinate = coordinate;
    }
}
