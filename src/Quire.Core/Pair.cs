#nullable disable
using Quire.Mirror;
using Quire.Mirror;

namespace Quire;

public partial struct Pair<TFirst, TSecond>
{
    [MirrorVerified("PtcPair2MechanicalDifferentialTests")]
    public TFirst First;

    [MirrorVerified("PtcPair2MechanicalDifferentialTests")]
    public TSecond Second;

    [MirrorVerified("PtcPair2MechanicalDifferentialTests")]
    public Pair(TFirst first, TSecond second)
    {
        First = first;
        Second = second;
    }

    [MirrorVerified("Release IL")]
    public override string ToString() => string.Format("First: {0} Second: {1}", First, Second);

    [MirrorVerified("Release IL")]
    public static bool operator !=(Pair<TFirst, TSecond> lhs, Pair<TFirst, TSecond> rhs) => !(lhs == rhs);
}
