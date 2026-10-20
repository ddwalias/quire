using System;
using System.Reflection;

namespace System.Reflection.Emit.Quire.Mpl;

public partial class CustomAttributeBuilder
{
    public ConstructorInfo Constructor => _constructorInfo;

    public CustomAttributeBuilder(ConstructorInfo con, object?[] constructorArgs)
        : this(con, constructorArgs, Array.Empty<PropertyInfo>(), Array.Empty<object>(), Array.Empty<FieldInfo>(), Array.Empty<object>())
    {
    }

    public CustomAttributeBuilder(ConstructorInfo con, object?[] constructorArgs, PropertyInfo[] namedProperties, object?[] propertyValues)
        : this(con, constructorArgs, namedProperties, propertyValues, Array.Empty<FieldInfo>(), Array.Empty<object>())
    {
    }

    public CustomAttributeBuilder(ConstructorInfo con, object?[] constructorArgs, FieldInfo[] namedFields, object?[] fieldValues)
        : this(con, constructorArgs, Array.Empty<PropertyInfo>(), Array.Empty<object>(), namedFields, fieldValues)
    {
    }
}
