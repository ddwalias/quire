using System;
using System.Reflection;
using System.Reflection.Emit;

namespace System.Reflection.Emit.Quire.Mpl;

public partial class TypeBuilder
{
    internal List<object> _methodDefStore = new List<object>();
    internal List<FieldBuilder> _fieldDefStore = new List<FieldBuilder>();
    internal List<EntityWrappers.CustomAttributeWrapper> _customAttributes = new List<EntityWrappers.CustomAttributeWrapper>();

    public override string Name { get; }
    public override AssemblyBuilder Assembly { get; }
    public override ModuleBuilder Module { get; }
    public override string? Namespace { get; }
    public Type? parentType { get; }
    internal TypeAttributes UserTypeAttribute { get; set; }

    public override string? AssemblyQualifiedName => throw new NotImplementedException();
    public override Type? BaseType => parentType;
    public override MethodBase? DeclaringMethod => throw new NotImplementedException();
    public override Type? DeclaringType => throw new NotImplementedException();

    public override string? FullName
    {
        get
        {
            if (!string.IsNullOrEmpty(Namespace))
            {
                return Namespace + "." + Name;
            }
            return Name;
        }
    }

    public override GenericParameterAttributes GenericParameterAttributes => throw new NotImplementedException();
    public override int GenericParameterPosition => throw new NotImplementedException();
    public override Guid GUID => throw new NotImplementedException();
    public override bool IsByRefLike => throw new NotImplementedException();
    public override bool IsConstructedGenericType => throw new NotImplementedException();
    public override bool IsGenericParameter => throw new NotImplementedException();
    public override bool IsGenericType => throw new NotImplementedException();
    public override bool IsGenericTypeDefinition => throw new NotImplementedException();
    public override bool IsSecurityCritical => throw new NotImplementedException();
    public override bool IsSecuritySafeCritical => throw new NotImplementedException();
    public override bool IsSecurityTransparent => throw new NotImplementedException();
    public override bool IsSZArray => throw new NotImplementedException();
    public override bool IsTypeDefinition => throw new NotImplementedException();
    public override int MetadataToken => throw new NotImplementedException();
    public PackingSize PackingSize => throw new NotImplementedException();
    public override Type? ReflectedType => throw new NotImplementedException();
    public int Size => throw new NotImplementedException();
    public override RuntimeTypeHandle TypeHandle => throw new NotImplementedException();
    public override Type UnderlyingSystemType => throw new NotImplementedException();

    internal TypeBuilder(string name, ModuleBuilder module, AssemblyBuilder assemblyBuilder, TypeAttributes typeAttributes, Type parent)
    {
        Name = name;
        Module = module;
        Assembly = assemblyBuilder;
        UserTypeAttribute = typeAttributes;
        parentType = parent;
        int num = Name.LastIndexOf('.');
        Namespace = string.Empty;
        if (num != -1)
        {
            Namespace = Name.Substring(0, num);
            Name = Name.Substring(num + 1);
        }
    }
}
