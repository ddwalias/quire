#nullable disable

namespace Quire.EngNotebook.Automation;

internal partial interface IProxyComClassManagerAgent<T> where T : class
{
    bool IsRegistered { get; }
    bool IsReleased { get; }
    void Release();
}
