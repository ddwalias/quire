#nullable disable
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
}
