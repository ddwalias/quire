#nullable disable

namespace Quire.EngNotebookPlus.Automation;

internal partial interface IProxyComRealSubject<out T>
{
    T RealSubjectInstance { get; }
}
