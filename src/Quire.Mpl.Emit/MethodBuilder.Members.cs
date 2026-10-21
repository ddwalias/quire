using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;

namespace System.Reflection.Emit.Quire.Mpl;

public partial class MethodBuilder
{
    public override string Name { get; }

    public override MethodAttributes Attributes { get; }

    public override CallingConventions CallingConvention { get; }

    public override TypeBuilder DeclaringType { get; }

    public override Module Module { get; }

    public override bool ContainsGenericParameters => throw new NotImplementedException();

    public bool InitLocals
    {
        get => throw new NotImplementedException();
        set { }
    }

    public override bool IsGenericMethod => throw new NotImplementedException();

    public override bool IsGenericMethodDefinition => throw new NotImplementedException();

    public override bool IsSecurityCritical => throw new NotImplementedException();

    public override bool IsSecuritySafeCritical => throw new NotImplementedException();

    public override bool IsSecurityTransparent => throw new NotImplementedException();

    public override int MetadataToken => throw new NotImplementedException();

    public override RuntimeMethodHandle MethodHandle => throw new NotImplementedException();

    public override Type? ReflectedType => throw new NotImplementedException();

    public override ParameterInfo ReturnParameter => throw new NotImplementedException();

    public override Type ReturnType => _returnType ?? throw new InvalidOperationException("_returnType must be not null");

    public override ICustomAttributeProvider ReturnTypeCustomAttributes => throw new NotImplementedException();

    internal MethodBuilder(string name, MethodAttributes attributes, CallingConventions callingConventions, Type? returnType, Type[]? parameters, TypeBuilder declaringType)
    {
        Name = name;
        Attributes = attributes;
        CallingConvention = callingConventions;
        DeclaringType = declaringType;
        Module = declaringType.Module;
        _returnType = returnType;
        _parameters = parameters;
        _localVariableManager = new LocalVariableManager(this);
        _labelManager = declaringType.Module._labelManager;
        _codeStore = new ILGeneratorStore(this, CommonMemberData.FromMethodBuilder(this));
        System.Reflection.Emit.DynamicMethod dynamicMethod = new System.Reflection.Emit.DynamicMethod("Dummy", typeof(void), _parameters);
        _paramInfos = dynamicMethod.GetParameters();
    }

    public ParameterBuilder DefineParameter(int position, ParameterAttributes attributes, string? strParamName) => throw new NotImplementedException();

    public void SetImplementationFlags(MethodImplAttributes attributes) => throw new NotImplementedException();

    public void SetParameters(params Type[] parameterTypes) => throw new NotImplementedException();

    public void SetReturnType(Type? returnType) => throw new NotImplementedException();

    public void SetSignature(Type? returnType, Type[]? returnTypeRequiredCustomModifiers, Type[]? returnTypeOptionalCustomModifiers, Type[]? parameterTypes, Type[][]? parameterTypeRequiredCustomModifiers, Type[][]? parameterTypeOptionalCustomModifiers) => throw new NotImplementedException();

    public GenericTypeParameterBuilder[] DefineGenericParameters(params string[] names) => throw new NotImplementedException();

    public override MethodInfo GetBaseDefinition() => throw new NotImplementedException();

    public override object[] GetCustomAttributes(bool inherit) => throw new NotImplementedException();

    public override object[] GetCustomAttributes(Type attributeType, bool inherit) => throw new NotImplementedException();

    public override Type[] GetGenericArguments() => throw new NotImplementedException();

    public override MethodInfo GetGenericMethodDefinition() => throw new NotImplementedException();

    public override int GetHashCode() => throw new NotImplementedException();

    public ILGenerator GetILGenerator() => _codeStore;

    public System.Reflection.Emit.ILGenerator GetILGenerator(int size) => throw new NotImplementedException();

    public override MethodImplAttributes GetMethodImplementationFlags() => throw new NotImplementedException();

    public override ParameterInfo[] GetParameters() => _paramInfos;

    public override object? Invoke(object? obj, BindingFlags invokeAttr, Binder? binder, object?[]? parameters, CultureInfo? culture) => throw new NotImplementedException();

    public override bool IsDefined(Type attributeType, bool inherit) => throw new NotImplementedException();

    [RequiresDynamicCode("The native code for this instantiation might not be available at runtime.")]
    [RequiresUnreferencedCode("If some of the generic arguments are annotated (either with DynamicallyAccessedMembersAttribute, or generic constraints), trimming can't validate that the requirements of those annotations are met.")]
    public override MethodInfo MakeGenericMethod(params Type[] typeArguments) => throw new NotImplementedException();

    public void SetCustomAttribute(ConstructorInfo con, byte[] binaryAttribute) => throw new NotImplementedException();

    public void SetCustomAttribute(System.Reflection.Emit.CustomAttributeBuilder customBuilder) => throw new NotImplementedException();

    public override string? ToString() => base.ToString();
}
