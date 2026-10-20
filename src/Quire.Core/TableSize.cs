#nullable disable
using Quire.Mirror;

namespace Quire;

public partial struct TableSize
{
    [MirrorVerified("Release IL")]
    private Discrete2DCoordinate _coordinate;

    [MirrorVerified("Release IL")]
    public TableSize(int rows, int columns)
    {
        _coordinate = Discrete2DCoordinate.Origin;
        Rows = rows;
        Columns = columns;
    }

    [MirrorVerified("Release IL")]
    public static bool operator !=(TableSize a, TableSize b) => !(a == b);
}
