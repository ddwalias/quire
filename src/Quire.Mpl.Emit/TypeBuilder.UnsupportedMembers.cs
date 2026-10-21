using System;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;

namespace System.Reflection.Emit.Quire.Mpl;

public partial class TypeBuilder
{
    public System.Reflection.Emit.ConstructorBuilder DefineDefaultConstructor(MethodAttributes attributes)
    {
        throw new NotImplementedException();
    }

    public EventBuilder DefineEvent(string name, EventAttributes attributes, Type eventtype)
    {
        throw new NotImplementedException();
    }

    public GenericTypeParameterBuilder[] DefineGenericParameters(params string[] names)
    {
        throw new NotImplementedException();
    }

    public System.Reflection.Emit.FieldBuilder DefineInitializedData(string name, byte[] data, FieldAttributes attributes)
    {
        throw new NotImplementedException();
    }

    public System.Reflection.Emit.FieldBuilder DefineUninitializedData(string name, int size, FieldAttributes attributes)
    {
        throw new NotImplementedException();
    }

    public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr)
    {
        throw new NotImplementedException();
    }

    public override object[] GetCustomAttributes(bool inherit)
    {
        throw new NotImplementedException();
    }

    public override object[] GetCustomAttributes(Type attributeType, bool inherit)
    {
        throw new NotImplementedException();
    }

    public override Type GetElementType()
    {
        throw new NotImplementedException();
    }

    public override EventInfo? GetEvent(string name, BindingFlags bindingAttr)
    {
        throw new NotImplementedException();
    }

    public override EventInfo[] GetEvents()
    {
        throw new NotImplementedException();
    }

    public override EventInfo[] GetEvents(BindingFlags bindingAttr)
    {
        throw new NotImplementedException();
    }

    public override FieldInfo[] GetFields(BindingFlags bindingAttr)
    {
        throw new NotImplementedException();
    }

    public override Type[] GetGenericArguments()
    {
        throw new NotImplementedException();
    }

    public override Type GetGenericTypeDefinition()
    {
        throw new NotImplementedException();
    }

    public override Type? GetInterface(string name, bool ignoreCase)
    {
        throw new NotImplementedException();
    }

    public override Type[] GetInterfaces()
    {
        throw new NotImplementedException();
    }

    public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
    {
        throw new NotImplementedException();
    }

    public override MethodInfo[] GetMethods(BindingFlags bindingAttr)
    {
        throw new NotImplementedException();
    }

    public override Type? GetNestedType(string name, BindingFlags bindingAttr)
    {
        throw new NotImplementedException();
    }

    public override Type[] GetNestedTypes(BindingFlags bindingAttr)
    {
        throw new NotImplementedException();
    }

    public override PropertyInfo[] GetProperties(BindingFlags bindingAttr)
    {
        throw new NotImplementedException();
    }

    protected override bool HasElementTypeImpl()
    {
        throw new NotImplementedException();
    }

    public override object? InvokeMember(string name, BindingFlags invokeAttr, Binder? binder, object? target, object?[]? args, ParameterModifier[]? modifiers, CultureInfo? culture, string[]? namedParameters)
    {
        throw new NotImplementedException();
    }

    protected override bool IsByRefImpl()
    {
        throw new NotImplementedException();
    }

    protected override bool IsCOMObjectImpl()
    {
        throw new NotImplementedException();
    }

    public bool IsCreated()
    {
        throw new NotImplementedException();
    }

    public override bool IsDefined(Type attributeType, bool inherit)
    {
        throw new NotImplementedException();
    }

    protected override bool IsPointerImpl()
    {
        throw new NotImplementedException();
    }

    protected override bool IsPrimitiveImpl()
    {
        throw new NotImplementedException();
    }

    public override bool IsSubclassOf(Type c)
    {
        throw new NotImplementedException();
    }

    public override Type MakeArrayType()
    {
        throw new NotImplementedException();
    }

    public override Type MakeArrayType(int rank)
    {
        throw new NotImplementedException();
    }

    public override Type MakeByRefType()
    {
        throw new NotImplementedException();
    }

    public override Type MakeGenericType(params Type[] typeArguments)
    {
        throw new NotImplementedException();
    }

    public override Type MakePointerType()
    {
        throw new NotImplementedException();
    }

    public override string ToString()
    {
        throw new NotImplementedException();
    }
}
