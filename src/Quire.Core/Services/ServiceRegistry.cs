#nullable disable
using Quire.Mirror;
using Quire.Mirror;

namespace Quire.Services;

public partial class ServiceRegistry
{
    [MirrorVerified("PtcServicesServiceRegistryMechanicalDifferentialTests")]
    public ServiceRegistry() { }

    [MirrorVerified("Release IL")]
    public T GetService<T>() where T : IService => (T)GetService(typeof(T));
}
