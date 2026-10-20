using Quire.Automation;
using Quire.Mirror;

namespace Quire.Calculation;

internal partial class ValueProxyWrapper
{
    [MirrorVerified("PtcCalculationValueProxyWrapperMechanicalDifferentialTests")]
    public IValueProxy Proxy { get; private set; }

    [MirrorVerified("PtcCalculationValueProxyWrapperMechanicalDifferentialTests")]
    public ValueProxyWrapper(IValueProxy proxy)
    {
        Proxy = proxy;
    }
}
