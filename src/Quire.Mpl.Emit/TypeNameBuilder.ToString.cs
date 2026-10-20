using System;

namespace System.Reflection.Emit.Quire.Mpl;

internal sealed partial class TypeNameBuilder
{
    internal static string ToString(Type type, Format format)
    {
        if ((format == Format.FullName || format == Format.AssemblyQualifiedName) && !type.IsGenericTypeDefinition && type.ContainsGenericParameters)
        {
            return null;
        }
        TypeNameBuilder typeNameBuilder = new TypeNameBuilder();
        typeNameBuilder.AddAssemblyQualifiedName(type, format);
        return typeNameBuilder.ToString();
    }
}
