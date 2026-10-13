using System.Collections.Immutable;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;

namespace Quire.Tools.MirrorGen;

/// <summary>
/// Reads an assembly's metadata tables into <see cref="TypeModel"/>s. The file is parsed as data; nothing is loaded or
/// executed, and method bodies are never read.
/// </summary>
public sealed class MetadataLoader
{
    public static AssemblyModel? Load(string path)
    {
        using var stream = File.OpenRead(path);
        using var pe = new PEReader(stream);
        if (!pe.HasMetadata) return null;
        var md = pe.GetMetadataReader();
        if (!md.IsAssembly) return null;
        return new MetadataLoader(md).Read(path);
    }

    private readonly MetadataReader _md;
    private readonly SigProvider _sigs;
    private readonly string _asm;

    private MetadataLoader(MetadataReader md)
    {
        _md = md;
        _asm = md.GetString(md.GetAssemblyDefinition().Name);
        _sigs = new SigProvider(_asm);
    }

    private AssemblyModel Read(string path)
    {
        var references = _md.AssemblyReferences.Select(h => _md.GetString(_md.GetAssemblyReference(h).Name)).ToList();
        var model = new AssemblyModel(_asm, path, references);
        foreach (var ca in _md.GetAssemblyDefinition().GetCustomAttributes())
        {
            var attr = _md.GetCustomAttribute(ca);
            string? name = AttributeName(attr);
            if (name == "System.Runtime.CompilerServices.InternalsVisibleToAttribute"
                && attr.DecodeValue(_sigs).FixedArguments[0].Value is string friend)
                model.InternalsVisibleTo.Add(friend.Split(',')[0].Trim());
            else if (name == "System.Reflection.AssemblyMetadataAttribute"
                && attr.DecodeValue(_sigs).FixedArguments is [{ Value: string key }, { Value: var value }])
                model.Metadata[key] = value as string;
        }
        var impls = MethodImpls();
        foreach (var h in _md.TypeDefinitions)
        {
            var td = _md.GetTypeDefinition(h);
            if (_md.GetString(td.Name) == "<Module>") continue;
            model.Types.Add(ReadType(h, td, impls));
        }
        return model;
    }

    /// <summary>Explicit interface implementations: method body → the interface methods it implements.</summary>
    private Dictionary<MethodDefinitionHandle, List<(TypeSig, string)>> MethodImpls()
    {
        var map = new Dictionary<MethodDefinitionHandle, List<(TypeSig, string)>>();
        foreach (var h in _md.TypeDefinitions)
            foreach (var ih in _md.GetTypeDefinition(h).GetMethodImplementations())
            {
                var impl = _md.GetMethodImplementation(ih);
                if (impl.MethodBody.Kind != HandleKind.MethodDefinition) continue;
                var (owner, name) = MethodOwner(impl.MethodDeclaration);
                if (owner is null) continue;
                var body = (MethodDefinitionHandle)impl.MethodBody;
                if (!map.TryGetValue(body, out var list)) map[body] = list = [];
                list.Add((owner, name));
            }
        return map;
    }

    private (TypeSig?, string) MethodOwner(EntityHandle method)
    {
        switch (method.Kind)
        {
            case HandleKind.MethodDefinition:
                var def = _md.GetMethodDefinition((MethodDefinitionHandle)method);
                return (_sigs.GetTypeFromDefinition(_md, def.GetDeclaringType(), 0), _md.GetString(def.Name));
            case HandleKind.MemberReference:
                var mr = _md.GetMemberReference((MemberReferenceHandle)method);
                return (DecodeEntity(mr.Parent), _md.GetString(mr.Name));
            default:
                return (null, "");
        }
    }

    private TypeSig? DecodeEntity(EntityHandle h) => h.Kind switch
    {
        HandleKind.TypeDefinition => _sigs.GetTypeFromDefinition(_md, (TypeDefinitionHandle)h, 0),
        HandleKind.TypeReference => _sigs.GetTypeFromReference(_md, (TypeReferenceHandle)h, 0),
        HandleKind.TypeSpecification => _md.GetTypeSpecification((TypeSpecificationHandle)h).DecodeSignature(_sigs, null),
        _ => null,
    };

