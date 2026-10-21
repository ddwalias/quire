using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace System.Reflection.Emit.Quire.Mpl;

public partial class ModuleBuilder
{
    internal void AppendMetadata(MetadataBuilder metadata, MethodBodyStreamEncoder methodBodyStreamEncoder)
    {
        metadata.AddModule(0, metadata.GetOrAddString(ScopeName), metadata.GetOrAddGuid(Guid.NewGuid()), default, default);
        _metadataReferenceTracker = new MetadataReferenceTracker(metadata, this);
        metadata.AddTypeDefinition(TypeAttributes.NotPublic, default, metadata.GetOrAddString("<Module>"), default, MetadataTokens.FieldDefinitionHandle(1), MetadataTokens.MethodDefinitionHandle(1));
        DynamicAssemblyModel dynamicAssemblyModel = new DynamicAssemblyModel(this, metadata, _metadataReferenceTracker, _typeDefStore);
        foreach (TypeBuilder item in _typeDefStore)
        {
            int fieldToken = dynamicAssemblyModel.FieldIndex(item);
            int methodToken = dynamicAssemblyModel.MethodIndex(item);
            TypeDefinitionHandle value = MetadataHelper.AddTypeDef(item, metadata, _metadataReferenceTracker, fieldToken, methodToken);
            _definedTypes.Add(new EntityWrappers.TypeReferenceWrapper(item), value);
        }
        foreach (TypeBuilder item2 in _typeDefStore)
        {
            _currentType = item2;
            _currentTypeHandle = _definedTypes[new EntityWrappers.TypeReferenceWrapper(item2)];
            if (!_currentTypeHandle.HasValue)
            {
                throw new InvalidOperationException("_currentTypeHandle can not be null");
            }
            foreach (FieldBuilder item3 in item2._fieldDefStore)
            {
                FieldDefinitionHandle value2 = MetadataHelper.AddFieldDefintion(metadata, item2, _currentTypeHandle.Value, item3);
                _definedFields[new EntityWrappers.FieldReferenceWrapper(item3)] = value2;
            }
        }
        foreach (TypeBuilder item4 in _typeDefStore)
        {
            _currentType = item4;
            _currentTypeHandle = _definedTypes[new EntityWrappers.TypeReferenceWrapper(item4)];
            if (!_currentTypeHandle.HasValue)
            {
                throw new InvalidOperationException("_currentTypeHandle can not be null");
            }
            List<int> list = dynamicAssemblyModel.MethodOffset(item4);
            int num = 0;
            foreach (object item5 in item4._methodDefStore)
            {
                MethodDefinitionHandle value3;
                if (item5 is ConstructorBuilder)
                {
                    ConstructorBuilder constructorBuilder = (item5 as ConstructorBuilder) ?? throw new InvalidOperationException("Operand must be a ConstructorBuilder");
                    value3 = MetadataHelper.AddConstructorDefintionNoBodyWithOffset(metadata, _metadataReferenceTracker, item4, _currentTypeHandle.Value, constructorBuilder, methodBodyStreamEncoder, list[num++]);
                    _definedConstructors[new EntityWrappers.ConstructorReferenceWrapper(constructorBuilder)] = value3;
                }
                else if (item5 is MethodBuilder)
                {
                    MethodBuilder methodBuilder = (item5 as MethodBuilder) ?? throw new InvalidOperationException("Operand must be a MethodBuilder");
                    value3 = MetadataHelper.AddMethodDefintionNoBodyWithOffset(metadata, _metadataReferenceTracker, item4, _currentTypeHandle.Value, methodBuilder, methodBodyStreamEncoder, list[num++]);
                    _definedMethods[new EntityWrappers.MethodReferenceWrapper(methodBuilder)] = value3;
                }
            }
        }
        foreach (TypeBuilder item6 in _typeDefStore)
        {
            _currentType = item6;
            _currentTypeHandle = _definedTypes[new EntityWrappers.TypeReferenceWrapper(item6)];
            if (!_currentTypeHandle.HasValue)
            {
                throw new InvalidOperationException("_currentTypeHandle can not be null");
            }
            List<int> list2 = dynamicAssemblyModel.MethodOffset(item6);
            int num2 = 0;
            foreach (object item7 in item6._methodDefStore)
            {
                if (item7 is ConstructorBuilder)
                {
                    ConstructorBuilder ctrBuilder = (item7 as ConstructorBuilder) ?? throw new InvalidOperationException("Operand must be a ConstructorBuilder");
                    if (MetadataHelper.AddConstructorDefintionBody(metadata, _metadataReferenceTracker, item6, _currentTypeHandle.Value, ctrBuilder, methodBodyStreamEncoder) != list2[num2++])
                    {
                        throw new InvalidOperationException("Wrong offset value");
                    }
                }
                else if (item7 is MethodBuilder)
                {
                    MethodBuilder methodBuilder2 = (item7 as MethodBuilder) ?? throw new InvalidOperationException("Operand must be a MethodBuilder");
                    if (MetadataHelper.AddMethodDefintionBody(metadata, _metadataReferenceTracker, item6, _currentTypeHandle.Value, methodBuilder2, methodBodyStreamEncoder) != list2[num2++])
                    {
                        throw new InvalidOperationException("Wrong offset value");
                    }
                }
            }
        }
        foreach (TypeBuilder item8 in _typeDefStore)
        {
            _currentType = item8;
            _currentTypeHandle = _definedTypes[new EntityWrappers.TypeReferenceWrapper(item8)];
            if (!_currentTypeHandle.HasValue)
            {
                throw new InvalidOperationException("_currentTypeHandle can not be null");
            }
            foreach (EntityWrappers.CustomAttributeWrapper customAttribute in item8._customAttributes)
            {
                MemberReferenceHandle orAddConstructorReference = _metadataReferenceTracker.GetOrAddConstructorReference(customAttribute.constructorInfo);
                metadata.AddCustomAttribute(_currentTypeHandle.Value, orAddConstructorReference, metadata.GetOrAddBlob(customAttribute.binaryAttribute));
            }
        }
        AddTargetFrameworkAttribute(metadata, Assembly._assemblyDef, _metadataReferenceTracker);
    }
}
