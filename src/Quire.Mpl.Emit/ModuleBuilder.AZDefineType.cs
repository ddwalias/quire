using System;
using System.Reflection;

namespace System.Reflection.Emit.Quire.Mpl;

public partial class ModuleBuilder
{
    public TypeBuilder DefineType(string name, TypeAttributes attr)
    {
        TypeBuilder typeBuilder = new TypeBuilder(name, this, Assembly, attr, typeof(object));
        _typeDefStore.Add(typeBuilder);
        return typeBuilder;
    }
}
