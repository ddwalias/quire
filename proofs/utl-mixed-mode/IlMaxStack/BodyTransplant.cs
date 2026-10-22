using System.Collections.Immutable;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;

internal static class BodyTransplant
{
    private static readonly Dictionary<ushort, OpCode> s_opcodes = typeof(OpCodes)
        .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
        .Where(field => field.FieldType == typeof(OpCode))
        .Select(field => (OpCode)field.GetValue(null)!)
        .ToDictionary(opcode => unchecked((ushort)opcode.Value));

    public static List<string> Apply(byte[] candidateBytes, PEReader candidatePe,
        MetadataReader candidateReader, string installPath, IlSectionWriter ilSection)
    {
        var messages = new List<string>();
        var claimedLocalSignatures = new HashSet<StandaloneSignatureHandle>();
        var types = new (string Assembly, string Candidate, string Original, (string Name, int ParameterCount)[] Methods)[]
        {
            ("mcmisc", "Quire.Engine.Misc.StringUtils", "Engine.Misc.StringUtils", new[]
            {
                ("Capitalize", 1), ("IsWhitespace", 1), ("StringSubstitute", 3),
                ("ContainsString", 2), ("PrefixLines", 2), ("StringQuote", 3), ("CharNames", 1),
                ("StringUnquote", 1), ("ExpandStringEscapes", 1), ("SameLines", 2), ("SameLines", 6),
                ("ToCommandString", 1), (".cctor", 0)
            }),
            ("mcmisc", "Quire.Engine.Misc.Pair`2", "Engine.Misc.Pair`2", new[]
            {
                (".ctor", 2), ("get_First", 0), ("set_First", 1), ("get_Second", 0), ("set_Second", 1)
            }),
            ("mcmisc", "Quire.Engine.Misc.Triple`3", "Engine.Misc.Triple`3", new[]
            {
                (".ctor", 3), ("get_First", 0), ("set_First", 1), ("get_Second", 0),
                ("set_Second", 1), ("get_Third", 0), ("set_Third", 1)
            }),
            ("mcmisc", "Quire.Engine.Misc.HashSetUtils", "Engine.Misc.HashSetUtils", new[]
            {
                (".ctor", 0), ("HashSetToArray", 1)
            }),
            ("mcmisc", "Quire.Engine.Misc.XmlUtils", "Engine.Misc.XmlUtils", new[]
            {
                ("BadSyntax", 3), ("BadSyntax", 4), ("BadSyntax", 5), ("BadSyntax", 6),
                ("Dbg", 1), ("Dbg", 2), ("Dbg", 3), ("Dbg", 4), ("LookingAtText", 3),
                ("RequireText", 3), ("FindElementStart", 2), ("FindElementStart", 3),
                ("ScanElement", 3), ("SkipElement", 2), ("ScanTag", 6), ("ScanTagName", 3),
                ("ScanAttributeString", 2), ("SkipMatchingTag", 3), ("SkipBracketedSGML", 2),
                ("SkipSgmlComment", 2), ("FindCandidateXmlnsAttr", 2), ("StripXmlnsAttrs", 1),
                ("EquivalentXml", 2), ("get_TightenXmlnsAttributes", 0),
                ("set_TightenXmlnsAttributes", 1), ("TightenXmlWhitespace", 1),
                ("FindXmlnsAttrs", 3), ("SkipWhitespace", 2), ("BackOverWhitespace", 2),
                ("SkipCDATA", 2), ("UnorderedEqual", 2), ("UnorderedEqual", 3),
                ("OrderedArbitrarily", 1), ("TestInit", 0), ("TestCheck", 4),
                ("TestCheck", 1), ("BumpTest", 0), ("TestFinalize", 0), ("SelfTest", 1),
                (".ctor", 0), (".cctor", 0)
            }),
            ("mcmisc", "Quire.Engine.Misc.LispUtils", "Engine.Misc.LispUtils", new[]
            {
                ("skipLine", 2), ("PrettyPrint", 1), ("PrettyString", 1), ("PrettyStringTop", 2),
                ("PrettyStringWorker", 6),
                ("IsDefinerName", 1), ("IdentifierName", 1), ("IndentTo", 2),
                ("scanExpression", 3),
                ("skipExpression", 2), ("skipList", 2), ("skipAtom", 2),
                ("findExpressionStart", 2), ("findExpressionStart", 3), ("skipString", 2),
                ("scanExpressionLine", 1), ("scanExpressionLine", 2), ("scanExpressionLine", 3),
                ("scanExpressions", 2), ("partitionExpressionLine", 3)
            }),
            ("mcmisc", "Quire.Engine.Misc.LispUtils+LinePartition", "Engine.Misc.LispUtils+LinePartition", new[]
            {
                (".ctor", 1), ("get_Count", 0), ("PutSegment", 2), ("PeekSegment", 0), ("TakeSegment", 0),
                ("SegmentsFrom", 1), ("JoinedTextFrom", 1)
            }),
            ("mcmisc", "Quire.Engine.Misc.FileUtils", "Engine.Misc.FileUtils", new[]
            {
                ("StreamToPath", 1), ("ProcessInclude", 2), ("SkipPast", 2),
                ("ComputeLineNumber", 2), ("ComputeLineNumber", 3), ("ComputeLineNumber", 4),
                ("ReadExpanded", 1), ("ReadExpanded", 2),
                ("ContractUnicode", 1), ("ExpandUnicode", 1), ("UnicodeToAscii", 2),
                ("AsciiToUnicode", 2), ("CharNameToUnicodeString", 2),
                ("UnicodeToCharName", 2), ("GreekShortcutKeyToUnicode", 2),
                ("LongUnicodeToShortcutKey", 3), ("UnicodeToGreekShortcutKey", 2),
                ("InitCharMappings", 0), ("InitNamedCharMap", 0), (".cctor", 0)
            }),
            ("utl-mgd", "Quire.utl_mgd.LabeledDictionary", "utl_mgd.LabeledDictionary", new[]
            {
                (".ctor", 1), (".ctor", 1), (".ctor", 0), ("Lock", 0), ("OnChange", 0),
                ("get_IsFixedSize", 0), ("get_IsReadOnly", 0), ("get_Item", 1), ("set_Item", 2),
                ("get_Keys", 0), ("get_Values", 0), ("Add", 2), ("Clear", 0), ("Contains", 1),
                ("Contains1", 1), ("Remove", 1), ("_e1", 0), ("_e2", 0), ("get_Count", 0),
                ("get_IsSynchronized", 0), ("get_SyncRoot", 0), ("CopyTo", 2), ("Copy", 0),
                ("MergeResult", 1)
            }),
            ("utl-mgd", "Quire.utl_mgd.SafeMonitor", "utl_mgd.SafeMonitor", new[]
            {
                ("GetCriticalSection", 2), ("Free", 0), ("on_exit", 0), (".cctor", 0),
                ("Enter", 1), ("TryEnter", 1), ("Exit", 1), (".ctor", 0)
            }),
            ("utl-mgd", "Quire.utl_mgd.SafeMonitor+CSecRecord", "utl_mgd.SafeMonitor+CSecRecord", new[]
            {
                (".ctor", 2)
            }),
            ("utl-mgd", "Quire.utl_mgd.TaskQueue", "utl_mgd.TaskQueue", new[]
            {
                (".ctor", 0)
            }),
            ("utl-mgd", "Quire.utl_mgd.MessageQueue`3", "utl_mgd.MessageQueue`3", new[]
            {
                ("Init", 1), ("MsgLoop", 1), ("!MessageQueue`3", 0), ("~MessageQueue`3", 0)
            }),
            ("utl-mgd", "Quire.utl_mgd.LabelSet", "utl_mgd.LabelSet", new[]
            {
                ("Empty", 0)
            }),
            ("utl-mgd", "Quire.utl_mgd.LabelTrie", "utl_mgd.LabelTrie", new[]
            {
                ("Find", 2)
            }),
            ("utl-mgd", "Quire.utl_mgd.LabelUtils", "utl_mgd.LabelUtils", new[]
            {
                ("SplitMangledLabels", 1), ("LookupPossiblyWild", 2), ("FindBinding", 2)
            }),
            ("utl-mgd", "Quire.utl_mgd._Hex", "utl_mgd._Hex", new[]
            {
                ("FromHex", 1)
            })
        };
        foreach (var type in types)
        {
            var originalPath = Path.Combine(installPath, type.Assembly + ".dll");
            if (!File.Exists(originalPath))
            {
                messages.Add($"skip {type.Original}: {type.Assembly}.dll missing");
                continue;
            }

            using var originalStream = File.OpenRead(originalPath);
            using var originalPe = new PEReader(originalStream);
            var originalReader = originalPe.GetMetadataReader();
            var originalBytes = File.ReadAllBytes(originalPath);
            var candidateType = FindType(candidateReader, type.Candidate);
            var originalType = FindType(originalReader, type.Original);
            if (candidateType.IsNil || originalType.IsNil)
                continue;
            var overloadIndexes = new Dictionary<(string Name, int ParameterCount), int>();
            foreach (var (name, parameterCount) in type.Methods)
            {
                var overloadKey = (name, parameterCount);
                var overloadIndex = overloadIndexes.GetValueOrDefault(overloadKey);
                overloadIndexes[overloadKey] = overloadIndex + 1;
                var originalMethod = FindMethodAt(originalReader, originalType, name, parameterCount, overloadIndex);
                var candidateMethod = originalMethod.IsNil ? default : FindMatchingCandidateMethod(
                    originalReader, originalMethod, candidateReader, candidateType, name, parameterCount);
                if (originalMethod.IsNil || candidateMethod.IsNil)
                {
                    messages.Add($"skip {type.Original}::{name}: method definition missing");
                    continue;
                }

                var originalDefinition = originalReader.GetMethodDefinition(originalMethod);
                var candidateDefinition = candidateReader.GetMethodDefinition(candidateMethod);
                var originalOffset = RvaToOffset(originalPe.PEHeaders, originalDefinition.RelativeVirtualAddress);
                var candidateOffset = RvaToOffset(candidatePe.PEHeaders, candidateDefinition.RelativeVirtualAddress);
                var sourceSize = GetMethodBodySize(originalBytes, originalOffset);
                var capacity = GetBodyCapacity(candidateReader, candidatePe.PEHeaders,
                    candidateDefinition.RelativeVirtualAddress);
                var body = originalBytes.AsSpan(originalOffset, sourceSize).ToArray();
                var candidateMethodBody = candidatePe.GetMethodBody(candidateDefinition.RelativeVirtualAddress);
                if (!PatchLocalSignature(body, originalReader, candidateReader, candidateBytes,
                        candidatePe, candidatePe.PEHeaders, candidateMethodBody.LocalSignature, claimedLocalSignatures))
                {
                    messages.Add($"skip {type.Original}::{name}: incompatible candidate local signature");
                    continue;
                }
                if (!PatchInstructionTokens(body, originalDefinition.RelativeVirtualAddress, originalPe.PEHeaders,
                        originalReader, candidateReader, candidateBytes, candidatePe, candidatePe.PEHeaders,
                        out var transplantFailure))
                {
                    messages.Add($"skip {type.Original}::{name}: {transplantFailure}");
                    continue;
                }

                if (sourceSize > capacity)
                {
                    ilSection.PlaceBody(candidateBytes, candidateReader, candidatePe.PEHeaders,
                        candidateMethod, body);
                    if (type.Original == "Engine.Misc.XmlUtils" && name == "LookingAtText")
                    messages.Add($"transplanted {type.Original}::{name} IL from {type.Assembly}.dll into .ilnew");
                }
                else
                {
                    body.CopyTo(candidateBytes.AsSpan(candidateOffset, sourceSize));
                    messages.Add($"transplanted {type.Original}::{name} IL from {type.Assembly}.dll");
                }
            }
        }
        return messages;
    }

