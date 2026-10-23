#nullable disable
using Quire.Mirror;

namespace Quire.Wpf;

public partial class WeakMultiCastDelegate<TDelegate> where TDelegate : class
{
    [MirrorVerified("Release IL")]
    public void Combine(TDelegate tDelegate)
    {
        if (tDelegate == null)
        {
            return;
        }
        System.Delegate[] invocationList = (tDelegate as System.Delegate).GetInvocationList();
        for (int index = 0; index < invocationList.Length; index++)
        {
            System.Delegate item = invocationList[index];
            _weakDelegates.Add(new WeakDelegate(item));
        }
    }

    [MirrorVerified("Release IL")]
    public void Invoke(object[] arguments)
    {
        System.Collections.Generic.List<WeakDelegate> deadDelegates = new System.Collections.Generic.List<WeakDelegate>();
        foreach (WeakDelegate weakDelegate in _weakDelegates)
        {
            if (weakDelegate != null)
            {
                if (weakDelegate.IsWeakReferenceAlive)
                {
                    weakDelegate.Invoke(arguments);
                }
                else
                {
                    deadDelegates.Add(weakDelegate);
                }
            }
        }
        foreach (WeakDelegate weakDelegate in deadDelegates)
        {
            _weakDelegates.Remove(weakDelegate);
        }
    }
}
