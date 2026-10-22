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
if (edits.Count > 0)
    File.WriteAllBytes(candidatePath, candidateBytes);
else if (transplantMessages.Any(message => message.StartsWith("transplanted ", StringComparison.Ordinal)))
    File.WriteAllBytes(candidatePath, candidateBytes);

foreach (var edit in edits)
    Console.WriteLine($"maxstack {edit.Key}: {edit.Value}");
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
