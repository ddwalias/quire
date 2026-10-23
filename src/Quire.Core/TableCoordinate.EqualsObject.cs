using Quire.Mirror;

namespace Quire;

public partial struct TableCoordinate
{
    [MirrorVerified("Release IL")]
    public override bool Equals(object obj)
    {
        if (obj.GetType() != typeof(TableCoordinate))
        {
            return false;
        }
        return Equals((TableCoordinate)obj);
    }
}
