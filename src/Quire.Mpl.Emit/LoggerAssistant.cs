using System.Reflection;

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

    public static string GetFullFieldName(FieldInfo field)
    {
        return $"{field.DeclaringType?.FullName}.{field.Name} : {field.FieldType.Name}";
    }

}
