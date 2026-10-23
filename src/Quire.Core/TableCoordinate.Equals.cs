using Quire.Mirror;

namespace Quire;

public partial struct TableCoordinate
{
    [MirrorVerified("Release IL")]
    public bool Equals(TableCoordinate obj) => obj._coordinate.Equals(_coordinate);
}
