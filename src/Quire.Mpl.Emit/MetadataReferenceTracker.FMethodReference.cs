using System;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace System.Reflection.Emit.Quire.Mpl;

public partial class MetadataReferenceTracker
{
    public MemberReferenceHandle GetOrAddMethodReference(MethodInfo method)
	{
		EntityWrappers.MethodReferenceWrapper key = new EntityWrappers.MethodReferenceWrapper(method);
		if (_methodRefs.TryGetValue(key, out var value))
		{
			return value;
		}
		Type type = method.DeclaringType ?? throw new InvalidOperationException("method.DeclaringType must be not null");
		ParameterInfo[] parameters = method.GetParameters();
		bool isStatic = method.IsStatic;
		bool flag = method.IsGenericMethod && method.IsGenericMethodDefinition;
		BlobBuilder blobBuilder = new BlobBuilder();
		BlobEncoder blobEncoder = new BlobEncoder(blobBuilder);
        MethodSignatureEncoder methodSignatureEncoder;
        BlobHandle orAddBlob;
        methodSignatureEncoder = blobEncoder.MethodSignature(SignatureCallingConvention.Default, isInstanceMethod: !isStatic, genericParameterCount: flag ? method.GetGenericArguments().Length : 0);
        methodSignatureEncoder.Parameters(parameters.Length, (ReturnTypeEncoder returnType) =>
		{
			if (method.ReturnType == typeof(void))
			{
				returnType.Void();
			}
			else
			{
				returnType.Type().FromType(method.ReturnType, _module._currentType, _module._currentTypeHandle);
			}
		}, (ParametersEncoder paramEncoder) =>
		{
			ParameterInfo[] array = parameters;
			foreach (ParameterInfo parameterInfo in array)
			{
				paramEncoder.AddParameter().Type().FromType(parameterInfo.ParameterType, _module._currentType, _module._currentTypeHandle);
			}
		});
        orAddBlob = _metadata.GetOrAddBlob(blobBuilder);
		if (type.IsGenericType)
		{
			TypeSpecificationHandle orAddTypeSpecification = GetOrAddTypeSpecification(type);
			value = _metadata.AddMemberReference(orAddTypeSpecification, _metadata.GetOrAddString(method.Name), orAddBlob);
		}
		else
		{
			TypeReferenceHandle orAddTypeReference = GetOrAddTypeReference(type);
			value = _metadata.AddMemberReference(orAddTypeReference, _metadata.GetOrAddString(method.Name), orAddBlob);
		}
		_methodRefs[key] = value;
		return value;
	}
}
