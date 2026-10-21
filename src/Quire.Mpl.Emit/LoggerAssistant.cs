using System.Reflection;
using System.Runtime.CompilerServices;

namespace System.Reflection.Emit.Quire.Mpl;

public static partial class LoggerAssistant
{
    public static string GetFullName(MemberInfo member)
    {
        Type declaringType = member.DeclaringType;
        string value = declaringType?.Assembly.GetName().Name;
        string value2 = declaringType?.FullName;
        return $"{value}:{value2}.{member.Name}";
    }

    public static string GetFullMethodName(MethodInfo method)
    {
        string text = method.Name;
        if (method.IsGenericMethod)
        {
            Type[] genericArguments = method.GetGenericArguments();
            text = text + "<" + string.Join(",", Array.ConvertAll(genericArguments, (Type t) => t.Name)) + ">";
        }
        string fullName = method.ReturnType.FullName;
        ParameterInfo[] parameters = method.GetParameters();
        string value = string.Join(",", Array.ConvertAll(parameters, (ParameterInfo p) => p.ParameterType.Name + " " + p.Name));
        return $"{fullName} {method.DeclaringType?.FullName}.{text}({value})";
    }

    public static string GetFullConstructorName(ConstructorInfo ctor)
    {
        ParameterInfo[] parameters = ctor.GetParameters();
        string text = string.Join(",", Array.ConvertAll(parameters, (ParameterInfo p) => p.ParameterType.Name + " " + p.Name));
        return ctor.DeclaringType?.FullName + ".ctor(" + text + ")";
    }

    public static string GetFullFieldName(FieldInfo field)
    {
        return $"{field.DeclaringType?.FullName}.{field.Name} : {field.FieldType.Name}";
    }

    public static string GetFullNameWithAssembly(MemberInfo member)
    {
        string value = member.DeclaringType?.Assembly?.GetName().Name;
        string value2 = member.DeclaringType?.FullName;
        MethodInfo method;
        DefaultInterpolatedStringHandler handler;
        ConstructorInfo ctor;
        DefaultInterpolatedStringHandler handler2;
        FieldInfo fieldInfo;
        DefaultInterpolatedStringHandler handler3;
        DefaultInterpolatedStringHandler handler4;

        method = member as MethodInfo;
        if (method is not null)
        {
            handler = new DefaultInterpolatedStringHandler(3, 3);
            handler.AppendLiteral("[");
            handler.AppendFormatted(value);
            handler.AppendLiteral("]");
            handler.AppendFormatted(value2);
            handler.AppendLiteral(".");
            handler.AppendFormatted(GetFullMethodName(method));
            return handler.ToStringAndClear();
        }
        ctor = member as ConstructorInfo;
        if (ctor is not null)
        {
            handler2 = new DefaultInterpolatedStringHandler(3, 3);
            handler2.AppendLiteral("[");
            handler2.AppendFormatted(value);
            handler2.AppendLiteral("]");
            handler2.AppendFormatted(value2);
            handler2.AppendLiteral(".");
            handler2.AppendFormatted(GetFullConstructorName(ctor));
            return handler2.ToStringAndClear();
        }
        fieldInfo = member as FieldInfo;
        if (fieldInfo is not null)
        {
            handler3 = new DefaultInterpolatedStringHandler(3, 3);
            handler3.AppendLiteral("[");
            handler3.AppendFormatted(value);
            handler3.AppendLiteral("]");
            handler3.AppendFormatted(value2);
            handler3.AppendLiteral(".");
            handler3.AppendFormatted(fieldInfo.Name);
            return handler3.ToStringAndClear();
        }
        handler4 = new DefaultInterpolatedStringHandler(4, 4);
        handler4.AppendLiteral("[");
        handler4.AppendFormatted(value);
        handler4.AppendLiteral("]");
        handler4.AppendFormatted(value2);
        handler4.AppendLiteral(".");
        handler4.AppendFormatted(member.Name);
        handler4.AppendLiteral(".");
        handler4.AppendFormatted(member.MemberType);
        return handler4.ToStringAndClear();
    }

}
