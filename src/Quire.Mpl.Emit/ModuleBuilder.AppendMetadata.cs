// File-name letters AZ and B set the compile order of the ModuleBuilder partials; Roslyn numbers closure ordinals in that order.
// Renaming them fails the Release IL gate (closure ordinals shift).
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
        foreach (TypeBuilder typeDef in _typeDefStore)
        {
            _currentType = typeDef;
            _currentTypeHandle = _definedTypes[new EntityWrappers.TypeReferenceWrapper(typeDef)];
            if (!_currentTypeHandle.HasValue)
            {
                throw new InvalidOperationException("_currentTypeHandle can not be null");
            }
            foreach (FieldBuilder fieldDef in typeDef._fieldDefStore)
            {
                FieldDefinitionHandle fieldDefHandle = MetadataHelper.AddFieldDefintion(metadata, typeDef, _currentTypeHandle.Value, fieldDef);
                _definedFields[new EntityWrappers.FieldReferenceWrapper(fieldDef)] = fieldDefHandle;
            }
        }
        foreach (TypeBuilder typeDef in _typeDefStore)
        {
            _currentType = typeDef;
            _currentTypeHandle = _definedTypes[new EntityWrappers.TypeReferenceWrapper(typeDef)];
            if (!_currentTypeHandle.HasValue)
            {
                throw new InvalidOperationException("_currentTypeHandle can not be null");
            }
            List<int> list = dynamicAssemblyModel.MethodOffset(typeDef);
            int num = 0;
            foreach (object methodDef in typeDef._methodDefStore)
            {
                MethodDefinitionHandle methodDefHandle;
                if (methodDef is ConstructorBuilder)
                {
                    ConstructorBuilder constructorBuilder = (methodDef as ConstructorBuilder) ?? throw new InvalidOperationException("Operand must be a ConstructorBuilder");
                    methodDefHandle = MetadataHelper.AddConstructorDefintionNoBodyWithOffset(metadata, _metadataReferenceTracker, typeDef, _currentTypeHandle.Value, constructorBuilder, methodBodyStreamEncoder, list[num++]);
                    _definedConstructors[new EntityWrappers.ConstructorReferenceWrapper(constructorBuilder)] = methodDefHandle;
                }
                else if (methodDef is MethodBuilder)
                {
                    MethodBuilder methodBuilder = (methodDef as MethodBuilder) ?? throw new InvalidOperationException("Operand must be a MethodBuilder");
                    methodDefHandle = MetadataHelper.AddMethodDefintionNoBodyWithOffset(metadata, _metadataReferenceTracker, typeDef, _currentTypeHandle.Value, methodBuilder, methodBodyStreamEncoder, list[num++]);
                    _definedMethods[new EntityWrappers.MethodReferenceWrapper(methodBuilder)] = methodDefHandle;
                }
            }
        }
        foreach (TypeBuilder typeDef in _typeDefStore)
        {
            _currentType = typeDef;
            _currentTypeHandle = _definedTypes[new EntityWrappers.TypeReferenceWrapper(typeDef)];
            if (!_currentTypeHandle.HasValue)
            {
                throw new InvalidOperationException("_currentTypeHandle can not be null");
            }
            List<int> methodOffsets = dynamicAssemblyModel.MethodOffset(typeDef);
            int bodyIndex = 0;
            foreach (object methodDef in typeDef._methodDefStore)
            {
                if (methodDef is ConstructorBuilder)
                {
                    ConstructorBuilder ctrBuilder = (methodDef as ConstructorBuilder) ?? throw new InvalidOperationException("Operand must be a ConstructorBuilder");
                    if (MetadataHelper.AddConstructorDefintionBody(metadata, _metadataReferenceTracker, typeDef, _currentTypeHandle.Value, ctrBuilder, methodBodyStreamEncoder) != methodOffsets[bodyIndex++])
                    {
                        throw new InvalidOperationException("Wrong offset value");
                    }
                }
                else if (methodDef is MethodBuilder)
                {
                    MethodBuilder methodBuilder = (methodDef as MethodBuilder) ?? throw new InvalidOperationException("Operand must be a MethodBuilder");
                    if (MetadataHelper.AddMethodDefintionBody(metadata, _metadataReferenceTracker, typeDef, _currentTypeHandle.Value, methodBuilder, methodBodyStreamEncoder) != methodOffsets[bodyIndex++])
                    {
                        throw new InvalidOperationException("Wrong offset value");
                    }
                }
            }
        }
        foreach (TypeBuilder typeDef in _typeDefStore)
        {
            _currentType = typeDef;
            _currentTypeHandle = _definedTypes[new EntityWrappers.TypeReferenceWrapper(typeDef)];
            if (!_currentTypeHandle.HasValue)
            {
                throw new InvalidOperationException("_currentTypeHandle can not be null");
            }
            foreach (EntityWrappers.CustomAttributeWrapper customAttribute in typeDef._customAttributes)
            {
                MemberReferenceHandle orAddConstructorReference = _metadataReferenceTracker.GetOrAddConstructorReference(customAttribute.constructorInfo);
                metadata.AddCustomAttribute(_currentTypeHandle.Value, orAddConstructorReference, metadata.GetOrAddBlob(customAttribute.binaryAttribute));
            }
        }
        AddTargetFrameworkAttribute(metadata, Assembly._assemblyDef, _metadataReferenceTracker);
    }
}
