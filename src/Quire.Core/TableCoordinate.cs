#nullable disable
using Quire.Mirror;

namespace Quire;

public partial struct TableCoordinate
{
    [MirrorVerified("Release IL")]
    private Discrete2DCoordinate _coordinate;

    [MirrorVerified("Release IL")]
    public TableCoordinate(int row, int col)
    {
        _coordinate = Discrete2DCoordinate.Origin;
        Row = row;
        Column = col;
    }

    [MirrorVerified("Release IL")]
    public static bool operator !=(TableCoordinate a, TableCoordinate b) => !(a == b);
}