    private TypeModel ReadType(TypeDefinitionHandle h, TypeDefinition td,
        Dictionary<MethodDefinitionHandle, List<(TypeSig, string)>> impls)
    {
        var attrs = td.Attributes;
        string fullName = SigProvider.FullName(_md, h);
        var baseType = td.BaseType.IsNil ? null : DecodeEntity(td.BaseType);
        string? baseName = (baseType as NamedSig)?.Name;
        var kind = (attrs & TypeAttributes.Interface) != 0 ? TypeKind.Interface
            : baseName == "System.Enum" ? TypeKind.Enum
            : baseName == "System.ValueType" && fullName != "System.Enum" ? TypeKind.Struct
            : baseName == "System.MulticastDelegate" ? TypeKind.Delegate
            : TypeKind.Class;

        var outer = h;
        while (_md.GetTypeDefinition(outer).GetDeclaringType() is { IsNil: false } d) outer = d;

        var typeAttrs = AttributeNames(td.GetCustomAttributes());
        string? defaultMember = null, mirrorOf = null;
        foreach (var ca in td.GetCustomAttributes())
        {
            string? an = AttributeName(_md.GetCustomAttribute(ca));
            if (an == "System.Reflection.DefaultMemberAttribute")
                defaultMember = _md.GetCustomAttribute(ca).DecodeValue(_sigs).FixedArguments[0].Value as string;
            else if (an == "Quire.Mirror.MirrorOfAttribute")
                mirrorOf = _md.GetCustomAttribute(ca).DecodeValue(_sigs).FixedArguments[0].Value as string;
        }

        var layout = td.GetLayout();
        var type = new TypeModel
        {
            Assembly = _asm,
            FullName = fullName,
            Namespace = _md.GetString(_md.GetTypeDefinition(outer).Namespace),
            Name = _md.GetString(td.Name),
            DeclaringType = td.GetDeclaringType().IsNil ? null : SigProvider.FullName(_md, td.GetDeclaringType()),
            Kind = kind,
            Access = TypeAccess(attrs),
            Abstract = (attrs & TypeAttributes.Abstract) != 0,
            Sealed = (attrs & TypeAttributes.Sealed) != 0,
            CompilerGenerated = typeAttrs.Contains("System.Runtime.CompilerServices.CompilerGeneratedAttribute"),
            GenericParams = GenericParams(td.GetGenericParameters()),
            BaseType = kind is TypeKind.Class ? baseType : null,
            Interfaces = [.. td.GetInterfaceImplementations().Select(i => DecodeEntity(_md.GetInterfaceImplementation(i).Interface)!)],
            IsFlags = typeAttrs.Contains("System.FlagsAttribute"),
            IsReadOnly = typeAttrs.Contains("System.Runtime.CompilerServices.IsReadOnlyAttribute"),
            IsByRefLike = typeAttrs.Contains("System.Runtime.CompilerServices.IsByRefLikeAttribute"),
            DefaultMember = defaultMember,
            NativeSize = layout.IsDefault ? null : layout.Size,
            CustomAttributes = typeAttrs,
            MirrorOf = mirrorOf,
        };

        var methods = new Dictionary<MethodDefinitionHandle, MethodModel>();
        foreach (var mh in td.GetMethods())
        {
            var m = ReadMethod(mh, impls);
            methods[mh] = m;
        }

        var accessors = new HashSet<MethodDefinitionHandle>();
        foreach (var ph in td.GetProperties())
        {
            var p = _md.GetPropertyDefinition(ph);
            var a = p.GetAccessors();
            accessors.Add(a.Getter); accessors.Add(a.Setter);
            var sig = p.DecodeSignature(_sigs, null);
            var getter = a.Getter.IsNil ? null : methods[a.Getter];
            var setter = a.Setter.IsNil ? null : methods[a.Setter];
            var index = (getter?.Params ?? setter?.Params.RemoveAt(setter.Params.Length - 1) ?? []);
            type.Properties.Add(new PropertyModel(_md.GetString(p.Name), sig.ReturnType, getter, setter, index)
                { Status = Status(p.GetCustomAttributes(), out var pe), Evidence = pe });
        }
        foreach (var eh in td.GetEvents())
        {
            var e = _md.GetEventDefinition(eh);
            var a = e.GetAccessors();
            accessors.Add(a.Adder); accessors.Add(a.Remover);
            type.Events.Add(new EventModel(_md.GetString(e.Name), DecodeEntity(e.Type)!,
                a.Adder.IsNil ? null : methods[a.Adder], a.Remover.IsNil ? null : methods[a.Remover])
                { Status = Status(e.GetCustomAttributes(), out var ee), Evidence = ee });
        }
        // Obfuscated assemblies can lose property and event rows while keeping their special-name accessors.
        // Rebuild them, so the accessors are declared as the properties and events they were written as.
        var orphans = methods.Where(kv => !accessors.Contains(kv.Key) && kv.Value.SpecialName).ToList();
        foreach (var group in orphans.Select(kv => (kv.Key, kv.Value, Split: AccessorName(kv.Value.Name)))
                     .Where(x => x.Split.Kind != null).GroupBy(x => (x.Split.Kind is "get" or "set", x.Split.Member)))
        {
            var get = group.FirstOrDefault(x => x.Split.Kind is "get" or "add");
            var set = group.FirstOrDefault(x => x.Split.Kind is "set" or "remove");
            if (group.Key.Item1)
            {
                var g = get.Value; var st = set.Value;
                var propType = g?.Return ?? st!.Params[^1].Type;
                var index = g?.Params ?? st!.Params.RemoveAt(st.Params.Length - 1);
                type.Properties.Add(new PropertyModel(group.Key.Member, propType, g, st, index));
            }
            else if (get.Value is { Params.Length: 1 } add && set.Value != null)
                type.Events.Add(new EventModel(group.Key.Member, add.Params[0].Type, add, set.Value));
            else continue;
            foreach (var x in group) accessors.Add(x.Key);
        }

        foreach (var (mh, m) in methods)
            if (!accessors.Contains(mh)) type.Methods.Add(m);

        foreach (var fh in td.GetFields())
        {
            var f = _md.GetFieldDefinition(fh);
            var fa = f.Attributes;
            string name = _md.GetString(f.Name);
            if (kind == TypeKind.Enum && (fa & FieldAttributes.Static) == 0)
            {
                type = WithUnderlying(type, f.DecodeSignature(_sigs, null));
                continue;
            }
            object? value = (fa & FieldAttributes.Literal) != 0 && !f.GetDefaultValue().IsNil ? ReadConstant(f.GetDefaultValue()) : null;
            type.Fields.Add(new FieldModel(name, f.DecodeSignature(_sigs, null), FieldAccess(fa),
                (fa & FieldAttributes.Static) != 0, (fa & FieldAttributes.InitOnly) != 0, (fa & FieldAttributes.Literal) != 0, value)
                { Status = Status(f.GetCustomAttributes(), out var fe), Evidence = fe });
        }
        return type;
    }

