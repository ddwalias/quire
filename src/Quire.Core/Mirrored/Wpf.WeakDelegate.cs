#nullable disable
#pragma warning disable
using Quire.Mirror;

namespace Quire.Wpf;

partial class WeakDelegate
{
    [MirrorVerified("Release IL")]
    public WeakDelegate(System.Delegate realDelegate)
    {
        if (realDelegate == null || realDelegate.Method == null)
        {
            throw new System.ArgumentNullException("realDelegate");
        }
        _weakReference = realDelegate.Target == null ? null : new System.WeakReference(realDelegate.Target);
        _method = realDelegate.Method;
    }

    public bool IsWeakReferenceAlive { get => _weakReference == null || _weakReference.Target != null; }

    [MirrorVerified("Release IL")]
    public void Invoke(object[] arguments)
    {
        if (_weakReference == null)
        {
            _method.Invoke(null, arguments);
            return;
        }
        object target = _weakReference.Target;
        if (target != null)
        {
            _method.Invoke(target, arguments);
        }
    }

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
