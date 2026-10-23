using System.Reflection;
using System.Runtime.CompilerServices;

namespace System.Reflection.Emit.Quire.Mpl;

public static partial class LoggerAssistant
{
    public static string GetFullName(MemberInfo member)
    {
        Type declaringType = member.DeclaringType;
        string value = declaringType?.Assembly.GetName().Name;
        string declaringTypeName = declaringType?.FullName;
        return $"{value}:{declaringTypeName}.{member.Name}";
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
        string declaringTypeName = member.DeclaringType?.FullName;
        // Up-front declarations: the Release IL differs when each formatter moves into its branch.
        MethodInfo method;
        DefaultInterpolatedStringHandler methodFormatter;
        ConstructorInfo ctor;
        DefaultInterpolatedStringHandler ctorFormatter;
        FieldInfo fieldInfo;
        DefaultInterpolatedStringHandler fieldFormatter;
        DefaultInterpolatedStringHandler memberFormatter;

        method = member as MethodInfo;
        if (method is not null)
        {
            methodFormatter = new DefaultInterpolatedStringHandler(3, 3);
            methodFormatter.AppendLiteral("[");
            methodFormatter.AppendFormatted(value);
            methodFormatter.AppendLiteral("]");
            methodFormatter.AppendFormatted(declaringTypeName);
            methodFormatter.AppendLiteral(".");
            methodFormatter.AppendFormatted(GetFullMethodName(method));
            return methodFormatter.ToStringAndClear();
        }
        ctor = member as ConstructorInfo;
        if (ctor is not null)
        {
            ctorFormatter = new DefaultInterpolatedStringHandler(3, 3);
            ctorFormatter.AppendLiteral("[");
            ctorFormatter.AppendFormatted(value);
            ctorFormatter.AppendLiteral("]");
            ctorFormatter.AppendFormatted(declaringTypeName);
            ctorFormatter.AppendLiteral(".");
            ctorFormatter.AppendFormatted(GetFullConstructorName(ctor));
            return ctorFormatter.ToStringAndClear();
        }
        fieldInfo = member as FieldInfo;
        if (fieldInfo is not null)
        {
            fieldFormatter = new DefaultInterpolatedStringHandler(3, 3);
            fieldFormatter.AppendLiteral("[");
            fieldFormatter.AppendFormatted(value);
            fieldFormatter.AppendLiteral("]");
            fieldFormatter.AppendFormatted(declaringTypeName);
            fieldFormatter.AppendLiteral(".");
            fieldFormatter.AppendFormatted(fieldInfo.Name);
            return fieldFormatter.ToStringAndClear();
        }
        memberFormatter = new DefaultInterpolatedStringHandler(4, 4);
        memberFormatter.AppendLiteral("[");
        memberFormatter.AppendFormatted(value);
        memberFormatter.AppendLiteral("]");
        memberFormatter.AppendFormatted(declaringTypeName);
        memberFormatter.AppendLiteral(".");
        memberFormatter.AppendFormatted(member.Name);
        memberFormatter.AppendLiteral(".");
        memberFormatter.AppendFormatted(member.MemberType);
        return memberFormatter.ToStringAndClear();
    }

}
