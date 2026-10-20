using System.Globalization;

namespace System.Reflection.Emit.Quire.Mpl;

internal static partial class AssemblyNameResolver
{
    private static readonly string[] _systemRuntimeTypes = new string[23]
    {
        "Object", "String", "ValueType", "Enum", "Delegate", "Array", "Void", "Boolean", "Char", "SByte",
        "Byte", "Int16", "UInt16", "Int32", "UInt32", "Int64", "UInt64", "IntPtr", "UIntPtr", "Single",
        "Double", "Type", "TargetFrameworkAttribute"
    };

    public static AssemblyName GetResolvedAssemblyName(Type type)
    {
        if (type == null)
        {
            throw new ArgumentNullException("type");
        }
        AssemblyName name = type.Assembly.GetName();
        if (name.Name == "System.Private.CoreLib" && IsSystemNamespace(type.Namespace) && _systemRuntimeTypes.Contains(type.Name))
        {
            return new AssemblyName
            {
                Name = "System.Runtime",
                Version = new Version(10, 0, 0, 0),
                CultureInfo = CultureInfo.InvariantCulture
            }.WithPublicKeyToken("b03f5f7f11d50a3a");
        }
        return name;
    }

    public static AssemblyName WithPublicKeyToken(this AssemblyName assemblyName, string publicKeyToken)
    {
        assemblyName.SetPublicKeyToken(HexStringToByteArray(publicKeyToken));
        return assemblyName;
    }

    private static byte[] HexStringToByteArray(string hex)
    {
        byte[] array = new byte[hex.Length / 2];
        for (int i = 0; i < hex.Length; i += 2)
        {
            array[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
        }
        return array;
    }

    private static bool IsSystemNamespace(string @namespace)
    {
        if (@namespace != null)
        {
            if (!(@namespace == "System"))
            {
                return @namespace.StartsWith("System.");
            }
            return true;
        }
        return false;
    }
}
