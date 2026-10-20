#nullable disable

namespace Quire.EngNotebookPlus.Automation;

internal partial interface IProxyComClassManagerAgent<T> where T : class
{
    bool IsRegistered { get; }
    bool IsReleased { get; }
    void Release();
}
