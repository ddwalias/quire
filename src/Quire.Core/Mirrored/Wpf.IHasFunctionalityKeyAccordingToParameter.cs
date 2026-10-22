#nullable disable
#pragma warning disable

namespace Quire.Wpf;

partial interface IHasFunctionalityKeyAccordingToParameter
{
    global::Quire.FunctionalitiesLimitation.EnumFunctionalityKey GetFunctionalityKeyAccordingParameter(object parameter, bool failAssertionIfNotFound = true) ;
}