    private static bool PatchLocalSignature(byte[] body, MetadataReader originalReader,
        MetadataReader candidateReader, byte[] candidateBytes, PEReader candidatePe, PEHeaders candidateHeaders,
        StandaloneSignatureHandle candidateLocalSignature,
        HashSet<StandaloneSignatureHandle> claimedLocalSignatures)
    {
        var flags = BitConverter.ToUInt16(body, 0);
        if ((flags & 0x3) == 0x2)
            return true;
        if ((flags & 0x3) != 0x3 || ((flags >> 12) & 0xf) * 4 < 12)
            return false;

        var originalToken = BitConverter.ToUInt32(body, 8);
        if (originalToken == 0)
            return true;
        if ((originalToken >> 24) != 0x11)
            return false;

        var originalHandle = MetadataTokens.StandaloneSignatureHandle((int)(originalToken & 0x00ffffff));
        ImmutableArray<string> originalLocals;
        try
        {
            originalLocals = originalReader.GetStandaloneSignature(originalHandle)
                .DecodeLocalSignature(NameProvider.Instance, null);
        }
        catch (BadImageFormatException)
        {
            return false;
        }
        var mappedOriginalLocals = originalLocals
            .Select(local => MapSignatureType(candidateReader, local))
            .ToImmutableArray();
        var targetSignature = EncodeLocalSignature(mappedOriginalLocals, candidateReader);
        if (targetSignature is null)
            return false;

        if (candidateLocalSignature.IsNil)
        {
            foreach (var signatureHandle in EnumerateStandaloneSignatures(candidateReader))
            {
                if (claimedLocalSignatures.Contains(signatureHandle)
                    || StandaloneSignatureInUse(candidateReader, candidatePe, signatureHandle))
                    continue;
                try
                {
                    var signatureBlob = candidateReader.GetStandaloneSignature(signatureHandle).Signature;
                    if (!WriteBlobPayload(candidateReader, candidateHeaders, candidateBytes,
                            signatureHandle, signatureBlob, targetSignature))
                        continue;
                    BitConverter.TryWriteBytes(body.AsSpan(8, sizeof(uint)),
                        unchecked((uint)MetadataTokens.GetToken(signatureHandle)));
                    claimedLocalSignatures.Add(signatureHandle);
                    return true;
                }
                catch (BadImageFormatException)
                {
                    continue;
                }
            }
            return false;
        }
        try
        {
            var candidateLocals = candidateReader.GetStandaloneSignature(candidateLocalSignature)
                .DecodeLocalSignature(NameProvider.Instance, null);
            if (mappedOriginalLocals.SequenceEqual(candidateLocals))
            {
                BitConverter.TryWriteBytes(body.AsSpan(8, sizeof(uint)),
                    unchecked((uint)MetadataTokens.GetToken(candidateLocalSignature)));
                claimedLocalSignatures.Add(candidateLocalSignature);
                return true;
            }
        }
        catch (BadImageFormatException)
        {
            return false;
        }
        var users = 0;
        foreach (var methodHandle in candidateReader.MethodDefinitions)
        {
            var method = candidateReader.GetMethodDefinition(methodHandle);
            if (method.RelativeVirtualAddress == 0)
                continue;
            try
            {
                if (candidatePe.GetMethodBody(method.RelativeVirtualAddress).LocalSignature == candidateLocalSignature)
                    users++;
            }
            catch (BadImageFormatException)
            {
                continue;
            }
        }
        try
        {
            if (users == 1 && !claimedLocalSignatures.Contains(candidateLocalSignature)
                && WriteBlobPayload(candidateReader, candidateHeaders, candidateBytes,
                    candidateLocalSignature, candidateReader.GetStandaloneSignature(candidateLocalSignature).Signature,
                    targetSignature))
            {
                BitConverter.TryWriteBytes(body.AsSpan(8, sizeof(uint)),
                    unchecked((uint)MetadataTokens.GetToken(candidateLocalSignature)));
                claimedLocalSignatures.Add(candidateLocalSignature);
                return true;
            }

            foreach (var signatureHandle in EnumerateStandaloneSignatures(candidateReader))
            {
                if (signatureHandle == candidateLocalSignature
                    || claimedLocalSignatures.Contains(signatureHandle)
                    || StandaloneSignatureInUse(candidateReader, candidatePe, signatureHandle))
                    continue;
                if (!WriteBlobPayload(candidateReader, candidateHeaders, candidateBytes,
                        signatureHandle, candidateReader.GetStandaloneSignature(signatureHandle).Signature,
                        targetSignature))
                    continue;
                BitConverter.TryWriteBytes(body.AsSpan(8, sizeof(uint)),
                    unchecked((uint)MetadataTokens.GetToken(signatureHandle)));
                claimedLocalSignatures.Add(signatureHandle);
                return true;
            }
            return false;
        }
        catch (BadImageFormatException)
        {
            return false;
        }
    }

