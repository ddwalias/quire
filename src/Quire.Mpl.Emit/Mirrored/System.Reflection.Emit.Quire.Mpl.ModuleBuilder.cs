#nullable disable
#pragma warning disable

namespace System.Reflection.Emit.Quire.Mpl;

partial class ModuleBuilder
{
    internal global::System.Collections.Generic.List<global::System.Reflection.Emit.Quire.Mpl.TypeBuilder> _typeDefStore = new global::System.Collections.Generic.List<global::System.Reflection.Emit.Quire.Mpl.TypeBuilder>();
    internal global::System.Collections.Generic.Dictionary<global::System.Reflection.Emit.Quire.Mpl.EntityWrappers.TypeReferenceWrapper, global::System.Reflection.Metadata.TypeDefinitionHandle> _definedTypes = new global::System.Collections.Generic.Dictionary<global::System.Reflection.Emit.Quire.Mpl.EntityWrappers.TypeReferenceWrapper, global::System.Reflection.Metadata.TypeDefinitionHandle>();
    internal readonly global::System.Collections.Generic.Dictionary<global::System.Reflection.Emit.Quire.Mpl.EntityWrappers.FieldReferenceWrapper, global::System.Reflection.Metadata.FieldDefinitionHandle> _definedFields = new global::System.Collections.Generic.Dictionary<global::System.Reflection.Emit.Quire.Mpl.EntityWrappers.FieldReferenceWrapper, global::System.Reflection.Metadata.FieldDefinitionHandle>();
    internal readonly global::System.Collections.Generic.Dictionary<global::System.Reflection.Emit.Quire.Mpl.EntityWrappers.MethodReferenceWrapper, global::System.Reflection.Metadata.MethodDefinitionHandle> _definedMethods = new global::System.Collections.Generic.Dictionary<global::System.Reflection.Emit.Quire.Mpl.EntityWrappers.MethodReferenceWrapper, global::System.Reflection.Metadata.MethodDefinitionHandle>();
    internal readonly global::System.Collections.Generic.Dictionary<global::System.Reflection.Emit.Quire.Mpl.EntityWrappers.ConstructorReferenceWrapper, global::System.Reflection.Metadata.MethodDefinitionHandle> _definedConstructors = new global::System.Collections.Generic.Dictionary<global::System.Reflection.Emit.Quire.Mpl.EntityWrappers.ConstructorReferenceWrapper, global::System.Reflection.Metadata.MethodDefinitionHandle>();
    internal global::System.Reflection.Emit.Quire.Mpl.LabelManager _labelManager = new global::System.Reflection.Emit.Quire.Mpl.LabelManager();
    public override global::System.Reflection.Emit.Quire.Mpl.AssemblyBuilder Assembly { get; }
    public override string ScopeName { get; }
}
