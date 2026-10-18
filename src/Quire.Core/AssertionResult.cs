#nullable disable
using Quire.Mirror;

namespace Quire;

public partial class AssertionResult
{
    [MirrorVerified("PtcAssertionResultMechanicalDifferentialTests")]
    private readonly bool _passed;

    [MirrorVerified("PtcAssertionResultMechanicalDifferentialTests")]
    private AssertionResult(bool passed)
    {
        _passed = passed;
    }

    [MirrorVerified("PtcAssertionResultMechanicalDifferentialTests")]
    public bool Passed => _passed;
}