    private static bool StandaloneSignatureInUse(MetadataReader reader, PEReader pe,
        StandaloneSignatureHandle signatureHandle)
    {
        foreach (var methodHandle in reader.MethodDefinitions)
        {
            var method = reader.GetMethodDefinition(methodHandle);
            if (method.RelativeVirtualAddress == 0)
                continue;
            MethodBodyBlock body;
            try { body = pe.GetMethodBody(method.RelativeVirtualAddress); }
            catch (BadImageFormatException) { continue; }
            if (body.LocalSignature == signatureHandle)
                return true;

            var code = body.GetILContent().AsSpan();
            var position = 0;
            while (position < code.Length)
            {
                ushort opcodeValue = code[position++];
                if (opcodeValue == 0xfe)
                {
                    if (position >= code.Length)
                        return true;
                    opcodeValue = (ushort)(0xfe00 | code[position++]);
                }
                if (!s_opcodes.TryGetValue(opcodeValue, out var opcode))
                    return true;
                var operandSize = GetOperandSize(opcode.OperandType, code, position);
                if (operandSize < 0 || position + operandSize > code.Length)
                    return true;
                if (opcode.OperandType == OperandType.InlineSig && operandSize == sizeof(uint)
                    && BitConverter.ToUInt32(code.Slice(position, sizeof(uint)))
                        == unchecked((uint)MetadataTokens.GetToken(signatureHandle)))
                    return true;
                position += operandSize;
            }
        }
        return false;
    }

    private static byte[]? EncodeLocalSignature(ImmutableArray<string> locals, MetadataReader target)
    {
        var bytes = new List<byte> { 0x07 };
        WriteCompressedUInt(bytes, locals.Length);
        foreach (var local in locals)
            if (!EncodeLocalType(local, target, bytes))
            {
                Console.WriteLine($"cannot encode local signature type {local}");
                return null;
            }
        return bytes.ToArray();
    }

