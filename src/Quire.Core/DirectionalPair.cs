#nullable disable
using Quire.Mirror;

namespace Quire;

public partial struct DirectionalPair<TLeft, TRight>
{
    [MirrorVerified("Release IL")]
    public DirectionalPair(TLeft left, TRight right)
    {
        _pair = new Pair<TLeft, TRight>(left, right);
        Left = _pair.First;
        Right = _pair.Second;
    }

    [MirrorVerified("Release IL")]
    public TLeft Left;

    [MirrorVerified("Release IL")]
    public TRight Right;

    [MirrorVerified("Release IL")]
    public override string ToString() => string.Format("Left: {0} Right: {1}", Left, Right);
}
