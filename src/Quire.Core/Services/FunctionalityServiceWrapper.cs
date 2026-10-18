#nullable disable
using Quire.FunctionalitiesLimitation;
using Quire.Mirror;

namespace Quire.Services;

public partial class FunctionalityServiceWrapper
{
    [MirrorVerified("PtcServicesFunctionalityServiceWrapperMechanicalDifferentialTests")]
    private readonly FunctionalityService _funcService;

    [MirrorVerified("PtcServicesFunctionalityServiceWrapperMechanicalDifferentialTests")]
    public bool IsFunctionalityEnabled(EnumFunctionalityKey functionality) => _funcService.IsFunctionalityEnabled(functionality);
}