    private static bool EncodeLocalType(string type, MetadataReader target, List<byte> bytes)
    {
        type = MapSignatureType(target, type);
        if (type.StartsWith("!!", StringComparison.Ordinal)
            && int.TryParse(type.AsSpan(2), out var genericMethodParameter))
        {
            bytes.Add(0x1e); // MVAR
            WriteCompressedUInt(bytes, genericMethodParameter);
            return true;
        }
        if (type.StartsWith('!') && int.TryParse(type.AsSpan(1), out var genericTypeParameter))
        {
            bytes.Add(0x13); // VAR
            WriteCompressedUInt(bytes, genericTypeParameter);
            return true;
        }
        var genericOpen = type.IndexOf('<');
        if (genericOpen > 0 && type.EndsWith('>'))
        {
            var genericName = type[..genericOpen];
            var genericDefinition = FindType(target, genericName);
            var genericReference = genericDefinition.IsNil ? FindTypeReference(target, genericName) : default;
            if (genericDefinition.IsNil && genericReference.IsNil)
                return false;
            bytes.Add(0x15); // GENERICINST
            bytes.Add(IsValueType(genericName) ? (byte)0x11 : (byte)0x12);
            WriteCompressedUInt(bytes, genericDefinition.IsNil
                ? (MetadataTokens.GetRowNumber(genericReference) << 2) | 1
                : MetadataTokens.GetRowNumber(genericDefinition) << 2);
            var arguments = SplitGenericArguments(type[(genericOpen + 1)..^1]);
            WriteCompressedUInt(bytes, arguments.Count);
            foreach (var argument in arguments)
                if (!EncodeLocalType(argument, target, bytes))
                    return false;
            return true;
        }
        var primitive = type switch
        {
            "System.Boolean" => (byte)0x02, "System.Char" => (byte)0x03,
            "System.SByte" => (byte)0x04, "System.Byte" => (byte)0x05,
            "System.Int16" => (byte)0x06, "System.UInt16" => (byte)0x07,
            "System.Int32" => (byte)0x08, "System.UInt32" => (byte)0x09,
            "System.Int64" => (byte)0x0a, "System.UInt64" => (byte)0x0b,
            "System.Single" => (byte)0x0c, "System.Double" => (byte)0x0d,
            "System.String" => (byte)0x0e, "System.IntPtr" => (byte)0x18,
            "System.UIntPtr" => (byte)0x19, "System.Object" => (byte)0x1c,
            _ => (byte)0
        };
        if (primitive != 0)
        {
            bytes.Add(primitive);
            return true;
        }
        if (type.EndsWith("[]", StringComparison.Ordinal))
        {
            bytes.Add(0x1d);
            return EncodeLocalType(type[..^2], target, bytes);
        }
        if (type.EndsWith("*", StringComparison.Ordinal))
        {
            bytes.Add(0x0f); // PTR
            var elementType = type[..^1];
            if (elementType == "System.Void")
            {
                bytes.Add(0x01);
                return true;
            }
            return EncodeLocalType(elementType, target, bytes);
        }

        var definition = FindType(target, type);
        if (!definition.IsNil)
        {
            bytes.Add(IsValueType(type) ? (byte)0x11 : (byte)0x12);
            WriteCompressedUInt(bytes, (MetadataTokens.GetRowNumber(definition) << 2) | 0);
            return true;
        }
        var specification = FindTypeSpecification(target, type);
        if (!specification.IsNil)
        {
            bytes.Add(IsValueType(type) ? (byte)0x11 : (byte)0x12);
            WriteCompressedUInt(bytes, (MetadataTokens.GetRowNumber(specification) << 2) | 2);
            return true;
        }
        var reference = FindTypeReference(target, type);
        if (reference.IsNil)
            return false;
        bytes.Add(IsValueType(type) ? (byte)0x11 : (byte)0x12);
        WriteCompressedUInt(bytes, (MetadataTokens.GetRowNumber(reference) << 2) | 1);
        return true;
    }

    private static bool IsValueType(string type) => type is "System.Collections.DictionaryEntry" or "_RTL_CRITICAL_SECTION"
        || type.StartsWith("System.ReadOnlySpan`", StringComparison.Ordinal)
        || type.StartsWith("System.Collections.Generic.Queue`1+Enumerator", StringComparison.Ordinal);

    private static List<string> SplitGenericArguments(string arguments)
    {
        var result = new List<string>();
        var depth = 0;
        var start = 0;
        for (var i = 0; i < arguments.Length; i++)
        {
            if (arguments[i] == '<') depth++;
            else if (arguments[i] == '>') depth--;
            else if (arguments[i] == ',' && depth == 0)
            {
                result.Add(arguments[start..i].Trim());
                start = i + 1;
            }
        }
        result.Add(arguments[start..].Trim());
        return result;
    }

    private static List<(string Type, byte[] Bytes)>? ParseSimpleLocalSignature(
        MetadataReader reader, byte[] payload)
    {
        if (payload.Length < 2 || payload[0] != 0x07)
            return null;
        var position = 1;
        var count = ReadCompressedUInt(payload, ref position);
        var locals = new List<(string Type, byte[] Bytes)>();
        for (var i = 0; i < count; i++)
        {
            var local = ReadLocalType(reader, payload, ref position);
            if (local is null)
                return null;
            locals.Add(local.Value);
        }
        return position == payload.Length ? locals : null;
    }

    private static (string Type, byte[] Bytes)? ReadLocalType(MetadataReader reader,
        byte[] payload, ref int position)
    {
        var start = position;
        var element = payload[position++];
        string type;
        if (element == 0x1d)
        {
            var inner = ReadLocalType(reader, payload, ref position);
            if (inner is null)
                return null;
            type = inner.Value.Type + "[]";
        }
        else if (element is 0x11 or 0x12)
        {
            var coded = ReadCompressedUInt(payload, ref position);
            var row = coded >> 2;
            var tag = coded & 0x3;
            EntityHandle handle = tag switch
            {
                0 => MetadataTokens.TypeDefinitionHandle(checked((int)row)),
                1 => MetadataTokens.TypeReferenceHandle(checked((int)row)),
                2 => MetadataTokens.TypeSpecificationHandle(checked((int)row)),
                _ => default
            };
            type = TypeName(reader, handle) ?? "?";
            if (tag == 2)
                type = reader.GetTypeSpecification((TypeSpecificationHandle)handle)
                    .DecodeSignature(NameProvider.Instance, null);
        }
        else
        {
            type = PrimitiveElementType(element);
        }
        return (type, payload.AsSpan(start, position - start).ToArray());
    }

    private static byte[]? ReorderLocalSignature(ImmutableArray<string> desired,
        List<(string Type, byte[] Bytes)> existing)
    {
        var remaining = existing.ToList();
        var ordered = new List<byte[]>();
        foreach (var type in desired)
        {
            var index = remaining.FindIndex(item => item.Type == type);
            if (index < 0)
                return null;
            ordered.Add(remaining[index].Bytes);
            remaining.RemoveAt(index);
        }
        if (remaining.Count != 0)
            return null;
        var bytes = new List<byte> { 0x07 };
        WriteCompressedUInt(bytes, ordered.Count);
        foreach (var segment in ordered)
            bytes.AddRange(segment);
        return bytes.ToArray();
    }

    private static string PrimitiveElementType(byte type) => type switch
    {
        0x02 => "System.Boolean", 0x03 => "System.Char", 0x04 => "System.SByte", 0x05 => "System.Byte",
        0x06 => "System.Int16", 0x07 => "System.UInt16", 0x08 => "System.Int32", 0x09 => "System.UInt32",
        0x0a => "System.Int64", 0x0b => "System.UInt64", 0x0c => "System.Single", 0x0d => "System.Double",
        0x0e => "System.String", 0x18 => "System.IntPtr", 0x19 => "System.UIntPtr", 0x1c => "System.Object",
        _ => "?"
    };

