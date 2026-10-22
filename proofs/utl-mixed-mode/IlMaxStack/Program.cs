using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;

if (args.Length != 2)
    throw new ArgumentException("usage: IlMaxStack <candidate.dll> <Mathcad-install-dir>");

var candidatePath = Path.GetFullPath(args[0]);
var installPath = Path.GetFullPath(args[1]);
var candidateBytes = File.ReadAllBytes(candidatePath);
var reserved = IlSectionWriter.Reserve(candidateBytes, 1024 * 1024);
if (reserved.Writer.AlreadyPatched)
{
    Console.WriteLine("skip IL post-link: candidate already contains .ilnew");
    return;
}
candidateBytes = reserved.Bytes;
var ilSection = reserved.Writer;
var helperNameEdits = NormalizeInlineHelperNames(candidateBytes);
using (var metadataProbeStream = new MemoryStream(candidateBytes, writable: false))
using (var metadataProbePe = new PEReader(metadataProbeStream))
{
    var metadataProbeReader = metadataProbePe.GetMetadataReader();
    var signatures = new[]
    {
        ("mcmisc", "Engine.Misc.FileUtils", "ReadExpanded", 2),
        ("utl-mgd", "utl_mgd.Logging", "Log", 2),
        ("utl-mgd", "utl_mgd.Logging", "LogLine", 2)
    }
        .Select(item => BodyTransplant.MappedLocalSignature(installPath, item.Item1,
            item.Item2, item.Item3, item.Item4, metadataProbeReader))
        .Where(signature => signature is not null && !BodyTransplant.HasStandaloneSignature(metadataProbeReader, signature))
        .Select(signature => signature!)
        .ToArray();
    if (signatures.Length > 0)
    {
        var metadataRoot = MetadataRootEditor.AppendStandaloneSignatures(
            candidateBytes, metadataProbePe.PEHeaders, signatures);
        var metadataRva = ilSection.PlaceMetadataRoot(candidateBytes, metadataRoot);
        var cliDirectory = metadataProbePe.PEHeaders.PEHeader!.CorHeaderTableDirectory;
        var cliOffset = RvaToOffset(metadataProbePe.PEHeaders, cliDirectory.RelativeVirtualAddress);
        BitConverter.TryWriteBytes(candidateBytes.AsSpan(cliOffset + 8, sizeof(int)), metadataRva);
        BitConverter.TryWriteBytes(candidateBytes.AsSpan(cliOffset + 12, sizeof(int)), metadataRoot.Length);
        helperNameEdits.Add($"added {signatures.Length} exact method StandaloneSig row(s) to relocated metadata");
    }
}
using var candidateStream = new MemoryStream(candidateBytes, writable: false);
using var candidatePe = new PEReader(candidateStream);
var candidateReader = candidatePe.GetMetadataReader();
var edits = new List<(int Offset, int Value, string Key)>();
var skipped = new List<string>();

