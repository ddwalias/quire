#nullable disable

namespace Quire.EngNotebook.Automation;

internal partial interface IProxyComRealSubject<out T>
{
    T RealSubjectInstance { get; }
}
