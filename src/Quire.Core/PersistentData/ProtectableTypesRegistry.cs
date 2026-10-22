#nullable disable
using System;
using System.Linq;
using System.Xml.Serialization;
using Quire.Mirror;

namespace Quire.PersistentData;

public static partial class ProtectableTypesRegistry
{
    [MirrorVerified("Release IL")]
    private static void Register(string rootElementName, Type typeToProtect)
    {
        if (DebugTools.DevAssert(typeof(IProtectableDataType).IsAssignableFrom(typeToProtect) && typeToProtect.IsClass, null, Array.Empty<object>()).Failed)
        {
            return;
        }
        if (DebugTools.DevAssert(_map.ContainsKey(rootElementName) ? false : !_map.Values.Contains(typeToProtect), null, Array.Empty<object>()).Failed)
        {
            return;
        }
        _map.Add(rootElementName, typeToProtect);
    }

    [MirrorVerified("Release IL")]
    public static XmlSerializer GetXmlSerializer(string rootElementName, bool useOverrides)
    {
        if (DebugTools.DevAssert(_map.ContainsKey(rootElementName), null, Array.Empty<object>()).Failed)
        {
            return null;
        }
        return ((IProtectableDataType)Activator.CreateInstance(_map[rootElementName])).GetXmlSerializer(useOverrides);
    }
}