foreach (var candidateTypeHandle in candidateReader.TypeDefinitions)
{
    var candidateType = candidateReader.GetTypeDefinition(candidateTypeHandle);
    var binding = ReadMirrorOf(candidateReader, candidateType);
    if (binding is null)
        continue;

    var originalAssembly = binding.Value.Assembly + ".dll";
    var originalPath = Path.Combine(installPath, originalAssembly);
    if (!File.Exists(originalPath))
    {
        skipped.Add($"{candidateReader.GetString(candidateType.Name)}: missing {originalAssembly}");
        continue;
    }

    using var originalStream = File.OpenRead(originalPath);
    using var originalPe = new PEReader(originalStream);
    var originalReader = originalPe.GetMetadataReader();
    var originalTypeHandle = FindType(originalReader, binding.Value.Type);
    if (originalTypeHandle.IsNil)
    {
        skipped.Add($"{binding.Value.Type}: missing in {originalAssembly}");
        continue;
    }

    var originalType = originalReader.GetTypeDefinition(originalTypeHandle);
    foreach (var candidateMethodHandle in candidateType.GetMethods())
    {
        var candidateMethod = candidateReader.GetMethodDefinition(candidateMethodHandle);
        var name = candidateReader.GetString(candidateMethod.Name);
        var parameterCount = candidateMethod.DecodeSignature(NameProvider.Instance, null).ParameterTypes.Length;
        var originals = originalType.GetMethods()
            .Select(handle => (Handle: handle, Method: originalReader.GetMethodDefinition(handle)))
            .Where(item => originalReader.GetString(item.Method.Name) == name
                && item.Method.DecodeSignature(NameProvider.Instance, null).ParameterTypes.Length == parameterCount)
            .ToArray();
        if (originals.Length != 1)
            continue;

        var originalMethod = originals[0].Method;
        if (candidateMethod.RelativeVirtualAddress == 0 || originalMethod.RelativeVirtualAddress == 0)
            continue;

        var candidateBody = candidatePe.GetMethodBody(candidateMethod.RelativeVirtualAddress);
        var originalBody = originalPe.GetMethodBody(originalMethod.RelativeVirtualAddress);
        if (candidateBody.MaxStack > originalBody.MaxStack)
        {
            skipped.Add($"{name}/{parameterCount}: candidate maxstack {candidateBody.MaxStack} exceeds original {originalBody.MaxStack}");
            continue;
        }
        if (candidateBody.MaxStack == originalBody.MaxStack)
            continue;

        var candidateOffset = RvaToOffset(candidatePe.PEHeaders, candidateMethod.RelativeVirtualAddress);
        var flagsAndSize = BitConverter.ToUInt16(candidateBytes, candidateOffset);
        if ((flagsAndSize & 0x3) != 0x3)
        {
            skipped.Add($"{name}/{parameterCount}: expected a fat method header");
            continue;
        }

        edits.Add((candidateOffset + 2, originalBody.MaxStack, $"{binding.Value.Type}::{name}/{parameterCount}"));
    }
}

foreach (var edit in edits)
    BitConverter.TryWriteBytes(candidateBytes.AsSpan(edit.Offset, sizeof(ushort)), checked((ushort)edit.Value));
var transplantMessages = BodyTransplant.Apply(candidateBytes, candidatePe, candidateReader, installPath, ilSection);
if (edits.Count > 0 || helperNameEdits.Count > 0)
    File.WriteAllBytes(candidatePath, candidateBytes);
else if (transplantMessages.Any(message => message.StartsWith("transplanted ", StringComparison.Ordinal)))
    File.WriteAllBytes(candidatePath, candidateBytes);

foreach (var edit in edits)
    Console.WriteLine($"maxstack {edit.Key}: {edit.Value}");
foreach (var item in helperNameEdits)
    Console.WriteLine(item);
foreach (var item in skipped)
    Console.WriteLine($"skip {item}");
foreach (var item in transplantMessages)
    Console.WriteLine(item);

static (string Type, string Assembly)? ReadMirrorOf(MetadataReader reader, TypeDefinition type)
{
    foreach (var handle in type.GetCustomAttributes())
    {
        var attribute = reader.GetCustomAttribute(handle);
        var attributeType = GetAttributeType(reader, attribute.Constructor);
        if (attributeType != "Quire.Mirror.MirrorOfAttribute")
            continue;
        var blob = reader.GetBlobReader(attribute.Value);
        if (blob.ReadUInt16() != 1)
            continue;
        var value = blob.ReadSerializedString();
        if (value is null)
            continue;
        var separator = value.LastIndexOf(',');
        if (separator <= 0)
            continue;
        return (value[..separator].Trim(), value[(separator + 1)..].Trim());
    }
    return null;
}

