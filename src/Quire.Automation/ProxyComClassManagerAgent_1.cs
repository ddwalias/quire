#nullable disable

namespace Quire.Automation;

internal partial class ProxyComClassManagerAgent<T> where T : class
{
    private readonly ProxyComClassManager<T> _comProxyManagerInstance;

    public bool IsRegistered { get; private set; }

    public bool IsReleased { get; private set; }

    internal ProxyComClassManagerAgent(ProxyComClassManager<T> comProxyManagerInstance)
    {
        _comProxyManagerInstance = comProxyManagerInstance;
        IsRegistered = _comProxyManagerInstance.Register(this);
        IsReleased = false;
    }

    public void Release()
    {
        IsReleased = _comProxyManagerInstance.Unregister(this);
        IsRegistered = !IsReleased;
    }
}
