#nullable disable

namespace Quire.Automation;

internal partial interface IProxyComClassManagerAgent<T> where T : class
{
    bool IsRegistered { get; }
    bool IsReleased { get; }
    void Release();
}
