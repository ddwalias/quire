#nullable disable
using Quire.Mirror;

namespace Quire.Services;

public static partial class AppServiceRegistry
{
    [MirrorVerified("PtcServicesAppServiceRegistryMechanicalDifferentialTests")]
    private static readonly ServiceRegistry _serviceProvider;

    [MirrorVerified("PtcServicesAppServiceRegistryMechanicalDifferentialTests")]
    public static void AddService<T>(IService service, bool shouldConnect = true) where T : class, IService => _serviceProvider.AddService<T>(service, shouldConnect);

    [MirrorVerified("PtcServicesAppServiceRegistryMechanicalDifferentialTests")]
    public static void RemoveService<T>(bool shouldDisconnect = true) where T : class, IService => _serviceProvider.RemoveService<T>(shouldDisconnect);

    [MirrorVerified("PtcServicesAppServiceRegistryMechanicalDifferentialTests")]
    public static T GetService<T>() where T : class, IService => _serviceProvider.GetService<T>();
}
