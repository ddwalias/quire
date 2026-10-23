#nullable disable
using Quire.Mirror;

namespace Quire.Wpf;

public partial class WeakMultiCastDelegate<TDelegate> where TDelegate : class
{
    private readonly System.Collections.Generic.List<WeakDelegate> _weakDelegates = new System.Collections.Generic.List<WeakDelegate>();

    public WeakMultiCastDelegate() { }

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

    public void Remove(TDelegate tDelegate)
    {
        if (tDelegate == null)
        {
            return;
        }
        System.Delegate[] invocationList = (tDelegate as System.Delegate).GetInvocationList();
        foreach (System.Delegate obj in invocationList)
        {
            System.Delegate thisDelegate = obj;
            WeakDelegate weakDelegate = _weakDelegates.Find((WeakDelegate item) => item != null && item.Equal(thisDelegate));
            if (weakDelegate != null)
            {
                _weakDelegates.Remove(weakDelegate);
            }
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
