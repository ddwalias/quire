using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace System.Reflection.Emit.Quire.Mpl;

public partial class TypeBuilder
{
    public System.Reflection.Emit.ConstructorBuilder DefineConstructor(MethodAttributes attributes, CallingConventions callingConvention, Type[]? parameterTypes, Type[][]? requiredCustomModifiers, Type[][]? optionalCustomModifiers)
    {
        throw new NotImplementedException();
    }

    public System.Reflection.Emit.FieldBuilder DefineField(string fieldName, Type type, Type[]? requiredCustomModifiers, Type[]? optionalCustomModifiers, FieldAttributes attributes)
    {
        throw new NotImplementedException();
    }

    public System.Reflection.Emit.MethodBuilder DefineMethod(string name, MethodAttributes attributes, CallingConventions callingConvention)
    {
        throw new NotImplementedException();
    }

    public System.Reflection.Emit.MethodBuilder DefineMethod(string name, MethodAttributes attributes, CallingConventions callingConvention, Type? returnType, Type[]? returnTypeRequiredCustomModifiers, Type[]? returnTypeOptionalCustomModifiers, Type[]? parameterTypes, Type[][]? parameterTypeRequiredCustomModifiers, Type[][]? parameterTypeOptionalCustomModifiers)
    {
        throw new NotImplementedException();
    }

    public System.Reflection.Emit.TypeBuilder DefineNestedType(string name)
    {
        throw new NotImplementedException();
    }

    public System.Reflection.Emit.TypeBuilder DefineNestedType(string name, TypeAttributes attr)
    {
        throw new NotImplementedException();
    }

    public System.Reflection.Emit.TypeBuilder DefineNestedType(string name, TypeAttributes attr, Type? parent)
    {
        throw new NotImplementedException();
    }

    public System.Reflection.Emit.TypeBuilder DefineNestedType(string name, TypeAttributes attr, Type? parent, int typeSize)
    {
        throw new NotImplementedException();
    }

    public System.Reflection.Emit.TypeBuilder DefineNestedType(string name, TypeAttributes attr, Type? parent, PackingSize packSize)
    {
        throw new NotImplementedException();
    }

    public System.Reflection.Emit.TypeBuilder DefineNestedType(string name, TypeAttributes attr, Type? parent, PackingSize packSize, int typeSize)
    {
        throw new NotImplementedException();
    }

    public System.Reflection.Emit.TypeBuilder DefineNestedType(string name, TypeAttributes attr, Type? parent, Type[]? interfaces)
    {
        throw new NotImplementedException();
    }

    public System.Reflection.Emit.MethodBuilder DefinePInvokeMethod(string name, string dllName, MethodAttributes attributes, CallingConventions callingConvention, Type? returnType, Type[]? parameterTypes, CallingConvention nativeCallConv, CharSet nativeCharSet)
    {
        throw new NotImplementedException();
    }

    public System.Reflection.Emit.MethodBuilder DefinePInvokeMethod(string name, string dllName, string entryName, MethodAttributes attributes, CallingConventions callingConvention, Type? returnType, Type[]? parameterTypes, CallingConvention nativeCallConv, CharSet nativeCharSet)
    {
        throw new NotImplementedException();
    }

    public System.Reflection.Emit.MethodBuilder DefinePInvokeMethod(string name, string dllName, string entryName, MethodAttributes attributes, CallingConventions callingConvention, Type? returnType, Type[]? returnTypeRequiredCustomModifiers, Type[]? returnTypeOptionalCustomModifiers, Type[]? parameterTypes, Type[][]? parameterTypeRequiredCustomModifiers, Type[][]? parameterTypeOptionalCustomModifiers, CallingConvention nativeCallConv, CharSet nativeCharSet)
    {
        throw new NotImplementedException();
    }

    public PropertyBuilder DefineProperty(string name, PropertyAttributes attributes, CallingConventions callingConvention, Type returnType, Type[]? parameterTypes)
    {
        throw new NotImplementedException();
    }

    public PropertyBuilder DefineProperty(string name, PropertyAttributes attributes, CallingConventions callingConvention, Type returnType, Type[]? returnTypeRequiredCustomModifiers, Type[]? returnTypeOptionalCustomModifiers, Type[]? parameterTypes, Type[][]? parameterTypeRequiredCustomModifiers, Type[][]? parameterTypeOptionalCustomModifiers)
    {
        throw new NotImplementedException();
    }

    public PropertyBuilder DefineProperty(string name, PropertyAttributes attributes, Type returnType, Type[]? parameterTypes)
    {
        throw new NotImplementedException();
    }

    public PropertyBuilder DefineProperty(string name, PropertyAttributes attributes, Type returnType, Type[]? returnTypeRequiredCustomModifiers, Type[]? returnTypeOptionalCustomModifiers, Type[]? parameterTypes, Type[][]? parameterTypeRequiredCustomModifiers, Type[][]? parameterTypeOptionalCustomModifiers)
    {
        throw new NotImplementedException();
    }
}
