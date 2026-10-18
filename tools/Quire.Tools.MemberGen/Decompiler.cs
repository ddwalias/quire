using System.Collections.Immutable;
using System.Globalization;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using Quire.Tools.MirrorGen;

/// <summary>A body this tool does not reconstruct; <see cref="Exception.Message"/> is the survey's category.</summary>
sealed class NotMechanical(string category) : Exception(category);

sealed record FieldRef(TypeSig Owner, string Name, TypeSig Type, bool Static)
{
    public TypeSig FieldType => (Owner as NamedSig)?.Args.Length > 0 ? Decompiler.Substitute(Type, ((NamedSig)Owner).Args) : Type;
}
/// <summary>A called method; <paramref name="Virtual"/> is false only for a definition known to be non-virtual (a
/// method of another assembly may be virtual).</summary>
sealed record MethodRef(TypeSig Owner, string Name, MethodSignature<TypeSig> Sig, ImmutableArray<TypeSig> TypeArgs, bool Virtual)
{
    public bool Instance => Sig.Header.IsInstance;
    public TypeSig ReturnType => Decompiler.Substitute(Sig.ReturnType, (Owner as NamedSig)?.Args ?? [], TypeArgs);
    public TypeSig ParameterType(int i) => Decompiler.Substitute(Sig.ParameterTypes[i], (Owner as NamedSig)?.Args ?? [], TypeArgs);
}

abstract record Expr
{
    public abstract TypeSig? Type { get; }
}
sealed record This(TypeSig Owner) : Expr { public override TypeSig? Type => Owner; }
sealed record Arg(int Index, TypeSig ArgType) : Expr { public override TypeSig? Type => ArgType; }
sealed record Const(object? Value, TypeSig? ConstType) : Expr { public override TypeSig? Type => ConstType; }
sealed record Field(Expr? Target, FieldRef F) : Expr { public override TypeSig? Type => F.FieldType; }
sealed record Call(Expr? Target, MethodRef M, Expr[] Args, bool Base) : Expr { public override TypeSig? Type => M.ReturnType; }
sealed record New(MethodRef Ctor, Expr[] Args) : Expr { public override TypeSig? Type => Ctor.Owner; }
sealed record NewArray(TypeSig Element, Expr Size) : Expr { public override TypeSig? Type => new ArraySig(Element, 0); }
sealed record Element(Expr Array, Expr Index) : Expr { public override TypeSig? Type => (Array.Type as ArraySig)?.Element; }
sealed record Length(Expr Array) : Expr { public override TypeSig? Type => Decompiler.Int32; }
sealed record Conversion(Expr Value, TypeSig To, bool Explicit) : Expr { public override TypeSig? Type => To; }
sealed record As(Expr Value, TypeSig To) : Expr { public override TypeSig? Type => To; }
sealed record Is(Expr Value, TypeSig To) : Expr { public override TypeSig? Type => Decompiler.Boolean; }
sealed record TypeOf(TypeSig Of) : Expr { public override TypeSig? Type => new NamedSig("System.Runtime", "System.Type"); }
sealed record Unary(string Op, Expr Value) : Expr { public override TypeSig? Type => Op == "!" ? Decompiler.Boolean : Value.Type; }
sealed record Binary(string Op, Expr Left, Expr Right) : Expr
{
    public override TypeSig? Type => Op is "==" or "!=" or "<" or ">" or "<=" or ">=" ? Decompiler.Boolean : Left.Type;
}

abstract record Statement;
sealed record Assign(Expr Target, Expr Value) : Statement;
sealed record Do(Expr Value) : Statement;
sealed record Return(Expr? Value) : Statement;
sealed record Throw(Expr Value) : Statement;
sealed record Initializer(bool Same, MethodRef Ctor, Expr[] Args) : Statement;

/// <summary>A straight-line body, decompiled: statements, with a constructor's base or this call first.</summary>
sealed record Body(List<Statement> Statements, TypeSig Owner, TypeSig Returns, ImmutableArray<TypeSig> Parameters, bool Static)
{
    /// <summary>The survey's name for the body's shape.</summary>
    public string Kind(bool constructor)
    {
        var rest = Statements.Where(s => s is not Initializer and not Return { Value: null }).ToList();
        static bool Simple(Expr e) => e is Arg or Const or TypeOf or This || e is Field { Target: null or This };
        if (constructor)
            return rest.Count == 0 ? "ctor: chain only" : rest.All(s => s is Assign { Value: var v } && Simple(v)) ? "ctor: store arguments"
                : "ctor: other straight-line";
        return rest switch
        {
            [] => "empty",
            [Return { Value: Const }] => "constant",
            [Throw { Value: New }] => "throw",
            [Return { Value: Field { Target: null or This } }] => "field get",
            [Assign { Target: Field { Target: null or This }, Value: var v }] when Simple(v) => "field set",
            [Return { Value: Call c }] when c.Args.All(Simple) && (c.Target == null || Simple(c.Target)) => "forward call",
            [Do { Value: Call c }] when c.Args.All(Simple) && (c.Target == null || Simple(c.Target)) => "forward call",
            [Return { Value: New n }] when n.Args.All(Simple) => "forward call",
            [Return { Value: As or Conversion or TypeOf }] => "cast or conversion",
            [Return] => "one expression",
            _ => "straight-line statements",
        };
    }
}

