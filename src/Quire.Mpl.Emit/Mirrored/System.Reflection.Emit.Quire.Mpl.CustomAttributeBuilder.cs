#nullable disable
#pragma warning disable

namespace System.Reflection.Emit.Quire.Mpl;

partial class CustomAttributeBuilder
{
    private bool ValidateType(global::System.Type t)
    {
        if (t.IsPrimitive)
        {
            if (t != typeof(nint))
            {
                return t != typeof(nuint);
            }
            return false;
        }
        if (t == typeof(string) || t == typeof(global::System.Type))
        {
            return true;
        }
        if (t.IsEnum)
        {
            if (global::System.Type.GetTypeCode(global::System.Enum.GetUnderlyingType(t)) is var typeCode && (uint)(typeCode - 5) <= 7u)
            {
                return true;
            }
            return false;
        }
        if (t.IsArray)
        {
            if (t.GetArrayRank() == 1)
            {
                return ValidateType(t.GetElementType());
            }
            return false;
        }
        return t == typeof(object);
    }
}
