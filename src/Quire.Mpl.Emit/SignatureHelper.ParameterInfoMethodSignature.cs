using System;
using System.Reflection;
using System.Reflection.Metadata;

namespace System.Reflection.Emit.Quire.Mpl;

internal static partial class SignatureHelper
{
    internal static BlobBuilder MethodSignatureEnconder(ParameterInfo[]? parameters, ParameterInfo? returnType, bool isInstance)
    {
        Type[] parameters2 = null;
        Type returnType2 = null;
        if (parameters != null)
        {
            parameters2 = Array.ConvertAll(parameters, (ParameterInfo parameter) => parameter.ParameterType);
        }
        if (returnType != null)
        {
            returnType2 = returnType.ParameterType;
        }
        return MethodSignatureEnconder(parameters2, returnType2, isInstance);
    }
}
