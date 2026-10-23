using Quire.Mirror;

namespace Quire;

public partial struct TableCoordinate
{
    [MirrorVerified("Release IL")]
    public override string ToString() => _coordinate.ToString();
}
