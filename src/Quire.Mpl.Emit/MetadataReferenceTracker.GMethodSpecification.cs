using System;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace System.Reflection.Emit.Quire.Mpl;

public partial class MetadataReferenceTracker
{
    public MethodSpecificationHandle GetOrAddMethodSpecification(MethodInfo method)
    {
        EntityWrappers.MethodReferenceWrapper key = new EntityWrappers.MethodReferenceWrapper(method);
        if (_methodSpecs.TryGetValue(key, out var value))
        {
            return value;
        }
        if (method.IsGenericMethod && !method.IsGenericMethodDefinition)
        {
            MethodInfo genericMethodDefinition = method.GetGenericMethodDefinition();
            MemberReferenceHandle orAddMethodReference = GetOrAddMethodReference(genericMethodDefinition);
            Type[] genericArguments = method.GetGenericArguments();
            BlobBuilder blobBuilder = new BlobBuilder();
            BlobEncoder blobEncoder = new BlobEncoder(blobBuilder);
            GenericTypeArgumentsEncoder genericTypeArgumentsEncoder = blobEncoder.MethodSpecificationSignature(genericArguments.Length);
            Type[] methodTypeArguments = method.GetGenericArguments();
            foreach (Type type in methodTypeArguments)
            {
                genericTypeArgumentsEncoder.AddArgument().FromType(type);
            }
            MethodSpecificationHandle methodSpecHandle = _metadata.AddMethodSpecification(orAddMethodReference, _metadata.GetOrAddBlob(blobBuilder));
            _methodSpecs[key] = methodSpecHandle;
            return value;
        }
        throw new ArgumentException("The type should be generic with specified arguments!");
    }
}
