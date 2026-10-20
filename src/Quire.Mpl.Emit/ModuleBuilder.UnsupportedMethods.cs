using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.InteropServices;

namespace System.Reflection.Emit.Quire.Mpl;

public partial class ModuleBuilder
{
    public void CreateGlobalFunctions() => throw new NotImplementedException();

    public System.Reflection.Emit.EnumBuilder DefineEnum(string name, TypeAttributes visibility, Type underlyingType) => throw new NotImplementedException();

    public System.Reflection.Emit.MethodBuilder DefineGlobalMethod(string name, MethodAttributes attributes, CallingConventions callingConvention, Type? returnType, Type[]? parameterTypes) => throw new NotImplementedException();

    public System.Reflection.Emit.MethodBuilder DefineGlobalMethod(string name, MethodAttributes attributes, CallingConventions callingConvention, Type? returnType, Type[]? requiredReturnTypeCustomModifiers, Type[]? optionalReturnTypeCustomModifiers, Type[]? parameterTypes, Type[][]? requiredParameterTypeCustomModifiers, Type[][]? optionalParameterTypeCustomModifiers) => throw new NotImplementedException();

    public System.Reflection.Emit.MethodBuilder DefineGlobalMethod(string name, MethodAttributes attributes, Type? returnType, Type[]? parameterTypes) => throw new NotImplementedException();

    public System.Reflection.Emit.FieldBuilder DefineInitializedData(string name, byte[] data, FieldAttributes attributes) => throw new NotImplementedException();

    [RequiresUnreferencedCode("P/Invoke marshalling may dynamically access members that could be trimmed.")]
    public System.Reflection.Emit.MethodBuilder DefinePInvokeMethod(string name, string dllName, MethodAttributes attributes, CallingConventions callingConvention, Type? returnType, Type[]? parameterTypes, CallingConvention nativeCallConv, CharSet nativeCharSet) => throw new NotImplementedException();

    [RequiresUnreferencedCode("P/Invoke marshalling may dynamically access members that could be trimmed.")]
    public System.Reflection.Emit.MethodBuilder DefinePInvokeMethod(string name, string dllName, string entryName, MethodAttributes attributes, CallingConventions callingConvention, Type? returnType, Type[]? parameterTypes, CallingConvention nativeCallConv, CharSet nativeCharSet) => throw new NotImplementedException();

    public System.Reflection.Emit.TypeBuilder DefineType(string name) => throw new NotImplementedException();

    public System.Reflection.Emit.TypeBuilder DefineType(string name, TypeAttributes attr, Type? parent, int typesize) => throw new NotImplementedException();

    public System.Reflection.Emit.TypeBuilder DefineType(string name, TypeAttributes attr, Type? parent, PackingSize packsize) => throw new NotImplementedException();

    public System.Reflection.Emit.TypeBuilder DefineType(string name, TypeAttributes attr, Type? parent, PackingSize packingSize, int typesize) => throw new NotImplementedException();

    public System.Reflection.Emit.TypeBuilder DefineType(string name, TypeAttributes attr, Type? parent, Type[]? interfaces) => throw new NotImplementedException();

    public System.Reflection.Emit.FieldBuilder DefineUninitializedData(string name, int size, FieldAttributes attributes) => throw new NotImplementedException();

    public MethodInfo GetArrayMethod(Type arrayClass, string methodName, CallingConventions callingConvention, Type? returnType, Type[]? parameterTypes) => throw new NotImplementedException();

    public void SetCustomAttribute(ConstructorInfo con, byte[] binaryAttribute) => throw new NotImplementedException();

    public void SetCustomAttribute(System.Reflection.Emit.CustomAttributeBuilder customBuilder) => throw new NotImplementedException();
}
