using System;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace System.Reflection.Emit.Quire.Mpl;

internal partial class DynamicAssemblyModel
{
    public DynamicAssemblyModel(ModuleBuilder moduleBuilder, MetadataBuilder metadata, MetadataReferenceTracker referenceTracker, List<TypeBuilder> typesList)
    {
        _typeIndex = 1;
        _fieldIndex = 1;
        _methodIndex = 1;
        _moduleBuilder = moduleBuilder;
        _metadataReferenceTracker = referenceTracker;
        _metadata = metadata;
        BlobBuilder builder = new BlobBuilder();
        _methodBodyStream = new MethodBodyStreamEncoder(builder);
        foreach (TypeBuilder types in typesList)
        {
            AddTypeDefinition(types);
            foreach (FieldBuilder item in types._fieldDefStore)
            {
                AddFieldDefinition(item);
            }
            foreach (object item2 in types._methodDefStore)
            {
                AddMethodDefinition(item2);
            }
        }
        foreach (TypeBuilder types2 in typesList)
        {
            _moduleBuilder._currentType = types2;
            _moduleBuilder._currentTypeHandle = _moduleBuilder._definedTypes[new EntityWrappers.TypeReferenceWrapper(types2)];
            foreach (object item3 in types2._methodDefStore)
            {
                AddMethodBodiesOffsets(types2, item3);
            }
        }
        _moduleBuilder._definedTypes.Clear();
        _moduleBuilder._definedFields.Clear();
        _moduleBuilder._definedMethods.Clear();
        _moduleBuilder._definedConstructors.Clear();
        _moduleBuilder._labelManager.Clear();
    }

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
