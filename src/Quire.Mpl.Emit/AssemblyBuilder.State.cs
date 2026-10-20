using System.Globalization;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace System.Reflection.Emit.Quire.Mpl;

public partial class AssemblyBuilder
{
    public string Name => _assemblyName.Name ?? string.Empty;

    public override string FullName => _fullName;

    public Version Version => _version;

    public string Culture => _culture;

    private AssemblyBuilder(AssemblyName name)
    {
        _assemblyName = name;
        _assemblyName.CultureInfo = CultureInfo.InvariantCulture;
        _assemblyName.Version = _assemblyName.Version ?? new Version(0, 0, 0, 0);
        _assemblyName.SetPublicKey(null);
        _fullName = _assemblyName.FullName;
        _version = _assemblyName.Version;
        _culture = _assemblyName.CultureName ?? string.Empty;
    }

    public override AssemblyName GetName()
    {
        return _assemblyName;
    }
}
