using System;
using System.Reflection;

namespace System.Reflection.Emit.Quire.Mpl;

public partial class TypeBuilder
{
    public static ConstructorInfo GetConstructor(Type type, ConstructorInfo constructor)
    {
        throw new NotImplementedException();
    }

    protected override ConstructorInfo? GetConstructorImpl(BindingFlags bindingAttr, Binder? binder, CallingConventions callConvention, Type[] types, ParameterModifier[]? modifiers)
    {
        throw new NotImplementedException();
    }

    public override FieldInfo? GetField(string name, BindingFlags bindingAttr)
    {
        foreach (FieldBuilder item in _fieldDefStore)
        {
            if (string.Equals(item.Name, name, bindingAttr.HasFlag(BindingFlags.IgnoreCase) ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal))
            {
                bool isStatic = item.IsStatic;
                bool isPublic = item.IsPublic;
                bool isPrivate = item.IsPrivate;
                bool isFamily = item.IsFamily;
                bool isAssembly = item.IsAssembly;
                bool isFamilyOrAssembly = item.IsFamilyOrAssembly;
                bool visibilityMatched = false;
                if (isPublic && bindingAttr.HasFlag(BindingFlags.Public))
                {
                    visibilityMatched = true;
                }
                if ((isPrivate | isFamily | isAssembly | isFamilyOrAssembly) && bindingAttr.HasFlag(BindingFlags.NonPublic))
                {
                    visibilityMatched = true;
                }
                bool kindMatched = false;
                if (isStatic && bindingAttr.HasFlag(BindingFlags.Static))
                {
                    kindMatched = true;
                }
                if (!isStatic && bindingAttr.HasFlag(BindingFlags.Instance))
                {
                    kindMatched = true;
                }
                if (visibilityMatched & kindMatched)
                {
                    return item;
                }
            }
        }
        return null;
    }

    public static FieldInfo GetField(Type type, FieldInfo field)
    {
        throw new NotImplementedException();
    }

    public override MemberInfo[] GetMember(string name, MemberTypes type, BindingFlags bindingAttr)
    {
        throw new NotImplementedException();
    }

    public static MethodInfo GetMethod(Type type, MethodInfo method)
    {
        throw new NotImplementedException();
    }

    protected override MethodInfo? GetMethodImpl(string name, BindingFlags bindingAttr, Binder? binder, CallingConventions callConvention, Type[]? types, ParameterModifier[]? modifiers)
    {
        throw new NotImplementedException();
    }

    protected override PropertyInfo? GetPropertyImpl(string name, BindingFlags bindingAttr, Binder? binder, Type? returnType, Type[]? types, ParameterModifier[]? modifiers)
    {
        throw new NotImplementedException();
    }

    protected override bool IsArrayImpl()
    {
        return false;
    }

    public override bool IsAssignableFrom(TypeInfo? typeInfo)
    {
        throw new NotImplementedException();
    }

    public override bool IsAssignableFrom(Type? c)
    {
        throw new NotImplementedException();
    }
}
