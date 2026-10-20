namespace System.Reflection.Emit.Quire.Mpl;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

public partial class ModuleBuilder
{
    internal ModuleBuilder(string name, AssemblyBuilder assemblyBuilder)
    {
        ScopeName = name;
        Assembly = assemblyBuilder;
    }

    public TypeBuilder DefineType(string name, TypeAttributes attr)
    {
        TypeBuilder typeBuilder = new TypeBuilder(name, this, Assembly, attr, typeof(object));
        _typeDefStore.Add(typeBuilder);
        return typeBuilder;
    }

    public TypeBuilder DefineType(string name, TypeAttributes attr, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type? parent)
    {
        TypeBuilder typeBuilder = new TypeBuilder(name, this, Assembly, attr, parent ?? typeof(object));
        _typeDefStore.Add(typeBuilder);
        return typeBuilder;
    }
}
