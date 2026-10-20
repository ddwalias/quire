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

    private void AddMethodDefinition(object item)
    {
        _methodIndex++;
        if (item is ConstructorBuilder)
        {
            ConstructorBuilder method = item as ConstructorBuilder ?? throw new InvalidOperationException("Operand must be a ConstructorBuilder");
            _moduleBuilder._definedConstructors[new EntityWrappers.ConstructorReferenceWrapper(method)] = default;
            return;
        }

        if (item is MethodBuilder)
        {
            MethodBuilder method2 = item as MethodBuilder ?? throw new InvalidOperationException("Operand must be a MethodBuilder");
            _moduleBuilder._definedMethods[new EntityWrappers.MethodReferenceWrapper(method2)] = default;
        }
    }
}
