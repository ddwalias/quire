using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace System.Reflection.Emit.Quire.Mpl;

internal static partial class MetadataHelper
{
    internal static TypeDefinitionHandle AddTypeDef(TypeBuilder typeBuilder, MetadataBuilder metadata, MetadataReferenceTracker referencesTracker, int fieldToken, int methodToken)
    {
        Type type = typeBuilder.parentType ?? typeof(object);
        TypeReferenceHandle orAddTypeReference = referencesTracker.GetOrAddTypeReference(type);
        return metadata.AddTypeDefinition(typeBuilder.UserTypeAttribute, (typeBuilder.Namespace == null) ? default(StringHandle) : metadata.GetOrAddString(typeBuilder.Namespace), metadata.GetOrAddString(typeBuilder.Name), orAddTypeReference, MetadataTokens.FieldDefinitionHandle(fieldToken), MetadataTokens.MethodDefinitionHandle(methodToken));
    }
}
