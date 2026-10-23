using Quire.Mirror;

namespace Quire;

public partial struct TableSize
{
    [MirrorVerified("Release IL")]
    public bool Equals(TableSize obj) => obj._coordinate.Equals(_coordinate);
}
