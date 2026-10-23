using System;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;

namespace System.Reflection.Emit.Quire.Mpl;

public partial class AssemblyBuilder
{
    public byte[] EmitAssembly()
    {
        if (_previouslySaved)
        {
            throw new InvalidOperationException("Cannot save an assembly multiple times");
        }
        if (_assemblyName == null || _assemblyName.Name == null)
        {
            throw new ArgumentNullException("_assemblyName");
        }
        if (_module == null)
        {
            throw new InvalidOperationException("Assembly needs at least one module defined");
        }
        MetadataBuilder metadataBuilder = new MetadataBuilder(0, 0, 0, 0);
        BlobBuilder blobBuilder = new BlobBuilder(256);
        MethodBodyStreamEncoder methodBodyStreamEncoder = new MethodBodyStreamEncoder(blobBuilder);
        _assemblyDef = metadataBuilder.AddAssembly(metadataBuilder.GetOrAddString(_assemblyName.Name), _assemblyName.Version ?? new Version(0, 0, 0, 0), (_assemblyName.CultureName == null) ? default(StringHandle) : metadataBuilder.GetOrAddString(_assemblyName.CultureName), (_assemblyName.GetPublicKey() is { } publicKey) ? metadataBuilder.GetOrAddBlob(publicKey) : default(BlobHandle), AssemblyFlags.PublicKey, AssemblyHashAlgorithm.None);
        _module.AppendMetadata(metadataBuilder, methodBodyStreamEncoder);
        ManagedPEBuilder managedPEBuilder = new ManagedPEBuilder(new PEHeaderBuilder(Machine.Unknown, 8192, 512, 4194304uL, 48, 0, 4, 0, 0, 0, 4, 0, Subsystem.WindowsCui, DllCharacteristics.DynamicBase | DllCharacteristics.NxCompatible | DllCharacteristics.NoSeh | DllCharacteristics.TerminalServerAware, Characteristics.ExecutableImage | Characteristics.Dll, 1048576uL, 4096uL, 1048576uL, 4096uL), new MetadataRootBuilder(metadataBuilder), blobBuilder, null, null, null, null, 128, default, CorFlags.ILOnly | CorFlags.StrongNameSigned);
        BlobBuilder serializedBlob = new BlobBuilder(256);
        managedPEBuilder.Serialize(serializedBlob);
        _previouslySaved = true;
        return serializedBlob.ToArray();
    }
}