    /// <summary>"get_X" → ("get", "X"); "Ns.IFoo.set_X" → ("set", "Ns.IFoo.X").</summary>
    private static (string? Kind, string Member) AccessorName(string name)
    {
        int dot = name.LastIndexOf('.');
        string prefix = dot < 0 ? "" : name[..(dot + 1)], last = name[(dot + 1)..];
        foreach (var kind in new[] { "get", "set", "add", "remove" })
            if (last.StartsWith(kind + "_") && last.Length > kind.Length + 1) return (kind, prefix + last[(kind.Length + 1)..]);
        return (null, name);
    }

    private static TypeModel WithUnderlying(TypeModel t, TypeSig underlying)
    {
        var copy = new TypeModel
        {
            Assembly = t.Assembly, FullName = t.FullName, Namespace = t.Namespace, Name = t.Name, DeclaringType = t.DeclaringType,
            Kind = t.Kind, Access = t.Access, Abstract = t.Abstract, Sealed = t.Sealed, CompilerGenerated = t.CompilerGenerated,
            GenericParams = t.GenericParams, BaseType = t.BaseType, Interfaces = t.Interfaces, IsFlags = t.IsFlags,
            DefaultMember = t.DefaultMember, NativeSize = t.NativeSize, IsReadOnly = t.IsReadOnly, IsByRefLike = t.IsByRefLike,
            CustomAttributes = t.CustomAttributes, MirrorOf = t.MirrorOf,
            EnumUnderlying = ((NamedSig)underlying).Name,
        };
        copy.Fields.AddRange(t.Fields); copy.Methods.AddRange(t.Methods);
        copy.Properties.AddRange(t.Properties); copy.Events.AddRange(t.Events);
        return copy;
    }

