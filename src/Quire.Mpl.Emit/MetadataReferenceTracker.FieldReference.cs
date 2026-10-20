using System;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace System.Reflection.Emit.Quire.Mpl;

public partial class MetadataReferenceTracker
{
    public MemberReferenceHandle GetOrAddFieldReference(FieldInfo fieldInfo)
    {
        EntityWrappers.FieldReferenceWrapper key = new EntityWrappers.FieldReferenceWrapper(fieldInfo);
        if (_fieldRefs.TryGetValue(key, out var value))
        {
            return value;
        }
        if ((object)fieldInfo.DeclaringType == null)
        {
            throw new InvalidOperationException("fieldInfo.DeclaringType must be not null");
        }
        TypeReferenceHandle orAddTypeReference = GetOrAddTypeReference(fieldInfo.DeclaringType);
        BlobBuilder blobBuilder = new BlobBuilder();
        BlobEncoder blobEncoder = new BlobEncoder(blobBuilder);
        blobEncoder.FieldSignature().FromType(fieldInfo.FieldType, _module._currentType, _module._currentTypeHandle);
        BlobHandle orAddBlob = _metadata.GetOrAddBlob(blobBuilder);
        MemberReferenceHandle memberReferenceHandle = _metadata.AddMemberReference(orAddTypeReference, _metadata.GetOrAddString(fieldInfo.Name), orAddBlob);
        _fieldRefs[key] = memberReferenceHandle;
        return memberReferenceHandle;
    }
}
