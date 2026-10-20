using System;
using System.Reflection.Metadata;

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

    private void AddMethodBodiesOffsets(TypeBuilder typeBuilder, object item)
    {
        int item2 = 0;
        TypeDefinitionHandle typeHandle = _moduleBuilder._currentTypeHandle ?? throw new InvalidOperationException("_moduleBuilder._currentTypeHandle can not be null");
        if (item is ConstructorBuilder)
        {
            ConstructorBuilder ctrBuilder = item as ConstructorBuilder ?? throw new InvalidOperationException("Operand must be a ConstructorBuilder");
            item2 = MetadataHelper.AddConstructorDefintionBody(_metadata, _metadataReferenceTracker, typeBuilder, typeHandle, ctrBuilder, _methodBodyStream);
        }
        else if (item is MethodBuilder)
        {
            MethodBuilder methodBuilder = item as MethodBuilder ?? throw new InvalidOperationException("Operand must be a MethodBuilder");
            item2 = MetadataHelper.AddMethodDefintionBody(_metadata, _metadataReferenceTracker, typeBuilder, typeHandle, methodBuilder, _methodBodyStream);
        }
        _typeIndexes[new EntityWrappers.TypeReferenceWrapper(typeBuilder)].offsets.Add(item2);
    }
}