    private static int ReadCompressedUInt(byte[] bytes, ref int position)
    {
        var first = bytes[position++];
        if ((first & 0x80) == 0)
            return first;
        if ((first & 0xc0) == 0x80)
            return ((first & 0x3f) << 8) | bytes[position++];
        if ((first & 0xe0) == 0xc0)
            return ((first & 0x1f) << 24) | (bytes[position++] << 16)
                | (bytes[position++] << 8) | bytes[position++];
        throw new BadImageFormatException("invalid compressed integer");
    }

    private static void WriteCompressedUInt(List<byte> bytes, int value)
    {
        if (value < 0x80)
            bytes.Add((byte)value);
        else if (value < 0x4000)
        {
            bytes.Add((byte)((value >> 8) | 0x80));
            bytes.Add((byte)value);
        }
        else
        {
            bytes.Add((byte)((value >> 24) | 0xc0));
            bytes.Add((byte)(value >> 16));
            bytes.Add((byte)(value >> 8));
            bytes.Add((byte)value);
        }
    }

    private static byte[] ReadBlobPayload(MetadataReader reader, PEHeaders headers,
        byte[] bytes, BlobHandle handle)
    {
        var stream = FindMetadataStream(headers, bytes, "#Blob");
        var offset = stream + MetadataTokens.GetHeapOffset(handle);
        var position = offset;
        var length = ReadCompressedUInt(bytes, ref position);
        return bytes.AsSpan(position, length).ToArray();
    }

    private static bool WriteBlobPayload(MetadataReader reader, PEHeaders headers,
        byte[] bytes, StandaloneSignatureHandle ownerSignature, BlobHandle handle, byte[] payload)
    {
        // The CLI Blob heap can deduplicate StandaloneSig and Field signature payloads.
        // Rewriting a local-signature blob shared with a Field corrupts the field table.
        foreach (var fieldHandle in reader.FieldDefinitions)
            if (reader.GetFieldDefinition(fieldHandle).Signature == handle)
                return false;
        foreach (var signatureHandle in EnumerateStandaloneSignatures(reader))
            if (signatureHandle != ownerSignature
                && reader.GetStandaloneSignature(signatureHandle).Signature == handle)
                return false;

        var stream = FindMetadataStream(headers, bytes, "#Blob");
        var offset = stream + MetadataTokens.GetHeapOffset(handle);
        var position = offset;
        var length = ReadCompressedUInt(bytes, ref position);
        var oldPrefixLength = position - offset;
        var prefix = new List<byte>();
        WriteCompressedUInt(prefix, payload.Length);
        if (prefix.Count + payload.Length > oldPrefixLength + length)
            return false;
        prefix.CopyTo(bytes, offset);
        payload.CopyTo(bytes.AsSpan(offset + prefix.Count, payload.Length));
        return true;
    }

    private static int FindMetadataStream(PEHeaders headers, byte[] bytes, string name)
    {
        var cli = headers.CorHeader ?? throw new BadImageFormatException("missing CLI header");
        var position = RvaToOffset(headers, cli.MetadataDirectory.RelativeVirtualAddress);
        position += 12;
        var versionLength = checked((int)BitConverter.ToUInt32(bytes, position));
        position += 4 + ((versionLength + 3) & ~3);
        position += 2;
        var streams = BitConverter.ToUInt16(bytes, position);
        position += 2;
        for (var i = 0; i < streams; i++)
        {
            var streamOffset = checked((int)BitConverter.ToUInt32(bytes, position));
            position += 8;
            var nameStart = position;
            while (bytes[position] != 0)
                position++;
            var streamName = System.Text.Encoding.ASCII.GetString(bytes, nameStart, position - nameStart);
            position = (position + 4) & ~3;
            if (streamName == name)
                return RvaToOffset(headers, cli.MetadataDirectory.RelativeVirtualAddress) + streamOffset;
        }
        throw new BadImageFormatException($"metadata stream {name} is missing");
    }

    private static bool PatchInstructionTokens(byte[] body, int originalRva, PEHeaders originalHeaders,
        MetadataReader originalReader, MetadataReader candidateReader, byte[] candidateBytes,
        PEReader candidatePe, PEHeaders candidateHeaders, out string failure)
    {
        failure = "IL token has no candidate metadata counterpart";
        var codeOffset = GetCodeOffset(body);
        var codeSize = GetCodeSize(body);
        var stringTokens = ReadCandidateStrings(candidateReader, candidatePe);
        var il = body.AsSpan(codeOffset, codeSize);
        var position = 0;
        while (position < il.Length)
        {
            ushort opcodeValue = il[position++];
            if (opcodeValue == 0xfe)
            {
                if (position >= il.Length)
                {
                    failure = "truncated extended opcode";
                    return false;
                }
                opcodeValue = (ushort)(0xfe00 | il[position++]);
            }
            if (!s_opcodes.TryGetValue(opcodeValue, out var opcode))
            {
                failure = $"unknown IL opcode 0x{opcodeValue:x4} at offset 0x{position - 1:x}";
                return false;
            }

            var operandPosition = position;
            var operandSize = GetOperandSize(opcode.OperandType, il, position);
            if (operandSize < 0 || position + operandSize > il.Length)
            {
                failure = $"invalid {opcode.OperandType} operand size {operandSize} at offset 0x{position:x}";
                return false;
            }

            if (opcode.OperandType is OperandType.InlineString or OperandType.InlineMethod
                or OperandType.InlineField or OperandType.InlineType or OperandType.InlineTok
                or OperandType.InlineSig)
            {
                var token = BitConverter.ToUInt32(il.Slice(operandPosition, sizeof(uint)));
                uint mapped;
                if (opcode.OperandType == OperandType.InlineString)
                {
                    if ((token >> 24) != 0x70)
                    {
                        failure = $"invalid user-string token 0x{token:x8}";
                        return false;
                    }
                    var value = originalReader.GetUserString(MetadataTokens.UserStringHandle((int)(token & 0x00ffffff)));
                    if (!stringTokens.TryGetValue(value, out mapped))
                    {
                        failure = $"user string not found in candidate: {value}";
                        return false;
                    }
                }
                else if (!TryMapMetadataToken(token, originalReader, candidateReader, out mapped))
                {
                    failure = $"unmapped metadata operand 0x{token:x8} ({opcode.OperandType})";
                    Console.WriteLine($"unmapped metadata operand 0x{token:x8} ({opcode.OperandType})");
                    return false;
                }

                BitConverter.TryWriteBytes(il.Slice(operandPosition, sizeof(uint)), mapped);
            }
            position += operandSize;
        }
        return true;
    }

