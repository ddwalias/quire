#nullable enable
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace System.Reflection.Emit.Quire.Mpl;

internal static partial class SignatureHelper
{
    internal static BlobBuilder MethodSignatureEnconder(Type[]? parameters, Type? returnType, bool isInstance)
    {
        BlobBuilder blobBuilder = new BlobBuilder();
        ParametersEncoder parameters2;
        ReturnTypeEncoder returnType2;
        new BlobEncoder(blobBuilder).MethodSignature(SignatureCallingConvention.Default, 0, isInstance).Parameters(parameters == null ? 0 : parameters.Length, out returnType2, out parameters2);
        if (returnType != null && returnType != typeof(void))
        {
            returnType2.Type().FromType(returnType);
        }
        else
        {
            returnType2.Void();
        }
        if (parameters != null)
        {
            foreach (Type type in parameters)
            {
                parameters2.AddParameter().Type().FromType(type);
            }
        }
        return blobBuilder;
    }

    internal static BlobBuilder FieldSignatureEncoder(TypeBuilder typeBuilder, TypeDefinitionHandle typeHandle, Type type)
    {
        BlobBuilder blobBuilder = new BlobBuilder();
        new BlobEncoder(blobBuilder).FieldSignature().FromType(type, typeBuilder, typeHandle);
        return blobBuilder;
    }
}
