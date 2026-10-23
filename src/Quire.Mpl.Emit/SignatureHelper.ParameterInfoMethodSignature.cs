using System;
using System.Reflection;
using System.Reflection.Metadata;

namespace System.Reflection.Emit.Quire.Mpl;

internal static partial class SignatureHelper
{
    internal static BlobBuilder MethodSignatureEnconder(ParameterInfo[]? parameters, ParameterInfo? returnType, bool isInstance)
    {
        Type[] parameterTypes = null;
        Type returnValueType = null;
        if (parameters != null)
        {
            parameterTypes = Array.ConvertAll(parameters, (ParameterInfo parameter) => parameter.ParameterType);
        }
        if (returnType != null)
        {
            returnValueType = returnType.ParameterType;
        }
        return MethodSignatureEnconder(parameterTypes, returnValueType, isInstance);
    }
}
