using Quire.Mirror;

namespace Quire;

public partial struct TableCoordinate
{
    [MirrorVerified("Release IL")]
    public static bool operator ==(TableCoordinate a, TableCoordinate b)
    {
        if (a.Column == b.Column)
        {
            return a.Row == b.Row;
        }
        return false;
    }
}
