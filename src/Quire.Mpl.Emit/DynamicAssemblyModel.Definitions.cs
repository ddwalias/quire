namespace System.Reflection.Emit.Quire.Mpl;

internal partial class DynamicAssemblyModel
{
    private void AddTypeDefinition(TypeBuilder typeBuilder)
    {
        TypeIndexInfo value = new TypeIndexInfo
        {
            typeIndex = _typeIndex++,
            fieldIndex = _fieldIndex,
            methodIndex = _methodIndex
        };
        _typeIndexes.Add(new EntityWrappers.TypeReferenceWrapper(typeBuilder), value);
        _moduleBuilder._definedTypes[new EntityWrappers.TypeReferenceWrapper(typeBuilder)] = default;
    }

    private void AddFieldDefinition(FieldBuilder field)
    {
        _fieldIndex++;
        _moduleBuilder._definedFields[new EntityWrappers.FieldReferenceWrapper(field)] = default;
    }
}
