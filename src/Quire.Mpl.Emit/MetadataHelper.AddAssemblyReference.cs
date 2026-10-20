#nullable enable
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace System.Reflection.Emit.Quire.Mpl;

internal static partial class MetadataHelper
{
    internal static AssemblyReferenceHandle AddAssemblyReference(Assembly assembly, MetadataBuilder metadata)
    {
        AssemblyName name = assembly.GetName();
        if (name == null || name.Name == null)
        {
            throw new ArgumentNullException("assemblyName");
        }
        return AddAssemblyReference(metadata, name.Name, name.Version, name.CultureName, name.GetPublicKey(), (AssemblyFlags)name.Flags);
    }

    internal static AssemblyReferenceHandle AddAssemblyReference(MetadataBuilder metadata, string name, Version? version, string? culture, byte[]? publicKey, AssemblyFlags flags)
    {
        MetadataReferenceTracker metadataReferenceTracker = MetadataReferenceTracker.GetInstance() ?? throw new ArgumentNullException("Tracker must be not null");
        AssemblyName assemblyName = new AssemblyName(name);
        assemblyName.Version = version;
        assemblyName.CultureName = culture;
        assemblyName.SetPublicKey(publicKey);
        assemblyName.Flags = (AssemblyNameFlags)flags;
        return metadataReferenceTracker.GetOrAddAssemblyReference(assemblyName);
    }
}
