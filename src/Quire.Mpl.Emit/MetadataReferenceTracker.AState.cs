// File-name letters A–J set the compile order of these partials; Roslyn numbers closure ordinals in that order.
// Renaming them fails the Release IL gate (closure ordinals shift).
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace System.Reflection.Emit.Quire.Mpl;

public partial class MetadataReferenceTracker
{
    private readonly MetadataBuilder _metadata;
    private readonly ModuleBuilder _module;
    private readonly Dictionary<EntityWrappers.AssemblyNameReferenceWrapper, AssemblyReferenceHandle> _assemblyRefs = new Dictionary<EntityWrappers.AssemblyNameReferenceWrapper, AssemblyReferenceHandle>();
    private readonly Dictionary<Module, ModuleReferenceHandle> _moduleRefs = new Dictionary<Module, ModuleReferenceHandle>();
    private readonly Dictionary<EntityWrappers.TypeReferenceWrapper, TypeReferenceHandle> _typeRefs = new Dictionary<EntityWrappers.TypeReferenceWrapper, TypeReferenceHandle>();
    private readonly Dictionary<EntityWrappers.TypeReferenceWrapper, TypeSpecificationHandle> _typeSpecs = new Dictionary<EntityWrappers.TypeReferenceWrapper, TypeSpecificationHandle>();
    private readonly Dictionary<EntityWrappers.MethodReferenceWrapper, MemberReferenceHandle> _methodRefs = new Dictionary<EntityWrappers.MethodReferenceWrapper, MemberReferenceHandle>();
    private readonly Dictionary<EntityWrappers.MethodReferenceWrapper, MethodSpecificationHandle> _methodSpecs = new Dictionary<EntityWrappers.MethodReferenceWrapper, MethodSpecificationHandle>();
    private readonly Dictionary<EntityWrappers.ConstructorReferenceWrapper, MemberReferenceHandle> _constructorRefs = new Dictionary<EntityWrappers.ConstructorReferenceWrapper, MemberReferenceHandle>();
    private readonly Dictionary<EntityWrappers.FieldReferenceWrapper, MemberReferenceHandle> _fieldRefs = new Dictionary<EntityWrappers.FieldReferenceWrapper, MemberReferenceHandle>();

    public MetadataReferenceTracker(MetadataBuilder metadata, ModuleBuilder moduleBuilder)
    {
        _metadata = metadata;
        _module = moduleBuilder;
        _instance = this;
    }
}
