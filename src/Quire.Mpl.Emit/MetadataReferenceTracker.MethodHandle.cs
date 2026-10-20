using System.Reflection;
using System.Reflection.Metadata;

namespace System.Reflection.Emit.Quire.Mpl;

public partial class MetadataReferenceTracker
{
    public Handle GetOrAddMethodHandle(MethodInfo method)
    {
        if (method.IsGenericMethod && !method.IsGenericMethodDefinition)
        {
            return GetOrAddMethodSpecification(method);
        }
        return GetOrAddMethodReference(method);
    }
}
