using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;

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

    internal static void DefineCustomAttribute(ModuleBuilder mod, int tkOwner, object value, byte[] m_blob)
    {
        throw new NotImplementedException();
    }

    public MethodBuilder DefineMethod(string name, MethodAttributes attributes)
    {
        throw new NotImplementedException();
    }

    public void AddInterfaceImplementation(Type interfaceType)
    {
        throw new NotImplementedException();
    }

    public TypeInfo? CreateTypeInfo()
    {
        throw new NotImplementedException();
    }

    private bool _IsInterface()
    {
        return (UserTypeAttribute & TypeAttributes.ClassSemanticsMask) == TypeAttributes.ClassSemanticsMask;
    }

    private bool IsStatic()
    {
        if ((UserTypeAttribute & TypeAttributes.Abstract) == TypeAttributes.Abstract)
        {
            return (UserTypeAttribute & TypeAttributes.Sealed) == TypeAttributes.Sealed;
        }
        return false;
    }

    public void DefineMethodOverride(MethodInfo methodInfoBody, MethodInfo methodInfoDeclaration)
    {
        throw new NotImplementedException();
    }

    public void SetParent(Type? parent)
    {
        throw new NotImplementedException();
    }

    private bool HasConstructor()
    {
        return _methodDefStore.Any((object item) =>
        {
            MethodBuilder methodBuilder = item as MethodBuilder;
            return (methodBuilder is not null && methodBuilder.Name == ".ctor") || (item is ConstructorBuilder constructorBuilder && constructorBuilder.Name == ".ctor");
        });
    }

    public MethodBuilder DefineMethod(string name, MethodAttributes attributes, CallingConventions callingConvention, Type? returnType, Type[]? parameterTypes)
    {
        MethodBuilder methodBuilder = new MethodBuilder(name, attributes, callingConvention, returnType, parameterTypes, this);
        _methodDefStore.Add(methodBuilder);
        return methodBuilder;
    }

    public ConstructorBuilder DefineConstructor(MethodAttributes attributes, CallingConventions callingConvention, Type[]? parameterTypes)
    {
        string name = (((attributes & MethodAttributes.Static) == 0) ? ".ctor" : ".cctor");
        if ((attributes & MethodAttributes.SpecialName) == 0)
        {
            attributes |= MethodAttributes.SpecialName;
        }
        if ((attributes & MethodAttributes.RTSpecialName) == 0)
        {
            attributes |= MethodAttributes.RTSpecialName;
        }
        if ((attributes & MethodAttributes.HideBySig) == 0)
        {
            attributes |= MethodAttributes.HideBySig;
        }
        ConstructorBuilder constructorBuilder = new ConstructorBuilder(name, attributes, callingConvention, typeof(void), parameterTypes, this);
        _methodDefStore.Add(constructorBuilder);
        return constructorBuilder;
    }

    public FieldBuilder DefineField(string fieldName, Type type, FieldAttributes attributes)
    {
        FieldBuilder fieldBuilder = new FieldBuilder(Module, Assembly, fieldName, type, attributes, this);
        _fieldDefStore.Add(fieldBuilder);
        return fieldBuilder;
    }

    public MethodBuilder DefineMethod(string name, MethodAttributes attributes, Type? returnType, Type[]? parameterTypes)
    {
        return DefineMethod(name, attributes, (CallingConventions)0, returnType, parameterTypes);
    }

    protected override TypeAttributes GetAttributeFlagsImpl()
    {
        return UserTypeAttribute;
    }

    public void SetCustomAttribute(CustomAttributeBuilder customBuilder)
    {
        SetCustomAttribute(customBuilder.Constructor, customBuilder._blob);
    }

    public void SetCustomAttribute(ConstructorInfo constructorInfo, byte[] binaryAttribute)
    {
        if (constructorInfo == null)
        {
            throw new ArgumentNullException("constructorInfo");
        }
        if (binaryAttribute == null)
        {
            throw new ArgumentNullException("binaryAttribute");
        }
        if (constructorInfo.DeclaringType == null)
        {
            throw new ArgumentException("Attribute constructor has no type.");
        }
        if (constructorInfo.DeclaringType.Name.Equals("ComImportAttribute"))
        {
            UserTypeAttribute |= TypeAttributes.Import;
            return;
        }
        EntityWrappers.CustomAttributeWrapper item = new EntityWrappers.CustomAttributeWrapper(constructorInfo, binaryAttribute);
        _customAttributes.Add(item);
    }

    public Type? CreateType()
    {
        if (!IsInterface && !IsStatic() && !HasConstructor())
        {
            Type type = ((parentType == null) ? typeof(object) : parentType);
            ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
            if (constructor == null)
            {
                throw new InvalidOperationException(type.Name + " must have a parameterless .ctor");
            }
            ILGenerator iLGenerator = DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, Type.EmptyTypes).GetILGenerator();
            iLGenerator.Emit(OpCodes.Ldarg_0);
            iLGenerator.Emit(OpCodes.Call, constructor);
            iLGenerator.Emit(OpCodes.Ret);
        }
        return this;
    }
}
