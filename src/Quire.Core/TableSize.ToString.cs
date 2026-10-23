using Quire.Mirror;

namespace Quire;

public partial struct TableSize
{
    [MirrorVerified("Release IL")]
    public override string ToString() => _coordinate.ToString();
}
