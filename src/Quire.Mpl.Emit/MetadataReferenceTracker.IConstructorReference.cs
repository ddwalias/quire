using System;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace System.Reflection.Emit.Quire.Mpl;

public partial class MetadataReferenceTracker
{
    public MemberReferenceHandle GetOrAddConstructorReference(ConstructorInfo ctor)
    {
        EntityWrappers.ConstructorReferenceWrapper key = new EntityWrappers.ConstructorReferenceWrapper(ctor);
        if (_constructorRefs.TryGetValue(key, out var value))
        {
            return value;
        }
        Type type = ctor.DeclaringType ?? throw new InvalidOperationException("ctor.DeclaringType must be not null");
        ParameterInfo[] parameters = ctor.GetParameters();
        bool isStatic = ctor.IsStatic;
        BlobBuilder blobBuilder = new BlobBuilder();
        BlobEncoder blobEncoder = new BlobEncoder(blobBuilder);
        MethodSignatureEncoder methodSignatureEncoder = blobEncoder.MethodSignature(SignatureCallingConvention.Default, 0, !isStatic);
        methodSignatureEncoder.Parameters(parameters.Length, (ReturnTypeEncoder returnType) =>
        {
            returnType.Void();
        }, (ParametersEncoder paramEncoder) =>
        {
            ParameterInfo[] array = parameters;
            foreach (ParameterInfo parameterInfo in array)
            {
                paramEncoder.AddParameter().Type().FromType(parameterInfo.ParameterType, _module._currentType, _module._currentTypeHandle);
            }
        });
        BlobHandle orAddBlob = _metadata.GetOrAddBlob(blobBuilder);
        if (type.IsGenericType)
        {
            TypeSpecificationHandle orAddTypeSpecification = GetOrAddTypeSpecification(type);
            value = _metadata.AddMemberReference(orAddTypeSpecification, _metadata.GetOrAddString(ctor.Name), orAddBlob);
        }
        else
        {
            TypeReferenceHandle orAddTypeReference = GetOrAddTypeReference(type);
            value = _metadata.AddMemberReference(orAddTypeReference, _metadata.GetOrAddString(ctor.Name), orAddBlob);
        }
        _constructorRefs[key] = value;
        return value;
    }
}
