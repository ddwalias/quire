using System.Globalization;
using System.IO;
using System.Reflection;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;

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

    public void Save(string assemblyFileName)
    {
        if (_previouslySaved)
        {
            throw new InvalidOperationException("Cannot save an assembly multiple times");
        }
        if (assemblyFileName == null)
        {
            throw new ArgumentNullException("assemblyFileName");
        }
        if (_assemblyName == null || _assemblyName.Name == null)
        {
            throw new ArgumentNullException("_assemblyName");
        }
        if (_module == null)
        {
            throw new InvalidOperationException("Assembly needs at least one module defined");
        }
        MetadataBuilder metadataBuilder = new MetadataBuilder();
        BlobBuilder blobBuilder = new BlobBuilder();
        MethodBodyStreamEncoder methodBodyStreamEncoder = new MethodBodyStreamEncoder(blobBuilder);
        _assemblyDef = metadataBuilder.AddAssembly(metadataBuilder.GetOrAddString(_assemblyName.Name), _assemblyName.Version ?? new Version(0, 0, 0, 0), (_assemblyName.CultureName == null) ? default(StringHandle) : metadataBuilder.GetOrAddString(_assemblyName.CultureName), _assemblyName.GetPublicKey() is { } publicKey ? metadataBuilder.GetOrAddBlob(publicKey) : default(BlobHandle), AssemblyFlags.PublicKey, AssemblyHashAlgorithm.None);
        _module.AppendMetadata(metadataBuilder, methodBodyStreamEncoder);
        using FileStream peStream = new FileStream(assemblyFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        WritePEImage(peStream, metadataBuilder, blobBuilder);
        _previouslySaved = true;
    }

    public static AssemblyBuilder DefineDynamicAssembly(AssemblyName name, AssemblyBuilderAccess access)
    {
        if (name == null || name.Name == null)
        {
            throw new ArgumentNullException();
        }
        return new AssemblyBuilder(name);
    }

    [RequiresDynamicCode("Defining a dynamic assembly requires dynamic code.")]
    public static System.Reflection.Emit.AssemblyBuilder DefineDynamicAssembly(AssemblyName name, AssemblyBuilderAccess access, IEnumerable<System.Reflection.Emit.CustomAttributeBuilder>? assemblyAttributes)
    {
        throw new NotImplementedException();
    }

    private static void WritePEImage(Stream peStream, MetadataBuilder metadataBuilder, BlobBuilder ilBuilder)
    {
        ManagedPEBuilder managedPEBuilder = new ManagedPEBuilder(new PEHeaderBuilder(Machine.Unknown, 8192, 512, 4194304uL, 48, 0, 4, 0, 0, 0, 4, 0, Subsystem.WindowsCui, DllCharacteristics.DynamicBase | DllCharacteristics.NxCompatible | DllCharacteristics.NoSeh | DllCharacteristics.TerminalServerAware, Characteristics.ExecutableImage | Characteristics.Dll, 1048576uL, 4096uL, 1048576uL, 4096uL), new MetadataRootBuilder(metadataBuilder), ilBuilder, null, null, null, null, 128, default, CorFlags.ILOnly | CorFlags.StrongNameSigned);
        BlobBuilder blobBuilder = new BlobBuilder();
        managedPEBuilder.Serialize(blobBuilder);
        blobBuilder.WriteContentTo(peStream);
    }

}
