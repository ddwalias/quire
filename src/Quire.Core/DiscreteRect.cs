#nullable disable
using Quire.Mirror;
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

    [MirrorVerified("Release IL")]
    public DiscreteRect(int top, int left, int bottom, int right)
    {
        _topLeft = new DiscretePoint(Discrete2DCoordinate.Origin);
        _bottomRight = new DiscretePoint(Discrete2DCoordinate.Origin);
        Top = top;
        Left = left;
        Bottom = bottom;
        Right = right;
        Normalize();
    }

    [MirrorVerified("Release IL")]
    public override string ToString() => string.Format("(({0}),({1}))", _topLeft, _bottomRight);

    [MirrorVerified("Release IL")]
    public int Height => Math.Abs(Bottom - Top);

    [MirrorVerified("Release IL")]
    public int Width => Math.Abs(Right - Left);
}
