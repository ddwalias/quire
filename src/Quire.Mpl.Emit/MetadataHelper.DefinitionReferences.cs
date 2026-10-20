using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace System.Reflection.Emit.Quire.Mpl;

internal static partial class MetadataHelper
{
    internal static MemberReferenceHandle AddConstructorReference(MetadataBuilder metadata, TypeReferenceHandle parent, MethodBase method)
    {
        BlobBuilder value = SignatureHelper.MethodSignatureEnconder(method.GetParameters(), null, isInstance: true);
        return metadata.AddMemberReference(parent, metadata.GetOrAddString(method.Name), metadata.GetOrAddBlob(value));
    }

    internal static FieldDefinitionHandle AddFieldDefintion(MetadataBuilder metadata, TypeBuilder typeBuilder, TypeDefinitionHandle typeHandle, FieldBuilder fieldBuilder)
    {
        BlobBuilder value = SignatureHelper.FieldSignatureEncoder(typeBuilder, typeHandle, fieldBuilder.FieldType);
        return metadata.AddFieldDefinition(fieldBuilder.Attributes, metadata.GetOrAddString(fieldBuilder.Name), metadata.GetOrAddBlob(value));
    }

    internal static MethodDefinitionHandle AddMethodDefintionNoBodyWithOffset(MetadataBuilder metadata, MetadataReferenceTracker referenceTracker, TypeBuilder typeBuilder, TypeDefinitionHandle typeHandle, MethodBuilder methodBuilder, MethodBodyStreamEncoder methodBodyStreamEncoder, int methodBodyOffset)
    {
        BlobBuilder value = SignatureHelper.MethodSignatureEnconder(methodBuilder._parameters, methodBuilder._returnType, !methodBuilder.IsStatic);
        return metadata.AddMethodDefinition(methodBuilder.Attributes, MethodImplAttributes.IL, metadata.GetOrAddString(methodBuilder.Name), metadata.GetOrAddBlob(value), methodBodyOffset, default);
    }
}