    private static bool TryMapMetadataToken(uint token, MetadataReader source,
        MetadataReader target, out uint mapped)
    {
        mapped = 0;
        EntityHandle handle;
        try { handle = MetadataTokens.EntityHandle(unchecked((int)token)); }
        catch { return false; }

        switch (handle.Kind)
        {
            case HandleKind.MemberReference:
            {
                var reference = source.GetMemberReference((MemberReferenceHandle)handle);
                var parent = TypeName(source, reference.Parent);
                if (parent is null)
                    return false;
                parent = MapSignatureType(target, parent);
                if (parent is null)
                    return false;
                var name = source.GetString(reference.Name);
                var signature = reference.GetKind() == MemberReferenceKind.Method
                    ? MapMethodReferenceKey(target, MethodReferenceKey(source, (MemberReferenceHandle)handle))
                    : MapSignatureType(target, FieldReferenceKey(source, (MemberReferenceHandle)handle));
                foreach (var candidateHandle in EnumerateMemberReferences(target))
                {
                    var candidate = target.GetMemberReference(candidateHandle);
                    if (TypeName(target, candidate.Parent) == parent && target.GetString(candidate.Name) == name
                        && (candidate.GetKind() == MemberReferenceKind.Method
                            ? MethodReferenceKey(target, candidateHandle)
                            : FieldReferenceKey(target, candidateHandle)) == signature)
                    {
                        mapped = unchecked((uint)MetadataTokens.GetToken(candidateHandle));
                        return true;
                    }
                }
                Console.WriteLine($"unmatched member reference {parent}::{name} {signature}");
                return false;
            }
            case HandleKind.MethodDefinition:
            {
                var method = source.GetMethodDefinition((MethodDefinitionHandle)handle);
                var ownerName = TypeName(source, method.GetDeclaringType());
                var mappedOwnerName = FindMappedTypeName(target, ownerName!);
                var owner = FindType(target, mappedOwnerName ?? ownerName!);
                if (owner.IsNil)
                    return false;
                var sourceSignature = method.DecodeSignature(NameProvider.Instance, null);
                var key = MapMethodReferenceKey(target,
                    sourceSignature.ReturnType + "(" + string.Join(",", sourceSignature.ParameterTypes) + ")");
                foreach (var candidateHandle in target.GetTypeDefinition(owner).GetMethods())
                {
                    var candidateSignature = target.GetMethodDefinition(candidateHandle)
                        .DecodeSignature(NameProvider.Instance, null);
                    var candidateKey = candidateSignature.ReturnType + "("
                        + string.Join(",", candidateSignature.ParameterTypes) + ")";
                    if (target.GetString(target.GetMethodDefinition(candidateHandle).Name) == source.GetString(method.Name)
                        && candidateKey == key)
                    {
                        mapped = unchecked((uint)MetadataTokens.GetToken(candidateHandle));
                        return true;
                    }
                }
                return false;
            }
            case HandleKind.FieldDefinition:
            {
                var field = source.GetFieldDefinition((FieldDefinitionHandle)handle);
                var ownerName = TypeName(source, field.GetDeclaringType());
                var mappedOwnerName = FindMappedTypeName(target, ownerName!);
                var owner = FindType(target, mappedOwnerName ?? ownerName!);
                if (owner.IsNil)
                    return false;
                var name = source.GetString(field.Name);
                foreach (var candidateHandle in target.GetTypeDefinition(owner).GetFields())
                {
                    var candidateField = target.GetFieldDefinition(candidateHandle);
                    if (target.GetString(candidateField.Name) == name)
                    {
                        mapped = unchecked((uint)MetadataTokens.GetToken(candidateHandle));
                        return true;
                    }
                }
                return false;
            }
            case HandleKind.TypeReference:
            {
                var name = TypeName(source, handle);
                var candidateHandle = FindTypeReference(target, MapSignatureType(target, name!));
                if (candidateHandle.IsNil)
                    return false;
                mapped = unchecked((uint)MetadataTokens.GetToken(candidateHandle));
                return true;
            }
            case HandleKind.TypeDefinition:
            {
                var name = TypeName(source, handle);
                var candidateHandle = FindType(target, FindMappedTypeName(target, name!) ?? name!);
                if (candidateHandle.IsNil)
                    return false;
                mapped = unchecked((uint)MetadataTokens.GetToken(candidateHandle));
                return true;
            }
            case HandleKind.TypeSpecification:
            {
                var name = source.GetTypeSpecification((TypeSpecificationHandle)handle)
                    .DecodeSignature(NameProvider.Instance, null);
                for (var row = 1; row <= target.GetTableRowCount(TableIndex.TypeSpec); row++)
                {
                    var candidateHandle = MetadataTokens.TypeSpecificationHandle(row);
                    if (target.GetTypeSpecification(candidateHandle)
                        .DecodeSignature(NameProvider.Instance, null) == MapSignatureType(target, name))
                    {
                        mapped = unchecked((uint)MetadataTokens.GetToken(candidateHandle));
                        return true;
                    }
                }
                return false;
            }
            case HandleKind.MethodSpecification:
            {
                var specification = source.GetMethodSpecification((MethodSpecificationHandle)handle);
                if (!TryMapMetadataToken(unchecked((uint)MetadataTokens.GetToken(specification.Method)),
                        source, target, out var mappedMethod))
                    return false;
                var arguments = specification.DecodeSignature(NameProvider.Instance, null);
                for (var row = 1; row <= target.GetTableRowCount(TableIndex.MethodSpec); row++)
                {
                    var candidateHandle = MetadataTokens.MethodSpecificationHandle(row);
                    var candidate = target.GetMethodSpecification(candidateHandle);
                    var candidateArguments = candidate.DecodeSignature(NameProvider.Instance, null);
                    if (unchecked((uint)MetadataTokens.GetToken(candidate.Method)) == mappedMethod
                        && arguments.SequenceEqual(candidateArguments))
                    {
                        mapped = unchecked((uint)MetadataTokens.GetToken(candidateHandle));
                        return true;
                    }
                }
                return false;
            }
            case HandleKind.StandaloneSignature:
            {
                ImmutableArray<string> signature;
                try
                {
                    signature = source.GetStandaloneSignature((StandaloneSignatureHandle)handle)
                        .DecodeLocalSignature(NameProvider.Instance, null);
                }
                catch (BadImageFormatException)
                {
                    return false;
                }
                foreach (var candidateHandle in EnumerateStandaloneSignatures(target))
                {
                    ImmutableArray<string> candidateSignature;
                    try
                    {
                        candidateSignature = target.GetStandaloneSignature(candidateHandle)
                            .DecodeLocalSignature(NameProvider.Instance, null);
                    }
                    catch (BadImageFormatException)
                    {
                        continue;
                    }
                    if (candidateSignature.SequenceEqual(signature))
                    {
                        mapped = unchecked((uint)MetadataTokens.GetToken(candidateHandle));
                        return true;
                    }
                }
                return false;
            }
            default:
                return false;
        }
    }

