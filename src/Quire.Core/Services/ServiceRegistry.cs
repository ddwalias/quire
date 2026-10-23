#nullable disable
using Quire.Mirror;
using Quire.Mirror;

namespace Quire.Services;

public partial class ServiceRegistry
{
    public void AddService<T>(IService service, bool shouldConnect = true) where T : IService
    {
        if (shouldConnect && DebugTools.DevAssert(!service.IsConnected, null).Passed)
        {
            service.IsConnected = true;
        }
        AddService(typeof(T), service);
    }

    public void RemoveService<T>(bool shouldDisconnect = true) where T : class, IService
    {
        T service = GetService<T>();
        RemoveService(typeof(T));
        if (((service != null) & shouldDisconnect) && DebugTools.DevAssert(service.IsConnected, null).Passed)
        {
            service.IsConnected = false;
        }
    }

    [MirrorVerified("PtcServicesServiceRegistryMechanicalDifferentialTests")]
    public ServiceRegistry() { }

    [MirrorVerified("Release IL")]
    public T GetService<T>() where T : IService => (T)GetService(typeof(T));
}
