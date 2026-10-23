#nullable enable
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace System.Reflection.Emit.Quire.Mpl;

internal static partial class SignatureHelper
{
    internal static BlobBuilder MethodSignatureEnconder(Type[]? parameters, Type? returnType, bool isInstance)
    {
        BlobBuilder blobBuilder = new BlobBuilder();
        ParametersEncoder parametersEncoder;
        ReturnTypeEncoder returnTypeEncoder;
        new BlobEncoder(blobBuilder).MethodSignature(SignatureCallingConvention.Default, 0, isInstance).Parameters(parameters == null ? 0 : parameters.Length, out returnTypeEncoder, out parametersEncoder);
        if (returnType != null && returnType != typeof(void))
        {
            returnTypeEncoder.Type().FromType(returnType);
        }
        else
        {
            returnTypeEncoder.Void();
        }
        if (parameters != null)
        {
            foreach (Type type in parameters)
            {
                parametersEncoder.AddParameter().Type().FromType(type);
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
