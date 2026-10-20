#nullable disable
using Quire.FunctionalitiesLimitation;
using Quire.Mirror;

namespace Quire.Services;

public partial class FunctionalityService
{
    [MirrorVerified("Release IL")]
    private readonly FunctionalitiesLimitationDictionary _functionalitiesLimitationDictionary;

    [MirrorVerified("Release IL")]
    private FunctionalityService()
    {
        _functionalitiesLimitationDictionary = new FunctionalitiesLimitationDictionary();
    }
}