    private static Dictionary<string, uint> ReadCandidateStrings(MetadataReader reader, PEReader pe)
    {
        var values = new Dictionary<string, uint>(StringComparer.Ordinal);
        foreach (var methodHandle in reader.MethodDefinitions)
        {
            var method = reader.GetMethodDefinition(methodHandle);
            if (method.RelativeVirtualAddress == 0)
                continue;
            byte[] codeBytes;
            try { codeBytes = pe.GetMethodBody(method.RelativeVirtualAddress).GetILBytes() ?? []; }
            catch (BadImageFormatException) { continue; }
            var code = codeBytes.AsSpan();
            var position = 0;
            while (position < code.Length)
            {
                var opValue = (ushort)code[position++];
                if (opValue == 0xfe)
                    opValue = (ushort)(0xfe00 | code[position++]);
                if (!s_opcodes.TryGetValue(opValue, out var opcode))
                    break;
                var operandPosition = position;
                var size = GetOperandSize(opcode.OperandType, code, position);
                if (size < 0 || position + size > code.Length)
                    break;
                if (opcode.OperandType == OperandType.InlineString)
                {
                    var token = BitConverter.ToUInt32(code.Slice(operandPosition, sizeof(uint)));
                    var handle = MetadataTokens.UserStringHandle((int)(token & 0x00ffffff));
                    values.TryAdd(reader.GetUserString(handle), token);
                }
                position += size;
            }
        }
        return values;
    }

    private static string MethodReferenceKey(MetadataReader reader, MemberReferenceHandle handle)
    {
        var signature = reader.GetMemberReference(handle).DecodeMethodSignature(NameProvider.Instance, null);
        return signature.ReturnType + "(" + string.Join(",", signature.ParameterTypes) + ")";
    }

    private static string FieldReferenceKey(MetadataReader reader, MemberReferenceHandle handle)
        => reader.GetMemberReference(handle).DecodeFieldSignature(NameProvider.Instance, null);

    private static string MapMethodReferenceKey(MetadataReader target, string signature)
    {
        var open = signature.IndexOf('(');
        if (open < 0 || !signature.EndsWith(')'))
            return MapSignatureType(target, signature);
        var returnType = MapSignatureType(target, signature[..open]);
        var parameters = SplitGenericArguments(signature[(open + 1)..^1])
            .Select(parameter => MapSignatureType(target, parameter));
        return returnType + "(" + string.Join(",", parameters) + ")";
    }

    private static string MapSignatureType(MetadataReader target, string type)
    {
        foreach (var suffix in new[] { "[]", "&", "*" })
        {
            if (!type.EndsWith(suffix, StringComparison.Ordinal))
                continue;
            var element = type[..^suffix.Length];
            return MapSignatureType(target, element) + suffix;
        }

        var genericOpen = type.IndexOf('<');
        if (genericOpen > 0 && type.EndsWith('>'))
        {
            var genericName = type[..genericOpen];
            var mappedName = FindMappedTypeName(target, genericName) ?? genericName;
            var arguments = SplitGenericArguments(type[(genericOpen + 1)..^1])
                .Select(argument => MapSignatureType(target, argument));
            return mappedName + "<" + string.Join(",", arguments) + ">";
        }
        return FindMappedTypeName(target, type) ?? type;
    }

    private static string MethodDefinitionKey(MetadataReader reader, MethodDefinitionHandle handle)
    {
        var method = reader.GetMethodDefinition(handle);
        var signature = method.DecodeSignature(NameProvider.Instance, null);
        return reader.GetString(method.Name) + ":" + signature.ReturnType + "("
            + string.Join(",", signature.ParameterTypes) + ")";
    }

    private static string? TypeName(MetadataReader reader, EntityHandle handle)
    {
        return handle.Kind switch
        {
            HandleKind.TypeDefinition => MetadataNames.TypeDefName(reader, reader.GetTypeDefinition((TypeDefinitionHandle)handle)),
            HandleKind.TypeReference => MetadataNames.TypeRefName(reader, reader.GetTypeReference((TypeReferenceHandle)handle)),
            HandleKind.TypeSpecification => reader.GetTypeSpecification((TypeSpecificationHandle)handle)
                .DecodeSignature(NameProvider.Instance, null),
            _ => null
        };
    }

    private static TypeDefinitionHandle FindType(MetadataReader reader, string fullName)
    {
        foreach (var handle in reader.TypeDefinitions)
            if (MetadataNames.TypeDefName(reader, reader.GetTypeDefinition(handle)) == fullName)
                return handle;
        return default;
    }

    private static string? FindMappedTypeName(MetadataReader reader, string originalTypeName)
    {
        foreach (var typeHandle in reader.TypeDefinitions)
        {
            var type = reader.GetTypeDefinition(typeHandle);
            foreach (var attributeHandle in type.GetCustomAttributes())
            {
                var attribute = reader.GetCustomAttribute(attributeHandle);
                var attributeType = attribute.Constructor.Kind switch
                {
                    HandleKind.MemberReference => TypeName(reader,
                        reader.GetMemberReference((MemberReferenceHandle)attribute.Constructor).Parent),
                    HandleKind.MethodDefinition => TypeName(reader,
                        reader.GetMethodDefinition((MethodDefinitionHandle)attribute.Constructor).GetDeclaringType()),
                    _ => null
                };
                if (attributeType != "Quire.Mirror.MirrorOfAttribute")
                    continue;
                var blob = reader.GetBlobReader(attribute.Value);
                if (blob.ReadUInt16() != 1)
                    continue;
                var value = blob.ReadSerializedString();
                if (value is null)
                    continue;
                var separator = value.LastIndexOf(',');
                if (separator <= 0 || value[..separator].Trim() != originalTypeName)
                    continue;
                return MetadataNames.TypeDefName(reader, type);
            }
        }
        return null;
    }

    private static string? MapParentType(MetadataReader reader, string originalTypeName)
    {
        var direct = FindMappedTypeName(reader, originalTypeName);
        if (direct is not null)
            return direct;
        var genericStart = originalTypeName.IndexOf('<');
        if (genericStart <= 0)
            return originalTypeName;
        var mappedBase = FindMappedTypeName(reader, originalTypeName[..genericStart]);
        return mappedBase is null ? originalTypeName : mappedBase + originalTypeName[genericStart..];
    }

    private static TypeReferenceHandle FindTypeReference(MetadataReader reader, string fullName)
    {
        for (var row = 1; row <= reader.GetTableRowCount(TableIndex.TypeRef); row++)
        {
            var handle = MetadataTokens.TypeReferenceHandle(row);
            if (MetadataNames.TypeRefName(reader, reader.GetTypeReference(handle)) == fullName)
                return handle;
        }
        return default;
    }

