using Quire.Mirror;

namespace Quire;

public partial struct TableSize
{
    [MirrorVerified("Release IL")]
    public override int GetHashCode() => _coordinate.GetHashCode();
}
