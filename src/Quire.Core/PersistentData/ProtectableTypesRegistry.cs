#nullable disable
using System;
using System.Xml.Serialization;
using Quire.Mirror;

namespace Quire.PersistentData;

public static partial class ProtectableTypesRegistry
{
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
