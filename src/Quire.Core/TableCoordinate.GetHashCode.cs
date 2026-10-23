using Quire.Mirror;

namespace Quire;

public partial struct TableCoordinate
{
    [MirrorVerified("Release IL")]
    public override int GetHashCode() => _coordinate.GetHashCode();
}
