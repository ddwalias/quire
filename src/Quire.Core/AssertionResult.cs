#nullable disable
using Quire.Mirror;
using Quire.Mirror;

namespace Quire;

public partial class AssertionResult
{
    public static readonly AssertionResult PassedResult = new(passed: true);

    public static readonly AssertionResult FailedResult = new(passed: false);

    public static AssertionResult Get(bool passed)
    {
        if (!passed)
        {
            return FailedResult;
        }
        return PassedResult;
    }

    [MirrorVerified("PtcAssertionResultMechanicalDifferentialTests")]
    private readonly bool _passed;

    [MirrorVerified("PtcAssertionResultMechanicalDifferentialTests")]
    private AssertionResult(bool passed)
    {
        _passed = passed;
    }

    [MirrorVerified("PtcAssertionResultMechanicalDifferentialTests")]
    public bool Passed => _passed;

    [MirrorVerified("Release IL")]
    public static bool operator !=(AssertionResult lhs, AssertionResult rhs) => !(lhs == rhs);

    [MirrorVerified("Release IL")]
    public override bool Equals(object obj) => this == (AssertionResult)obj;

    [MirrorVerified("Release IL")]
    public bool Failed => !_passed;

    [MirrorVerified("Release IL")]
    public override int GetHashCode() => _passed.GetHashCode();
}