    private MethodModel ReadMethod(MethodDefinitionHandle mh, Dictionary<MethodDefinitionHandle, List<(TypeSig, string)>> impls)
    {
        var m = _md.GetMethodDefinition(mh);
        var a = m.Attributes;
        _sigs.InitOnly = false;
        var sig = m.DecodeSignature(_sigs, null);
        bool initOnly = _sigs.InitOnly;

        var names = new string[sig.ParameterTypes.Length];
        var flags = new ParameterAttributes[sig.ParameterTypes.Length];
        var defaults = new object?[sig.ParameterTypes.Length];
        var hasDefault = new bool[sig.ParameterTypes.Length];
        var isParams = new bool[sig.ParameterTypes.Length];
        foreach (var ph in m.GetParameters())
        {
            var p = _md.GetParameter(ph);
            if (p.SequenceNumber == 0 || p.SequenceNumber > names.Length) continue;
            int i = p.SequenceNumber - 1;
            names[i] = _md.GetString(p.Name);
            flags[i] = p.Attributes;
            if ((p.Attributes & ParameterAttributes.HasDefault) != 0)
            {
                hasDefault[i] = true;
                defaults[i] = p.GetDefaultValue().IsNil ? null : ReadConstant(p.GetDefaultValue());
            }
            else if ((p.Attributes & ParameterAttributes.Optional) != 0)
                hasDefault[i] = false;
            isParams[i] = AttributeNames(p.GetCustomAttributes()).Contains("System.ParamArrayAttribute");
        }

        var ps = ImmutableArray.CreateBuilder<ParamModel>(names.Length);
        for (int i = 0; i < names.Length; i++)
        {
            var t = sig.ParameterTypes[i];
            bool isIn = t is ByRefSig { ReadOnly: true };
            bool isOut = t is ByRefSig && !isIn && (flags[i] & ParameterAttributes.Out) != 0 && (flags[i] & ParameterAttributes.In) == 0;
            ps.Add(new ParamModel(string.IsNullOrEmpty(names[i]) ? $"arg{i}" : names[i], t, isIn, isOut, isParams[i], hasDefault[i], defaults[i]));
        }

        var methodAttrs = AttributeNames(m.GetCustomAttributes());
        return new MethodModel(
            _md.GetString(m.Name), MethodAccess(a),
            (a & MethodAttributes.Static) != 0, (a & MethodAttributes.Abstract) != 0, (a & MethodAttributes.Virtual) != 0,
            (a & MethodAttributes.NewSlot) != 0, (a & MethodAttributes.Final) != 0, (a & MethodAttributes.SpecialName) != 0,
            sig.ReturnType, ps.MoveToImmutable(), GenericParams(m.GetGenericParameters()),
            impls.TryGetValue(mh, out var list) ? [.. list] : [],
            (a & MethodAttributes.PinvokeImpl) != 0 || (m.ImplAttributes & MethodImplAttributes.InternalCall) != 0,
            methodAttrs.Contains("System.Runtime.CompilerServices.ExtensionAttribute"),
            initOnly) { Status = Status(m.GetCustomAttributes(), out var me), Evidence = me };
    }