/// <summary>
/// Turns a branch-free, local-free Mathcad body into statements: stores, calls and one return, the bodies of
/// accessors, forwarding methods and constructors that store their arguments. Anything else throws
/// <see cref="NotMechanical"/>.
/// </summary>
static class Decompiler
{
    public static readonly TypeSig Int32 = new NamedSig("System.Runtime", "System.Int32");
    public static readonly TypeSig Boolean = new NamedSig("System.Runtime", "System.Boolean");
    private static readonly TypeSig Int64 = new NamedSig("System.Runtime", "System.Int64");
    private static readonly TypeSig Double = new NamedSig("System.Runtime", "System.Double");
    private static readonly TypeSig Single = new NamedSig("System.Runtime", "System.Single");
    private static readonly TypeSig String = new NamedSig("System.Runtime", "System.String");
    private static readonly TypeSig Object = new NamedSig("System.Runtime", "System.Object");

    private static readonly Dictionary<string, string> Operators = new()
    {
        ["add"] = "+", ["sub"] = "-", ["mul"] = "*", ["div"] = "/", ["rem"] = "%", ["and"] = "&", ["or"] = "|", ["xor"] = "^",
        ["shl"] = "<<", ["shr"] = ">>", ["shr.un"] = ">>", ["div.un"] = "/", ["rem.un"] = "%",
    };
    private static readonly Dictionary<string, string> Conversions = new()
    {
        ["conv.i1"] = "System.SByte", ["conv.i2"] = "System.Int16", ["conv.i4"] = "System.Int32", ["conv.i8"] = "System.Int64",
        ["conv.u1"] = "System.Byte", ["conv.u2"] = "System.UInt16", ["conv.u4"] = "System.UInt32", ["conv.u8"] = "System.UInt64",
        ["conv.r4"] = "System.Single", ["conv.r8"] = "System.Double",
    };

