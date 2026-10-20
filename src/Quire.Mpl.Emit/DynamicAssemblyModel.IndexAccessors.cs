using System.Collections.Generic;

namespace System.Reflection.Emit.Quire.Mpl;

internal partial class DynamicAssemblyModel
{
    public int MethodIndex(TypeBuilder typeBuilder)
    {
        return _typeIndexes[new EntityWrappers.TypeReferenceWrapper(typeBuilder)].methodIndex;
    }

    public int FieldIndex(TypeBuilder typeBuilder)
    {
        return _typeIndexes[new EntityWrappers.TypeReferenceWrapper(typeBuilder)].fieldIndex;
    }

    public List<int> MethodOffset(TypeBuilder typeBuilder)
    {
        return _typeIndexes[new EntityWrappers.TypeReferenceWrapper(typeBuilder)].offsets;
    }
}
