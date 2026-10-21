using System;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace System.Reflection.Emit.Quire.Mpl;

public partial class MetadataReferenceTracker
{
    public TypeSpecificationHandle GetOrAddTypeSpecification(Type type)
    {
        EntityWrappers.TypeReferenceWrapper key = new EntityWrappers.TypeReferenceWrapper(type);
        if (_typeSpecs.TryGetValue(key, out var value))
        {
            return value;
        }
        if (type.IsGenericType)
        {
            Type genericTypeDefinition = type.GetGenericTypeDefinition();
            TypeReferenceHandle orAddTypeReference = GetOrAddTypeReference(genericTypeDefinition);
            Type[] genericArguments = type.GetGenericArguments();
            BlobBuilder blobBuilder = new BlobBuilder(256);
            BlobEncoder blobEncoder = new BlobEncoder(blobBuilder);
            SignatureTypeEncoder encoder = new SignatureTypeEncoder(blobEncoder.Builder);
            encoder.GenericInstantiation(orAddTypeReference, genericArguments.Length, isValueType: false);
            Type[] array = genericArguments;
            foreach (Type type2 in array)
            {
                if (type2.IsGenericParameter)
                {
                    encoder.GenericTypeParameter(type2.GenericParameterPosition);
                }
                else
                {
                    TypeSignatureEncoderExtensions.FromType(encoder, type2, null, default);
                }
            }
            value = _metadata.AddTypeSpecification(_metadata.GetOrAddBlob(blobBuilder));
        }
        else
        {
            throw new ArgumentException("The type should be generic!");
        }
        _typeSpecs[key] = value;
        return value;
    }
}
