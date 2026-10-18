#nullable disable
using Quire.Mirror;

namespace Quire;

public partial struct DiscreteRect
{
    [MirrorVerified("PtcDiscreteRectMechanicalDifferentialTests")]
    private DiscretePoint _topLeft;

    [MirrorVerified("PtcDiscreteRectMechanicalDifferentialTests")]
    public DiscretePoint TopLeft { get => _topLeft; set => _topLeft = value; }

    [MirrorVerified("PtcDiscreteRectMechanicalDifferentialTests")]
    private DiscretePoint _bottomRight;

    [MirrorVerified("PtcDiscreteRectMechanicalDifferentialTests")]
    public DiscretePoint BottomRight { get => _bottomRight; set => _bottomRight = value; }
}