    public static Body Decompile(Artifact a, MethodDefinitionHandle handle)
    {
        var md = a.Metadata;
        var def = md.GetMethodDefinition(handle);
        if (def.RelativeVirtualAddress == 0) throw new NotMechanical("no body");
        var decoder = new SigProvider(a.Name);
        var sig = def.DecodeSignature(decoder, null);
        var owner = Owner(a, def.GetDeclaringType());
        bool isStatic = !sig.Header.IsInstance;
        var body = a.Pe.GetMethodBody(def.RelativeVirtualAddress);
        if (body.ExceptionRegions.Length > 0) throw new NotMechanical("exception handling");
        var code = Il.Read(body);
        var stack = new Stack<Expr>();
        var statements = new List<Statement>();
        Expr Pop() => stack.Count > 0 ? stack.Pop() : throw new NotMechanical("unbalanced stack");
        Expr[] PopArgs(int n) { var args = new Expr[n]; for (int i = n - 1; i >= 0; i--) args[i] = Pop(); return args; }
        void Statement(Statement s)
        {
            if (stack.Count > 0) throw new NotMechanical("values left on the stack across statements");
            statements.Add(s);
        }
        for (int i = 0; i < code.Count; i++)
        {
            var ins = code[i];
            string op = ins.Op.Name!;
            switch (op)
            {
                case "nop": break;
                case "ldarg":
                {
                    int n = (int)ins.Operand!;
                    stack.Push(!isStatic && n == 0 ? new This(owner) : new Arg(isStatic ? n : n - 1, sig.ParameterTypes[isStatic ? n : n - 1]));
                    if (stack.Peek() is Arg { ArgType: ByRefSig or PointerSig }) throw new NotMechanical("ref, out or pointer parameter");
                    break;
                }
                case "ldc.i4": stack.Push(new Const((int)ins.Operand!, Int32)); break;
                case "ldc.i8": stack.Push(new Const((long)ins.Operand!, Int64)); break;
                case "ldc.r8": stack.Push(new Const((double)ins.Operand!, Double)); break;
                case "ldc.r4": stack.Push(new Const((float)(double)ins.Operand!, Single)); break;
                case "ldnull": stack.Push(new Const(null, null)); break;
                case "ldstr": stack.Push(new Const(md.GetUserString((UserStringHandle)ins.Operand!), String)); break;
                case "ldfld": stack.Push(new Field(Pop(), Field(a, (EntityHandle)ins.Operand!, false))); break;
                case "ldsfld": stack.Push(new Field(null, Field(a, (EntityHandle)ins.Operand!, true))); break;
                case "stfld": { var v = Pop(); Statement(new Assign(new Field(Pop(), Field(a, (EntityHandle)ins.Operand!, false)), v)); break; }
                case "stsfld": { var v = Pop(); Statement(new Assign(new Field(null, Field(a, (EntityHandle)ins.Operand!, true)), v)); break; }
                case "call" or "callvirt":
                {
                    var m = Method(a, (EntityHandle)ins.Operand!);
                    if (m.Sig.Header.CallingConvention == SignatureCallingConvention.VarArgs) throw new NotMechanical("varargs");
                    var args = PopArgs(m.Sig.ParameterTypes.Length);
                    var target = m.Instance ? Pop() : null;
                    if (m.Name == ".ctor")
                    {
                        if (target is not This || statements.Count > 0) throw new NotMechanical("field initializers");
                        Statement(new Initializer(m.Owner.Equals(owner), m, args));
                        break;
                    }
                    // Type.GetTypeFromHandle(ldtoken T) is typeof(T).
                    if (m is { Name: "GetTypeFromHandle", Owner: NamedSig { Name: "System.Type" } } && args is [TypeOf t]) { stack.Push(t); break; }
                    // A non-virtual call to an inherited virtual method is `base.`; to a non-virtual one, a plain call.
                    var call = new Call(target, m, args, op == "call" && target is This && m.Virtual && !m.Owner.Equals(owner));
                    if (op == "call" && target != null && target is not This) throw new NotMechanical("non-virtual call on another object");
                    if (m.Sig.ReturnType is NamedSig { Name: "System.Void" }) Statement(new Do(call));
                    else stack.Push(call);
                    break;
                }
                case "newobj":
                {
                    var m = Method(a, (EntityHandle)ins.Operand!);
                    stack.Push(new New(m, PopArgs(m.Sig.ParameterTypes.Length)));
                    break;
                }
                case "ldtoken" when ins.Operand is EntityHandle { Kind: HandleKind.TypeDefinition or HandleKind.TypeReference or HandleKind.TypeSpecification } th:
                    stack.Push(new TypeOf(Type(a, th)));
                    break;
                case "isinst":
                {
                    var t = Type(a, (EntityHandle)ins.Operand!);
                    // x is T: isinst T; ldnull; cgt.un.
                    if (i + 2 < code.Count && code[i + 1].Op == OpCodes.Ldnull && code[i + 2].Op == OpCodes.Cgt_Un)
                    {
                        stack.Push(new Is(Pop(), t));
                        i += 2;
                    }
                    else stack.Push(new As(Pop(), t));
                    break;
                }
                case "castclass" or "unbox.any": stack.Push(new Conversion(Pop(), Type(a, (EntityHandle)ins.Operand!), true)); break;
                case "box": break;  // implicit in C#
                case "newarr": stack.Push(new NewArray(Type(a, (EntityHandle)ins.Operand!), Pop())); break;
                case "ldlen":
                    if (i + 1 >= code.Count || code[i + 1].Op != OpCodes.Conv_I4) throw new NotMechanical("native array length");
                    stack.Push(new Length(Pop()));
                    i++;
                    break;
                case var _ when op.StartsWith("ldelem") && op != "ldelema": { var index = Pop(); stack.Push(new Element(Pop(), index)); break; }
                case var _ when op.StartsWith("stelem"):
                {
                    var v = Pop();
                    var index = Pop();
                    Statement(new Assign(new Element(Pop(), index), v));
                    break;
                }
                case var _ when Operators.TryGetValue(op, out var symbol): { var r = Pop(); stack.Push(new Binary(symbol, Pop(), r)); break; }
                case "neg": stack.Push(new Unary("-", Pop())); break;
                case "not": stack.Push(new Unary("~", Pop())); break;
                case "ceq":
                {
                    var r = Pop();
                    var l = Pop();
                    stack.Push((l, r) switch
                    {
                        (Binary { Op: ">" } b, Const { Value: 0 }) => b with { Op = "<=" },
                        (Binary { Op: "<" } b, Const { Value: 0 }) => b with { Op = ">=" },
                        (Binary { Op: "==" } b, Const { Value: 0 }) => b with { Op = "!=" },
                        (Is s, Const { Value: 0 }) => new Unary("!", s),
                        (var x, Const { Value: 0 }) when x.Type is NamedSig { Name: "System.Boolean" } => new Unary("!", x),
                        _ => new Binary("==", l, r),
                    });
                    break;
                }
                case "cgt" or "cgt.un" or "clt" or "clt.un":
                {
                    var r = Pop();
                    var l = Pop();
                    if (op == "cgt.un" && r is Const { Value: null }) stack.Push(new Binary("!=", l, r));
                    else stack.Push(new Binary(op.StartsWith("cgt") ? ">" : "<", l, r));
                    break;
                }
                case var _ when Conversions.TryGetValue(op, out var to):
                {
                    var v = Pop();
                    var target = new NamedSig("System.Runtime", to);
                    stack.Push(v is Const { Value: int or long or double or float } c ? c with { ConstType = target } : new Conversion(v, target, false));
                    break;
                }
                case "pop": Statement(new Do(Pop() is var e && e is Call or New ? e : throw new NotMechanical("discarded value"))); break;
                case "ret":
                    if (i != code.Count - 1) throw new NotMechanical("branches");
                    Statement(new Return(stack.Count > 0 ? stack.Pop() : null));
                    break;
                case "dup": throw new NotMechanical("dup (compound assignment or initializer)");
                case "ldloc" or "stloc" or "ldloca": throw new NotMechanical("locals");
                case "ldftn" or "ldvirtftn": throw new NotMechanical("delegate creation");
                case "ldarga" or "ldflda" or "ldsflda" or "ldelema" or "initobj" or "constrained." or "ldobj" or "stobj":
                    throw new NotMechanical("addresses (value types)");
                case "throw":
                    if (i != code.Count - 1) throw new NotMechanical("branches");
                    Statement(new Throw(Pop()));
                    break;
                default:
                    if (ins.Op.FlowControl is FlowControl.Branch or FlowControl.Cond_Branch) throw new NotMechanical("branches");
                    throw new NotMechanical($"opcode {op}");
            }
        }
        if (statements.LastOrDefault() is not (Return or Throw)) throw new NotMechanical("does not end in a return");
        return new Body(statements, owner, sig.ReturnType, sig.ParameterTypes, isStatic);
    }

