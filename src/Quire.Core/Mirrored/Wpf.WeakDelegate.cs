#nullable disable
#pragma warning disable

namespace Quire.Wpf;

partial class WeakDelegate
{
    public bool IsWeakReferenceAlive { get => _weakReference == null || _weakReference.Target != null; }
}
