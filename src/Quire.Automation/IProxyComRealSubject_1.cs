#nullable disable

namespace Quire.Automation;

internal partial interface IProxyComRealSubject<out T>
{
    T RealSubjectInstance { get; }
}