    public static bool CompilerGenerated(Artifact a, MethodDefinitionHandle handle) =>
        a.Metadata.GetMethodDefinition(handle).GetCustomAttributes().Any(h => AttributeName(a.Metadata, h) == "System.Runtime.CompilerServices.CompilerGeneratedAttribute");

    private static string AttributeName(MetadataReader md, CustomAttributeHandle h)
    {
        var ctor = md.GetCustomAttribute(h).Constructor;
        var parent = ctor.Kind == HandleKind.MemberReference ? md.GetMemberReference((MemberReferenceHandle)ctor).Parent
            : md.GetMethodDefinition((MethodDefinitionHandle)ctor).GetDeclaringType();
        return parent.Kind == HandleKind.TypeReference ? SigProvider.FullName(md, (TypeReferenceHandle)parent)
            : parent.Kind == HandleKind.TypeDefinition ? SigProvider.FullName(md, (TypeDefinitionHandle)parent) : "";
    }

    private static TypeSig Owner(Artifact a, TypeDefinitionHandle h)
    {
        var sig = new NamedSig(a.Name, SigProvider.FullName(a.Metadata, h));
        int arity = a.Metadata.GetTypeDefinition(h).GetGenericParameters().Count;
        return arity == 0 ? sig : sig with { Args = [.. Enumerable.Range(0, arity).Select(i => (TypeSig)new GenericParamSig(i, false))] };
    }

    public static TypeSig Type(Artifact a, EntityHandle h)
    {
        var decoder = new SigProvider(a.Name);
        var md = a.Metadata;
        return h.Kind switch
        {
            HandleKind.TypeDefinition => decoder.GetTypeFromDefinition(md, (TypeDefinitionHandle)h, 0),
            HandleKind.TypeReference => decoder.GetTypeFromReference(md, (TypeReferenceHandle)h, 0),
            HandleKind.TypeSpecification => md.GetTypeSpecification((TypeSpecificationHandle)h).DecodeSignature(decoder, null),
            _ => throw new NotMechanical("unexpected type token"),
        };
    }

