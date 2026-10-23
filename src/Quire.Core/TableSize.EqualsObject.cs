using Quire.Mirror;

namespace Quire;

public partial struct TableSize
{
    [MirrorVerified("Release IL")]
    public override bool Equals(object obj)
    {
        if (obj.GetType() != typeof(TableSize))
        {
            return false;
        }
        return Equals((TableSize)obj);
    }
}