    /// <summary>The member's §3c status from its Quire.Mirror attributes, with the attribute's text argument.</summary>
    private MemberStatus Status(CustomAttributeHandleCollection handles, out string? evidence)
    {
        evidence = null;
        foreach (var h in handles)
        {
            var ca = _md.GetCustomAttribute(h);
            var status = AttributeName(ca) switch
            {
                "Quire.Mirror.MirrorStubAttribute" => MemberStatus.Stub,
                "Quire.Mirror.MirrorBlockedAttribute" => MemberStatus.Blocked,
                "Quire.Mirror.MirrorAnalyzedAttribute" => MemberStatus.Analyzed,
                "Quire.Mirror.MirrorTestedAttribute" => MemberStatus.Tested,
                "Quire.Mirror.MirrorVerifiedAttribute" => MemberStatus.Verified,
                "Quire.Mirror.MirrorDeviationAttribute" => MemberStatus.Deviation,
                "Quire.Mirror.MirrorDocumentedAttribute" => MemberStatus.Documented,
                "Quire.Mirror.QuireOnlyAttribute" => MemberStatus.QuireOnly,
                _ => MemberStatus.Unclassified,
            };
            if (status == MemberStatus.Unclassified) continue;
            if (status is not (MemberStatus.Stub or MemberStatus.QuireOnly))
                evidence = ca.DecodeValue(_sigs).FixedArguments[0].Value as string;
            return status;
        }
        return MemberStatus.Unclassified;
    }

    private ImmutableArray<GenericParam> GenericParams(GenericParameterHandleCollection handles)
    {
        var b = ImmutableArray.CreateBuilder<GenericParam>();
        foreach (var gh in handles)
        {
            var g = _md.GetGenericParameter(gh);
            var a = g.Attributes;
            var cons = g.GetConstraints().Select(c => DecodeEntity(_md.GetGenericParameterConstraint(c).Type)!)
                .Where(c => c is not NamedSig { Name: "System.ValueType" }).ToImmutableArray();
            int variance = (a & GenericParameterAttributes.Covariant) != 0 ? 1 : (a & GenericParameterAttributes.Contravariant) != 0 ? -1 : 0;
            b.Add(new GenericParam(_md.GetString(g.Name), variance,
                (a & GenericParameterAttributes.ReferenceTypeConstraint) != 0,
                (a & GenericParameterAttributes.NotNullableValueTypeConstraint) != 0,
                (a & GenericParameterAttributes.DefaultConstructorConstraint) != 0
                    && (a & GenericParameterAttributes.NotNullableValueTypeConstraint) == 0,
                cons));
        }
        return b.ToImmutable();
    }

    private HashSet<string> AttributeNames(CustomAttributeHandleCollection handles) =>
        [.. handles.Select(h => AttributeName(_md.GetCustomAttribute(h))).OfType<string>()];

    private string? AttributeName(CustomAttribute ca)
    {
        switch (ca.Constructor.Kind)
        {
            case HandleKind.MemberReference:
                var parent = _md.GetMemberReference((MemberReferenceHandle)ca.Constructor).Parent;
                return parent.Kind switch
                {
                    HandleKind.TypeReference => SigProvider.FullName(_md, (TypeReferenceHandle)parent),
                    HandleKind.TypeDefinition => SigProvider.FullName(_md, (TypeDefinitionHandle)parent),
                    _ => null,
                };
            case HandleKind.MethodDefinition:
                return SigProvider.FullName(_md, _md.GetMethodDefinition((MethodDefinitionHandle)ca.Constructor).GetDeclaringType());
            default:
                return null;
        }
    }

    private object? ReadConstant(ConstantHandle h)
    {
        var c = _md.GetConstant(h);
        var r = _md.GetBlobReader(c.Value);
        return c.TypeCode switch
        {
            ConstantTypeCode.Boolean => r.ReadBoolean(),
            ConstantTypeCode.Char => r.ReadChar(),
            ConstantTypeCode.SByte => r.ReadSByte(),
            ConstantTypeCode.Byte => r.ReadByte(),
            ConstantTypeCode.Int16 => r.ReadInt16(),
            ConstantTypeCode.UInt16 => r.ReadUInt16(),
            ConstantTypeCode.Int32 => r.ReadInt32(),
            ConstantTypeCode.UInt32 => r.ReadUInt32(),
            ConstantTypeCode.Int64 => r.ReadInt64(),
            ConstantTypeCode.UInt64 => r.ReadUInt64(),
            ConstantTypeCode.Single => r.ReadSingle(),
            ConstantTypeCode.Double => r.ReadDouble(),
            ConstantTypeCode.String => r.ReadUTF16(r.Length),
            _ => null,
        };
    }

