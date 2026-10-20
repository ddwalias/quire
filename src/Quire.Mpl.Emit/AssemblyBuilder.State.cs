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

    public ModuleBuilder DefineDynamicModule(string name)
    {
        if (name == null)
        {
            throw new ArgumentNullException("name");
        }
        if (name.Length == 0)
        {
            throw new ArgumentException("name");
        }
        if (_module != null)
        {
            throw new InvalidOperationException("Multi-module assemblies are not supported");
        }
        ModuleBuilder moduleBuilder = new ModuleBuilder(name, this);
        _module = moduleBuilder;
        return moduleBuilder;
    }

    public ModuleBuilder? GetDynamicModule(string name)
    {
        if (name == null)
        {
            throw new ArgumentNullException("name");
        }
        if (name.Length == 0)
        {
            throw new ArgumentException("name cannot have zero characters.");
        }
        if (_module == null)
        {
            return null;
        }
        if (_module.Name.Equals(name))
        {
            return _module;
        }
        return null;
    }
}
