#nullable disable
using Quire.Mirror;
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

    [MirrorVerified("Release IL")]
    public DiscretePoint(int x, int y)
    {
        _coordinate = Discrete2DCoordinate.Origin;
        X = x;
        Y = y;
    }

    [MirrorVerified("Release IL")]
    public int X
    {
        get => _coordinate.First;
        set => _coordinate.First = value;
    }

    [MirrorVerified("Release IL")]
    public int Y
    {
        get => _coordinate.Second;
        set => _coordinate.Second = value;
    }

    [MirrorVerified("Release IL")]
    public override string ToString() => _coordinate.ToString();
}