    private static Access TypeAccess(TypeAttributes a) => (a & TypeAttributes.VisibilityMask) switch
    {
        TypeAttributes.Public or TypeAttributes.NestedPublic => Access.Public,
        TypeAttributes.NotPublic or TypeAttributes.NestedAssembly => Access.Internal,
        TypeAttributes.NestedFamily => Access.Protected,
        TypeAttributes.NestedFamORAssem => Access.ProtectedInternal,
        TypeAttributes.NestedFamANDAssem => Access.PrivateProtected,
        _ => Access.Private,
    };

    private static Access MethodAccess(MethodAttributes a) => (a & MethodAttributes.MemberAccessMask) switch
    {
        MethodAttributes.Public => Access.Public,
        MethodAttributes.Assembly => Access.Internal,
        MethodAttributes.Family => Access.Protected,
        MethodAttributes.FamORAssem => Access.ProtectedInternal,
        MethodAttributes.FamANDAssem => Access.PrivateProtected,
        _ => Access.Private,
    };

    private static Access FieldAccess(FieldAttributes a) => (a & FieldAttributes.FieldAccessMask) switch
    {
        FieldAttributes.Public => Access.Public,
        FieldAttributes.Assembly => Access.Internal,
        FieldAttributes.Family => Access.Protected,
        FieldAttributes.FamORAssem => Access.ProtectedInternal,
        FieldAttributes.FamANDAssem => Access.PrivateProtected,
        _ => Access.Private,
    };
}

public sealed class AssemblyModel(string name, string path, List<string> references)
{
    public string Name { get; } = name;
    public string Path { get; } = path;
    public List<string> References { get; } = references;
    public List<TypeModel> Types { get; } = [];
    public List<string> InternalsVisibleTo { get; } = [];
    public Dictionary<string, string?> Metadata { get; } = [];
}

