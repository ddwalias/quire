#nullable disable
#pragma warning disable

namespace System.Reflection.Emit.Quire.Mpl;

partial class DynamicAssemblyModel
{
    private readonly global::System.Collections.Generic.Dictionary<global::System.Reflection.Emit.Quire.Mpl.EntityWrappers.TypeReferenceWrapper, global::System.Reflection.Emit.Quire.Mpl.DynamicAssemblyModel.TypeIndexInfo> _typeIndexes = new global::System.Collections.Generic.Dictionary<global::System.Reflection.Emit.Quire.Mpl.EntityWrappers.TypeReferenceWrapper, global::System.Reflection.Emit.Quire.Mpl.DynamicAssemblyModel.TypeIndexInfo>();
    private int _typeIndex;
    private int _fieldIndex;
    private int _methodIndex;
    private global::System.Reflection.Emit.Quire.Mpl.ModuleBuilder _moduleBuilder;
    private global::System.Reflection.Metadata.Ecma335.MethodBodyStreamEncoder _methodBodyStream;
    private global::System.Reflection.Emit.Quire.Mpl.MetadataReferenceTracker _metadataReferenceTracker;
    private global::System.Reflection.Metadata.Ecma335.MetadataBuilder _metadata;
}