    private static FieldRef Field(Artifact a, EntityHandle h, bool isStatic)
    {
        var md = a.Metadata;
        var decoder = new SigProvider(a.Name);
        if (h.Kind == HandleKind.FieldDefinition)
        {
            var f = md.GetFieldDefinition((FieldDefinitionHandle)h);
            return new(Owner(a, f.GetDeclaringType()), md.GetString(f.Name), f.DecodeSignature(decoder, null), isStatic);
        }
        var r = md.GetMemberReference((MemberReferenceHandle)h);
        return new(Type(a, r.Parent), md.GetString(r.Name), r.DecodeFieldSignature(decoder, null), isStatic);
    }

    private static MethodRef Method(Artifact a, EntityHandle h)
    {
        var md = a.Metadata;
        var decoder = new SigProvider(a.Name);
        switch (h.Kind)
        {
            case HandleKind.MethodDefinition:
                var d = md.GetMethodDefinition((MethodDefinitionHandle)h);
                return new(Owner(a, d.GetDeclaringType()), md.GetString(d.Name), d.DecodeSignature(decoder, null), [],
                    (d.Attributes & System.Reflection.MethodAttributes.Virtual) != 0);
            case HandleKind.MemberReference:
                var r = md.GetMemberReference((MemberReferenceHandle)h);
                if (r.Parent.Kind == HandleKind.MethodDefinition) throw new NotMechanical("vararg call site");
                return new(Type(a, r.Parent), md.GetString(r.Name), r.DecodeMethodSignature(decoder, null), [], true);
            case HandleKind.MethodSpecification:
                var s = md.GetMethodSpecification((MethodSpecificationHandle)h);
                return Method(a, s.Method) with { TypeArgs = s.DecodeSignature(decoder, null) };
            default:
                throw new NotMechanical("unexpected method token");
        }
    }

    public static string Literal(object? value) => value switch
    {
        null => "null",
        string s => SymbolDisplay(s),
        bool b => b ? "true" : "false",
        char c => c is >= ' ' and < '\x7f' and not '\'' and not '\\' ? $"'{c}'" : $"'\\u{(int)c:x4}'",
        double d when double.IsNaN(d) => "double.NaN",
        double d when double.IsPositiveInfinity(d) => "double.PositiveInfinity",
        double d when double.IsNegativeInfinity(d) => "double.NegativeInfinity",
        double d => d.ToString("R", CultureInfo.InvariantCulture) is var t && (t.Contains('.') || t.Contains('E')) ? t : t + ".0",
        float f when float.IsNaN(f) => "float.NaN",
        float f when float.IsPositiveInfinity(f) => "float.PositiveInfinity",
        float f when float.IsNegativeInfinity(f) => "float.NegativeInfinity",
        float f => f.ToString("R", CultureInfo.InvariantCulture) + "f",
        long l => l.ToString(CultureInfo.InvariantCulture) + "L",
        ulong u => u.ToString(CultureInfo.InvariantCulture) + "UL",
        uint u => u.ToString(CultureInfo.InvariantCulture) + "u",
        _ => System.Convert.ToString(value, CultureInfo.InvariantCulture)!,
    };

    private static string SymbolDisplay(string s)
    {
        var sb = new System.Text.StringBuilder("\"");
        foreach (char c in s)
            sb.Append(c switch
            {
                '"' => "\\\"", '\\' => "\\\\", '\n' => "\\n", '\r' => "\\r", '\t' => "\\t", '\0' => "\\0",
                < ' ' or '\x7f' or '\u2028' or '\u2029' or '\x85' => $"\\u{(int)c:x4}",
                _ => c.ToString(),
            });
        return sb.Append('"').ToString();
    }

    public static TypeSig Substitute(TypeSig sig, ImmutableArray<TypeSig> typeArgs, ImmutableArray<TypeSig> methodArgs = default) => sig switch
    {
        GenericParamSig { OfMethod: false } g when g.Index < typeArgs.Length => typeArgs[g.Index],
        GenericParamSig { OfMethod: true } g when !methodArgs.IsDefault && g.Index < methodArgs.Length => methodArgs[g.Index],
        NamedSig n when n.Args.Length > 0 => n with { Args = [.. n.Args.Select(a => Substitute(a, typeArgs, methodArgs))] },
        ArraySig a => a with { Element = Substitute(a.Element, typeArgs, methodArgs) },
        PointerSig p => p with { Element = Substitute(p.Element, typeArgs, methodArgs) },
        ByRefSig r => r with { Element = Substitute(r.Element, typeArgs, methodArgs) },
        _ => sig,
    };
}
