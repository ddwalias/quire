#nullable disable
#pragma warning disable

namespace Quire.Services;

partial interface IServiceRegistry
{
    void AddService<T>(global::Quire.Services.IService service, bool shouldConnect = true) where T : global::Quire.Services.IService ;
    void RemoveService<T>(bool shouldDisconnect = true) where T : class, global::Quire.Services.IService ;
    T GetService<T>() where T : global::Quire.Services.IService ;
}
