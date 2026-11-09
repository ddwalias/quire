#nullable disable
#pragma warning disable

namespace Quire.Services;

partial class FunctionalityServiceWrapper
{
    private static global::Quire.Services.FunctionalityServiceWrapper _instance;

    public static global::Quire.Services.FunctionalityServiceWrapper Instance { get => _instance ?? (_instance = new global::Quire.Services.FunctionalityServiceWrapper()); }
}
