using Quire.Mirror;

namespace Quire;

public partial struct TableSize
{
    [MirrorVerified("Release IL")]
    public static bool operator ==(TableSize a, TableSize b)
    {
        if (a.Columns == b.Columns)
        {
            return a.Rows == b.Rows;
        }
        return false;
    }
}
