using System;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.Versioning;

namespace System.Reflection.Emit.Quire.Mpl;

public partial class ModuleBuilder
{
    private void AddTargetFrameworkAttribute(MetadataBuilder metadata, AssemblyDefinitionHandle assemblyHandle, MetadataReferenceTracker metadataReferenceTracker)
	{
		TypeReferenceHandle orAddTypeReference = metadataReferenceTracker.GetOrAddTypeReference(typeof(TargetFrameworkAttribute));
		BlobBuilder blobBuilder = new BlobBuilder();
		new BlobEncoder(blobBuilder).MethodSignature(SignatureCallingConvention.Default, 0, isInstanceMethod: true).Parameters(1, (ReturnTypeEncoder returnType) =>
		{
			returnType.Void();
		}, (ParametersEncoder parameters) =>
		{
			parameters.AddParameter().Type().String();
		});
		MemberReferenceHandle memberReferenceHandle = metadata.AddMemberReference(orAddTypeReference, metadata.GetOrAddString(".ctor"), metadata.GetOrAddBlob(blobBuilder));
		BlobBuilder blobBuilder2 = new BlobBuilder();
		blobBuilder2.WriteUInt16(1);
		blobBuilder2.WriteSerializedString(".NETCoreApp,Version=v10.0");
		blobBuilder2.WriteUInt16(1);
		blobBuilder2.WriteByte(83);
		blobBuilder2.WriteByte(14);
		blobBuilder2.WriteSerializedString("FrameworkDisplayName");
		blobBuilder2.WriteSerializedString(".NET 10.0");
		metadata.AddCustomAttribute(assemblyHandle, memberReferenceHandle, metadata.GetOrAddBlob(blobBuilder2));
	}
}