static List<string> NormalizeInlineHelperNames(byte[] image)
{
    using var stream = new MemoryStream(image, writable: false);
    using var pe = new PEReader(stream);
    var reader = pe.GetMetadataReader();
    var helperType = reader.TypeDefinitions
        .Select(handle => (Handle: handle, Type: reader.GetTypeDefinition(handle)))
        .FirstOrDefault(item => reader.GetString(item.Type.Name)
            .StartsWith("<PrivateImplementationDetails><UtlInlineHelpers_netmodule>", StringComparison.Ordinal));
    if (helperType.Type.Name.IsNil)
        return [];

    var stringsOffset = FindStringHeapFileOffset(image, pe.PEHeaders);
    var edits = new List<string>();
    TruncateString(image, stringsOffset, helperType.Type.Name,
        "<PrivateImplementationDetails>".Length, "<PrivateImplementationDetails>", edits);
    foreach (var nestedHandle in helperType.Type.GetNestedTypes())
    {
        var nestedType = reader.GetTypeDefinition(nestedHandle);
        var nestedName = reader.GetString(nestedType.Name);
        const string alignmentSuffix = "_Align=2";
        if (nestedName.EndsWith(alignmentSuffix, StringComparison.Ordinal))
            TruncateString(image, stringsOffset, nestedType.Name,
                nestedName.Length - alignmentSuffix.Length, nestedName[..^alignmentSuffix.Length], edits);
    }
    foreach (var fieldHandle in helperType.Type.GetFields())
    {
        var field = reader.GetFieldDefinition(fieldHandle);
        var fieldName = reader.GetString(field.Name);
        var hashLength = fieldName.StartsWith('F') ? 65 : 64;
        if (fieldName.Length == hashLength + 1 && fieldName[^1] == '2'
            && fieldName[..hashLength].All(character => character == 'F' || Uri.IsHexDigit(character)))
            TruncateString(image, stringsOffset, field.Name, hashLength, fieldName[..hashLength], edits);
    }
    using var verifyStream = new MemoryStream(image, writable: false);
    using var verifyPe = new PEReader(verifyStream);
    var verifyReader = verifyPe.GetMetadataReader();
    var normalized = verifyReader.TypeDefinitions
        .Select(handle => (Handle: handle, Type: verifyReader.GetTypeDefinition(handle)))
        .FirstOrDefault(item => verifyReader.GetString(item.Type.Name) == "<PrivateImplementationDetails>");
    if (normalized.Type.Name.IsNil)
        throw new BadImageFormatException("C# inline helper TypeDef name normalization failed");
    return edits;
}

static void TruncateString(byte[] image, int stringsOffset, StringHandle handle,
    int prefixLength, string expectedPrefix, List<string> edits)
{
    var heapOffset = MetadataTokens.GetHeapOffset(handle);
    var fileOffset = checked(stringsOffset + heapOffset);
    var endOffset = checked(fileOffset + prefixLength);
    var currentEnd = Array.IndexOf(image, (byte)0, fileOffset);
    var current = System.Text.Encoding.UTF8.GetString(image, fileOffset,
        currentEnd < 0 ? 0 : currentEnd - fileOffset);
    if (!current.StartsWith(expectedPrefix, StringComparison.Ordinal))
        throw new BadImageFormatException($"unexpected metadata name while normalizing: {current}");
    if (current.Length == prefixLength)
        return;
    image[endOffset] = 0;
    edits.Add($"metadata name {current} -> {expectedPrefix}");
}

static int FindStringHeapFileOffset(byte[] image, PEHeaders headers)
{
    var metadataOffset = RvaToOffset(headers, headers.CorHeader!.MetadataDirectory.RelativeVirtualAddress);
    var versionLength = BitConverter.ToInt32(image, metadataOffset + 12);
    var streamHeader = (metadataOffset + 16 + versionLength + 3) & ~3;
    var streamCount = BitConverter.ToUInt16(image, streamHeader + 2);
    var position = streamHeader + 4;
    for (var index = 0; index < streamCount; index++)
    {
        var offset = BitConverter.ToInt32(image, position);
        var size = BitConverter.ToInt32(image, position + 4);
        var nameStart = position + 8;
        var nameEnd = Array.IndexOf(image, (byte)0, nameStart);
        if (nameEnd < 0)
            throw new BadImageFormatException("unterminated metadata stream name");
        var name = System.Text.Encoding.ASCII.GetString(image, nameStart, nameEnd - nameStart);
        if (name == "#Strings")
            return checked(metadataOffset + offset);
        position = (nameEnd + 1 + 3) & ~3;
        if (offset < 0 || size < 0)
            throw new BadImageFormatException("invalid metadata stream bounds");
    }
    throw new BadImageFormatException("metadata has no #Strings heap");
}

