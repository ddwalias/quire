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

    [MirrorVerified("Release IL")]
    public int Top
    {
        get => _topLeft.Y;
        set => _topLeft.Y = value;
    }

    [MirrorVerified("Release IL")]
    public int Left
    {
        get => _topLeft.X;
        set => _topLeft.X = value;
    }

    [MirrorVerified("Release IL")]
    public int Bottom
    {
        get => _bottomRight.Y;
        set => _bottomRight.Y = value;
    }

    [MirrorVerified("Release IL")]
    public int Right
    {
        get => _bottomRight.X;
        set => _bottomRight.X = value;
    }

    [MirrorVerified("Release IL")]
    public void Normalize()
    {
        int top;
        if (Top > Bottom)
        {
            top = Top;
            Top = Bottom;
            Bottom = top;
        }
        if (Left > Right)
        {
            top = Left;
            Left = Right;
            Right = top;
        }
    }
}
