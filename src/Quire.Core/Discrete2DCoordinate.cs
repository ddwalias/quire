#nullable disable
using Quire.Mirror;

namespace Quire;

public partial struct Discrete2DCoordinate
{
    [MirrorVerified("Release IL")]
    public static Discrete2DCoordinate Origin;

    [MirrorVerified("Release IL")]
    public int First;

    [MirrorVerified("Release IL")]
    public int Second;

    [MirrorVerified("Release IL")]
    public override string ToString() => string.Format("{0},{1}", First, Second);
}