/// <summary>Decodes signature blobs into <see cref="TypeSig"/>s.</summary>
internal sealed class SigProvider(string asm)
    : ISignatureTypeProvider<TypeSig, object?>, ICustomAttributeTypeProvider<TypeSig>
{
    /// <summary>Set when the last decoded signature carried <c>modreq(IsExternalInit)</c>: an <c>init</c> accessor.</summary>
    public bool InitOnly { get; set; }

    private static NamedSig Corlib(string name) => new("System.Runtime", name);

    public TypeSig GetPrimitiveType(PrimitiveTypeCode code) => Corlib(code switch
    {
        PrimitiveTypeCode.Boolean => "System.Boolean",
        PrimitiveTypeCode.Byte => "System.Byte",
        PrimitiveTypeCode.SByte => "System.SByte",
        PrimitiveTypeCode.Char => "System.Char",
        PrimitiveTypeCode.Int16 => "System.Int16",
        PrimitiveTypeCode.UInt16 => "System.UInt16",
        PrimitiveTypeCode.Int32 => "System.Int32",
        PrimitiveTypeCode.UInt32 => "System.UInt32",
        PrimitiveTypeCode.Int64 => "System.Int64",
        PrimitiveTypeCode.UInt64 => "System.UInt64",
        PrimitiveTypeCode.Single => "System.Single",
        PrimitiveTypeCode.Double => "System.Double",
        PrimitiveTypeCode.IntPtr => "System.IntPtr",
        PrimitiveTypeCode.UIntPtr => "System.UIntPtr",
        PrimitiveTypeCode.Object => "System.Object",
        PrimitiveTypeCode.String => "System.String",
        PrimitiveTypeCode.TypedReference => "System.TypedReference",
        PrimitiveTypeCode.Void => "System.Void",
        _ => throw new BadImageFormatException($"primitive {code}"),
    });

    public TypeSig GetTypeFromDefinition(MetadataReader reader, TypeDefinitionHandle handle, byte rawTypeKind) =>
        new NamedSig(asm, FullName(reader, handle));

    public TypeSig GetTypeFromReference(MetadataReader reader, TypeReferenceHandle handle, byte rawTypeKind)
    {
        var tr = reader.GetTypeReference(handle);
        var scope = tr.ResolutionScope;
        while (scope.Kind == HandleKind.TypeReference) scope = reader.GetTypeReference((TypeReferenceHandle)scope).ResolutionScope;
        string owner = scope.Kind == HandleKind.AssemblyReference
            ? reader.GetString(reader.GetAssemblyReference((AssemblyReferenceHandle)scope).Name)
            : asm;
        return new NamedSig(owner, FullName(reader, handle));
    }

    public TypeSig GetTypeFromSpecification(MetadataReader reader, object? context, TypeSpecificationHandle handle, byte rawTypeKind) =>
        reader.GetTypeSpecification(handle).DecodeSignature(this, context);

    public TypeSig GetGenericInstantiation(TypeSig genericType, ImmutableArray<TypeSig> typeArguments) =>
        ((NamedSig)genericType) with { Args = typeArguments };

    public TypeSig GetArrayType(TypeSig elementType, ArrayShape shape) => new ArraySig(elementType, Math.Max(1, shape.Rank));
    public TypeSig GetSZArrayType(TypeSig elementType) => new ArraySig(elementType, 0);
    public TypeSig GetPointerType(TypeSig elementType) => new PointerSig(elementType);
    public TypeSig GetByReferenceType(TypeSig elementType) => new ByRefSig(elementType);
    public TypeSig GetGenericMethodParameter(object? context, int index) => new GenericParamSig(index, true);
    public TypeSig GetGenericTypeParameter(object? context, int index) => new GenericParamSig(index, false);
    public TypeSig GetPinnedType(TypeSig elementType) => elementType;
    public TypeSig GetFunctionPointerType(MethodSignature<TypeSig> signature) => new FunctionPointerSig();

    public TypeSig GetModifiedType(TypeSig modifier, TypeSig unmodifiedType, bool isRequired)
    {
        if (!isRequired) return unmodifiedType;
        switch ((modifier as NamedSig)?.Name)
        {
            case "System.Runtime.InteropServices.InAttribute" when unmodifiedType is ByRefSig r:
                return r with { ReadOnly = true };
            case "System.Runtime.CompilerServices.IsExternalInit":
                InitOnly = true;
                return unmodifiedType;
            default:
                return unmodifiedType;
        }
    }

    // ICustomAttributeTypeProvider
    public TypeSig GetSystemType() => Corlib("System.Type");
    public bool IsSystemType(TypeSig type) => type is NamedSig { Name: "System.Type" };
    public TypeSig GetTypeFromSerializedName(string name) => new NamedSig("", name.Split(',')[0]);
    public PrimitiveTypeCode GetUnderlyingEnumType(TypeSig type) => PrimitiveTypeCode.Int32;

    public static string FullName(MetadataReader md, TypeDefinitionHandle h)
    {
        var td = md.GetTypeDefinition(h);
        string name = md.GetString(td.Name);
        var decl = td.GetDeclaringType();
        if (!decl.IsNil) return FullName(md, decl) + "+" + name;
        string ns = md.GetString(td.Namespace);
        return ns.Length == 0 ? name : ns + "." + name;
    }

    public static string FullName(MetadataReader md, TypeReferenceHandle h)
    {
        var tr = md.GetTypeReference(h);
        string name = md.GetString(tr.Name);
        if (tr.ResolutionScope.Kind == HandleKind.TypeReference)
            return FullName(md, (TypeReferenceHandle)tr.ResolutionScope) + "+" + name;
        string ns = md.GetString(tr.Namespace);
        return ns.Length == 0 ? name : ns + "." + name;
    }
}
