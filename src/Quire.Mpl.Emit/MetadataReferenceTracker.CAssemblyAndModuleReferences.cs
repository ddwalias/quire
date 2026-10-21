using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace System.Reflection.Emit.Quire.Mpl;

public partial class MetadataReferenceTracker
{
    public AssemblyReferenceHandle GetOrAddAssemblyReference(AssemblyName assemblyName)
    {
        EntityWrappers.AssemblyNameReferenceWrapper key = new EntityWrappers.AssemblyNameReferenceWrapper(assemblyName);
        if (_assemblyRefs.TryGetValue(key, out var value))
        {
            return value;
        }
        value = _metadata.AddAssemblyReference((assemblyName.Name != null) ? _metadata.GetOrAddString(assemblyName.Name) : default, assemblyName.Version ?? new Version(1, 0, 0, 0), (assemblyName.CultureName != null) ? _metadata.GetOrAddString(assemblyName.CultureName) : default, (assemblyName.GetPublicKeyToken() != null) ? _metadata.GetOrAddBlob(assemblyName.GetPublicKeyToken() ?? new byte[8]) : default, (AssemblyFlags)0, default);
        _assemblyRefs[key] = value;
        return value;
    }

    public ModuleReferenceHandle GetOrAddModuleReference(Module module)
    {
        if (_moduleRefs.TryGetValue(module, out var value))
        {
            return value;
        }
        value = _metadata.AddModuleReference(_metadata.GetOrAddString(module.Name));
        _moduleRefs[module] = value;
        return value;
    }
}
