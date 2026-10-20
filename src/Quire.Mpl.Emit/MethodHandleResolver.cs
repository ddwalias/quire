using System.Reflection;
using System.Reflection.Metadata;

namespace System.Reflection.Emit.Quire.Mpl;

internal partial class MethodHandleResolver
{
    public static void ResolveHandleEmit(TypeBuilder typeBuilder, MethodBase methodBase, Action<Handle> action, Action<MethodBase> actionError)
    {
        Handle? handle = ResolveMethod(typeBuilder, methodBase);
        if (handle.HasValue)
        {
            action(handle.Value);
        }
        else
        {
            actionError(methodBase);
        }
    }
}
