#nullable disable
#pragma warning disable
using Quire.Mirror;

namespace Quire.Wpf;

partial class WeakDelegate
{
    public bool IsWeakReferenceAlive { get => _weakReference == null || _weakReference.Target != null; }

    [MirrorVerified("Release IL")]
    public bool Equal(System.Delegate realDelegate)
    {
        if (realDelegate == null)
        {
            return false;
        }
        if (_weakReference != null)
        {
            if (_weakReference.Target == realDelegate.Target)
            {
                return _method == realDelegate.Method;
            }
            return false;
        }
        if (realDelegate.Target == null)
        {
            return _method == realDelegate.Method;
        }
        return false;
    }
}
