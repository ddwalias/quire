using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace System.Reflection.Emit.Quire.Mpl;

internal partial class LocalVariableManager
{
    private List<Type> _locals = new List<Type>();
    private MethodBase _method;

    public int Count => _locals.Count;

    public LocalBuilder DeclareLocal(Type type, bool pinned, string tag = null)
    {
        int count = _locals.Count;
        _locals.Add(type);
        return new LocalBuilder(count, type, _method, pinned);
    }

    public LocalVariableManager(MethodBase method)
    {
        _method = method;
    }

    public StandaloneSignatureHandle CreateSignature(MetadataBuilder metadata, TypeBuilder typeBuilder, TypeDefinitionHandle typeHandle)
    {
        BlobBuilder blobBuilder = new BlobBuilder();
        BlobEncoder blobEncoder = new BlobEncoder(blobBuilder);
        LocalVariablesEncoder localVariablesEncoder = blobEncoder.LocalVariableSignature(_locals.Count);
        foreach (Type local in _locals)
        {
            localVariablesEncoder.AddVariable().Type().FromType(local, typeBuilder, typeHandle);
        }
        return metadata.AddStandaloneSignature(metadata.GetOrAddBlob(blobBuilder));
    }
}
