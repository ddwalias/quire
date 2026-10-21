using System;
using System.Reflection;

namespace System.Reflection.Emit.Quire.Mpl;

public partial class CustomAttributeBuilder
{
    private static void VerifyTypeAndPassedObjectType(Type type, Type passedType, string paramName)
    {
        if (type != typeof(object) && Type.GetTypeCode(passedType) != Type.GetTypeCode(type))
        {
            throw new ArgumentException("Constant Doesnt Match");
        }
        if (passedType == typeof(nint) || passedType == typeof(nuint))
        {
            throw new ArgumentException("Bad arugment for custom attribute builder");
        }
    }
}