    private static TypeSpecificationHandle FindTypeSpecification(MetadataReader reader, string typeName)
    {
        for (var row = 1; row <= reader.GetTableRowCount(TableIndex.TypeSpec); row++)
        {
            var handle = MetadataTokens.TypeSpecificationHandle(row);
            if (reader.GetTypeSpecification(handle).DecodeSignature(NameProvider.Instance, null) == typeName)
                return handle;
        }
        return default;
    }

    private static IEnumerable<MemberReferenceHandle> EnumerateMemberReferences(MetadataReader reader)
    {
        for (var row = 1; row <= reader.GetTableRowCount(TableIndex.MemberRef); row++)
            yield return MetadataTokens.MemberReferenceHandle(row);
    }

    private static IEnumerable<StandaloneSignatureHandle> EnumerateStandaloneSignatures(MetadataReader reader)
    {
        for (var row = 1; row <= reader.GetTableRowCount(TableIndex.StandAloneSig); row++)
            yield return MetadataTokens.StandaloneSignatureHandle(row);
    }

    private static TypeDefinitionHandle FindType(MetadataReader reader, string fullName, bool _)
        => FindType(reader, fullName);

    private static MethodDefinitionHandle FindMethodAt(MetadataReader reader,
        TypeDefinitionHandle typeHandle, string name, int parameterCount, int index)
    {
        if (typeHandle.IsNil)
            return default;
        var matches = reader.GetTypeDefinition(typeHandle).GetMethods()
            .Where(handle => reader.GetString(reader.GetMethodDefinition(handle).Name) == name
                && reader.GetMethodDefinition(handle).DecodeSignature(NameProvider.Instance, null).ParameterTypes.Length == parameterCount)
            .ToArray();
        return index < matches.Length ? matches[index] : default;
    }

    private static MethodDefinitionHandle FindMatchingCandidateMethod(MetadataReader source,
        MethodDefinitionHandle sourceMethod, MetadataReader target, TypeDefinitionHandle targetType,
        string name, int parameterCount)
    {
        var sourceSignature = source.GetMethodDefinition(sourceMethod).DecodeSignature(NameProvider.Instance, null);
        var mappedSignature = MapMethodReferenceKey(target,
            sourceSignature.ReturnType + "(" + string.Join(",", sourceSignature.ParameterTypes) + ")");
        var matches = target.GetTypeDefinition(targetType).GetMethods()
            .Where(handle => target.GetString(target.GetMethodDefinition(handle).Name) == name)
            .Where(handle =>
            {
                var signature = target.GetMethodDefinition(handle).DecodeSignature(NameProvider.Instance, null);
                return signature.ParameterTypes.Length == parameterCount
                    && signature.ReturnType + "(" + string.Join(",", signature.ParameterTypes) + ")" == mappedSignature;
            })
            .ToArray();
        return matches.Length == 1 ? matches[0] : default;
    }

    private static int RvaToOffset(PEHeaders headers, int rva)
    {
        foreach (var section in headers.SectionHeaders)
        {
            var length = Math.Max(section.VirtualSize, section.SizeOfRawData);
            if (rva >= section.VirtualAddress && rva < section.VirtualAddress + length)
                return section.PointerToRawData + rva - section.VirtualAddress;
        }
        throw new InvalidDataException($"RVA 0x{rva:x} is not in a PE section");
    }

    private static int GetBodyCapacity(MetadataReader reader, PEHeaders headers, int rva)
    {
        var nextRva = reader.MethodDefinitions.Select(reader.GetMethodDefinition)
            .Select(method => method.RelativeVirtualAddress)
            .Where(other => other > rva)
            .DefaultIfEmpty(headers.PEHeader!.SizeOfImage)
            .Min();
        return nextRva - rva;
    }

    private static int GetMethodBodySize(byte[] bytes, int offset)
    {
        var first = bytes[offset];
        if ((first & 0x3) == 0x2)
            return 1 + (first >> 2);
        var flags = BitConverter.ToUInt16(bytes, offset);
        if ((flags & 0x3) != 0x3)
            throw new BadImageFormatException("invalid method body header");
        var headerSize = ((flags >> 12) & 0xf) * 4;
        var codeSize = checked((int)BitConverter.ToUInt32(bytes, offset + 4));
        var bodySize = headerSize + codeSize;
        if ((flags & 0x8) == 0)
            return bodySize;

        var sectionOffset = Align4(bodySize);
        byte sectionKind;
        do
        {
            sectionKind = bytes[offset + sectionOffset];
            var fatSection = (sectionKind & 0x40) != 0;
            var moreSections = (sectionKind & 0x80) != 0;
            var sectionSize = fatSection
                ? bytes[offset + sectionOffset + 1]
                    | (bytes[offset + sectionOffset + 2] << 8)
                    | (bytes[offset + sectionOffset + 3] << 16)
                : bytes[offset + sectionOffset + 1];
            if (sectionSize < 4 || (sectionKind & 0x3f) == 0)
                throw new BadImageFormatException("invalid method data section");
            sectionOffset += sectionSize;
            if (!moreSections)
                return sectionOffset;
            sectionOffset = Align4(sectionOffset);
        }
        while ((sectionKind & 0x80) != 0);

        throw new BadImageFormatException("invalid method data section chain");
    }

    private static int Align4(int value) => (value + 3) & ~3;

    private static int GetCodeOffset(byte[] body)
    {
        var first = body[0];
        return (first & 0x3) == 0x2 ? 1 : ((BitConverter.ToUInt16(body, 0) >> 12) & 0xf) * 4;
    }

    private static int GetCodeSize(byte[] body)
    {
        var first = body[0];
        return (first & 0x3) == 0x2 ? first >> 2 : checked((int)BitConverter.ToUInt32(body, 4));
    }

    private static int GetOperandSize(OperandType operandType, ReadOnlySpan<byte> code, int position)
    {
        return operandType switch
        {
            OperandType.InlineNone => 0,
            OperandType.ShortInlineBrTarget or OperandType.ShortInlineI or OperandType.ShortInlineVar => 1,
            OperandType.InlineVar => 2,
            OperandType.InlineI or OperandType.InlineField or OperandType.InlineMethod or OperandType.InlineSig
                or OperandType.InlineString or OperandType.InlineTok or OperandType.InlineType or OperandType.ShortInlineR
                or OperandType.InlineBrTarget => 4,
            OperandType.InlineI8 or OperandType.InlineR => 8,
            OperandType.InlineSwitch => position + 4 <= code.Length
                ? 4 + checked((int)BitConverter.ToUInt32(code.Slice(position, 4))) * 4 : -1,
            _ => -1
        };
    }
}
