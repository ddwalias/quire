#nullable disable
#pragma warning disable

namespace Quire.Services;

partial interface IFunctionalityService
{
    bool IsFunctionalityEnabled(global::Quire.FunctionalitiesLimitation.EnumFunctionalityKey functionality) ;
    bool IsFunctionalityHasUnsupportedLicenseTypes(global::Quire.FunctionalitiesLimitation.EnumFunctionalityKey functionality) ;
}