static string? GetAttributeType(MetadataReader reader, EntityHandle constructor)
{
    EntityHandle parent = constructor.Kind switch
    {
        HandleKind.MemberReference => reader.GetMemberReference((MemberReferenceHandle)constructor).Parent,
        HandleKind.MethodDefinition => reader.GetMethodDefinition((MethodDefinitionHandle)constructor).GetDeclaringType(),
        _ => default
    };
    return FullTypeName(reader, parent);
}

static string? FullTypeName(MetadataReader reader, EntityHandle handle)
{
    return handle.Kind switch
    {
        HandleKind.TypeDefinition => MetadataNames.TypeDefName(reader, reader.GetTypeDefinition((TypeDefinitionHandle)handle)),
        HandleKind.TypeReference => MetadataNames.TypeRefName(reader, reader.GetTypeReference((TypeReferenceHandle)handle)),
        _ => null
    };
}

static TypeDefinitionHandle FindType(MetadataReader reader, string fullName)
{
    foreach (var handle in reader.TypeDefinitions)
    {
        if (MetadataNames.TypeDefName(reader, reader.GetTypeDefinition(handle)) == fullName)
            return handle;
    }
    return default;
}

static int RvaToOffset(PEHeaders headers, int rva)
{
    foreach (var section in headers.SectionHeaders)
    {
        var length = Math.Max(section.VirtualSize, section.SizeOfRawData);
        if (rva >= section.VirtualAddress && rva < section.VirtualAddress + length)
            return section.PointerToRawData + rva - section.VirtualAddress;
    }
    throw new InvalidDataException($"RVA 0x{rva:x} is not in a PE section");
}

static class MetadataNames
{
    public static string TypeDefName(MetadataReader reader, TypeDefinition type)
    {
        var name = reader.GetString(type.Name);
        var declaring = type.GetDeclaringType();
        if (!declaring.IsNil)
            return TypeDefName(reader, reader.GetTypeDefinition(declaring)) + "+" + name;
        var ns = reader.GetString(type.Namespace);
        return ns.Length == 0 ? name : ns + "." + name;
    }

    public static string TypeRefName(MetadataReader reader, TypeReference type)
    {
        var name = reader.GetString(type.Name);
        if (type.ResolutionScope.Kind == HandleKind.TypeReference)
            return TypeRefName(reader, reader.GetTypeReference((TypeReferenceHandle)type.ResolutionScope)) + "+" + name;
        var ns = reader.GetString(type.Namespace);
        return ns.Length == 0 ? name : ns + "." + name;
    }
}

sealed class NameProvider : ISignatureTypeProvider<string, object?>
{
    public static NameProvider Instance { get; } = new();
    public string GetArrayType(string elementType, ArrayShape shape) => elementType + "[]";
    public string GetByReferenceType(string elementType) => elementType + "&";
    public string GetFunctionPointerType(MethodSignature<string> signature) => "fnptr";
    public string GetGenericInstantiation(string genericType, System.Collections.Immutable.ImmutableArray<string> typeArguments)
        => genericType + "<" + string.Join(",", typeArguments) + ">";
    public string GetGenericMethodParameter(object? genericContext, int index) => "!!" + index;
    public string GetGenericTypeParameter(object? genericContext, int index) => "!" + index;
    public string GetModifiedType(string modifier, string unmodifiedType, bool isRequired) => unmodifiedType;
    public string GetPinnedType(string elementType) => elementType;
    public string GetPointerType(string elementType) => elementType + "*";
    public string GetPrimitiveType(PrimitiveTypeCode typeCode) => "System." + typeCode;
    public string GetSZArrayType(string elementType) => elementType + "[]";
    public string GetTypeFromDefinition(MetadataReader reader, TypeDefinitionHandle handle, byte rawTypeKind)
        => MetadataNames.TypeDefName(reader, reader.GetTypeDefinition(handle));
    public string GetTypeFromReference(MetadataReader reader, TypeReferenceHandle handle, byte rawTypeKind)
        => MetadataNames.TypeRefName(reader, reader.GetTypeReference(handle));
    public string GetTypeFromSpecification(MetadataReader reader, object? genericContext,
        TypeSpecificationHandle handle, byte rawTypeKind)
        => reader.GetTypeSpecification(handle).DecodeSignature(this, genericContext);
}
