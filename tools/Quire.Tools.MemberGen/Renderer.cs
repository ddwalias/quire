using Quire.Tools.MirrorGen;

/// <summary>
/// Writes a decompiled <see cref="Body"/> as idiomatic C# in Quire's names: <c>=> expression;</c> for a single return
/// or statement, a block otherwise, and a constructor's <c>: base(…)</c> or <c>: this(…)</c>.
/// </summary>
sealed class Renderer(World world, Names names, TypeModel self, string[] parameters)
{
    public sealed record Text(string? Initializer, string? Expression, List<string> Statements);

    public Text Render(Body body, bool constructor)
    {
        string? initializer = null;
        var lines = new List<string>();
        foreach (var s in body.Statements)
            switch (s)
            {
                case Initializer i:
                    // An implicit base() call needs no initializer.
                    if (i.Same || i.Args.Length > 0) initializer = $": {(i.Same ? "this" : "base")}({Arguments(i.Ctor, i.Args)})";
                    break;
                case Return { Value: null }: break;
                case Return { Value: var v }: lines.Add($"return {Expression(v!, body.Returns)};"); break;
                case Throw t: lines.Add($"throw {Expression(t.Value, null)};"); break;
                case Assign x: lines.Add($"{Expression(x.Target, null)} = {Expression(x.Value, x.Target.Type)};"); break;
                case Do { Value: Call { M.Name: var n } c } when Accessor(c) is { } property:
                    string verb = n.StartsWith("set_") ? "=" : n.StartsWith("add_") ? "+=" : "-=";
                    lines.Add($"{property} {verb} {Expression(c.Args[^1], c.M.Sig.ParameterTypes[^1])};");
                    break;
                case Do d: lines.Add(Expression(d.Value, null) + ";"); break;
            }
        // A single return, or a single statement of a non-constructor, makes an expression body.
        string? expression = !constructor && lines.Count == 1
            ? lines[0].StartsWith("return ") ? lines[0]["return ".Length..^1] : lines[0][..^1] : null;
        return new(initializer, expression, lines);
    }

    /// <summary>The property or event an accessor call sets, adds to or removes from; null for other calls.</summary>
    private string? Accessor(Call c)
    {
        string name = c.M.Name;
        if (!(name.StartsWith("set_") && c.Args.Length == 1 || name.StartsWith("add_") || name.StartsWith("remove_"))) return null;
        string member = Member(c.M.Owner, name[(name.IndexOf('_') + 1)..], name.StartsWith("set_") ? Kind.Property : Kind.Event);
        return Receiver(c, member);
    }

    private enum Kind { Field, Property, Event, Method }

    /// <summary>Quire's name for a member of a Mathcad type: the same, or with the <c>_</c> MirrorGen appends on a
    /// clash; an auto-property's backing field is the property itself.</summary>
    private string Member(TypeSig owner, string name, Kind kind)
    {
        if (kind == Kind.Field && name.StartsWith('<') && name.EndsWith(">k__BackingField")) (name, kind) = (name[1..name.IndexOf('>')], Kind.Property);
        var quire = world.ToQuire(owner) is NamedSig n ? world.QuireTypes.GetValueOrDefault(n.Name) : null;
        if (quire == null)
            return owner is NamedSig o && world.QuireName(o.Assembly, o.Name) != null
                ? throw new NotMechanical("unresolved member") : Names.Identifier(name);
        IEnumerable<string> declared = kind switch
        {
            Kind.Field => quire.Fields.Select(f => f.Name),
            Kind.Property => quire.Properties.Select(p => p.Name),
            Kind.Event => quire.Events.Select(e => e.Name),
            _ => quire.Methods.Select(m => m.Name),
        };
        var set = declared.ToHashSet();
        return set.Contains(name) ? Names.Identifier(name) : set.Contains(name + "_") ? name + "_"
            : throw new NotMechanical("unresolved member");
    }

    private string QuireType(TypeSig sig) => names.Type(world.ToQuire(sig));

    /// <summary>Whether a static member of <paramref name="owner"/> is in scope here without qualification.</summary>
    private bool InScope(TypeSig owner)
    {
        string? name = (world.ToQuire(owner) as NamedSig)?.Name;
        for (string? t = self.FullName; t != null; t = world.QuireTypes.GetValueOrDefault(t)?.DeclaringType)
            if (t == name) return true;
        return false;
    }

    private string Receiver(Call c, string member) => c.Target switch
    {
        null => InScope(c.M.Owner) && !parameters.Contains(member) ? member : $"{QuireType(c.M.Owner)}.{member}",
        This when c.Base => "base." + member,
        This => parameters.Contains(member) ? "this." + member : member,
        var t => $"{Expression(t, null, 16)}.{member}",
    };

    private string Arguments(MethodRef m, Expr[] args) =>
        string.Join(", ", args.Select((a, i) => Expression(a, m.Sig.ParameterTypes[i])));

    private static readonly Dictionary<string, int> Precedence = new()
    {
        ["*"] = 13, ["/"] = 13, ["%"] = 13, ["+"] = 12, ["-"] = 12, ["<<"] = 11, [">>"] = 11,
        ["<"] = 10, [">"] = 10, ["<="] = 10, [">="] = 10, ["=="] = 9, ["!="] = 9, ["&"] = 8, ["^"] = 7, ["|"] = 6,
    };
    private static readonly Dictionary<string, string> OperatorMethods = new()
    {
        ["op_Addition"] = "+", ["op_Subtraction"] = "-", ["op_Multiply"] = "*", ["op_Division"] = "/", ["op_Modulus"] = "%",
        ["op_Equality"] = "==", ["op_Inequality"] = "!=", ["op_LessThan"] = "<", ["op_GreaterThan"] = ">",
        ["op_LessThanOrEqual"] = "<=", ["op_GreaterThanOrEqual"] = ">=", ["op_BitwiseAnd"] = "&", ["op_BitwiseOr"] = "|",
        ["op_ExclusiveOr"] = "^",
    };

    /// <summary>C# for an expression in a context of type <paramref name="expected"/> (when known), parenthesized
    /// when its precedence is below <paramref name="context"/>.</summary>
    public string Expression(Expr e, TypeSig? expected, int context = 0)
    {
        // IL booleans are ints: a comparison where an int is expected was `c ? 1 : 0`, which compiles to the bare
        // comparison.
        if (expected is NamedSig { Name: "System.Int32" } && e.Type is NamedSig { Name: "System.Boolean" } && e is Binary or Unary { Op: "!" } or Is)
        {
            string condition = Expression(e, Decompiler.Boolean, 3);
            return 2 < context ? $"({condition} ? 1 : 0)" : $"{condition} ? 1 : 0";
        }
        var (text, precedence) = Write(e, expected);
        return precedence < context ? $"({text})" : text;
    }

    private (string, int) Write(Expr e, TypeSig? expected)
    {
        switch (e)
        {
            case This: return ("this", 16);
            case Arg a: return (Names.Identifier(parameters[a.Index]), 16);
            case Const c: return Constant(c, expected);
            case TypeOf t: return ($"typeof({QuireType(t.Of)})", 16);
            case Field { F: var f } x:
            {
                string member = Member(f.Owner, f.Name, Kind.Field);
                return (x.Target switch
                {
                    null => InScope(f.Owner) && !parameters.Contains(member) ? member : $"{QuireType(f.Owner)}.{member}",
                    This => parameters.Contains(member) ? "this." + member : member,
                    var t => $"{Expression(t, null, 16)}.{member}",
                }, 16);
            }
            case Call c when OperatorMethods.TryGetValue(c.M.Name, out var op) && c.Target == null && c.Args.Length == 2:
                return Write(new Binary(op, c.Args[0], c.Args[1]), expected);
            case Call { M.Name: "op_Explicit" } c when c.Target == null: return ($"({QuireType(c.M.Sig.ReturnType)}){Expression(c.Args[0], null, 15)}", 15);
            case Call { M: { Name: "Concat", Owner: NamedSig { Name: "System.String" } } } c
                when c.Args.Length is >= 2 and <= 4 && c.M.Sig.ParameterTypes.All(p => p is NamedSig { Name: "System.String" }):
                return (string.Join(" + ", c.Args.Select((a, i) => Expression(a, c.M.Sig.ParameterTypes[i], i == 0 ? 12 : 13))), 12);
            case Call c:
            {
                string name = c.M.Name;
                if (name.StartsWith("get_") && Property(c) is { } property)
                    return (c.Args.Length == 0 ? Receiver(c, property) : $"{Indexed(c)}[{Arguments(c.M, c.Args)}]", 16);
                string method = Member(c.M.Owner, name, Kind.Method);
                string typeArgs = c.M.TypeArgs.IsDefaultOrEmpty ? "" : "<" + string.Join(", ", c.M.TypeArgs.Select(QuireType)) + ">";
                return ($"{Receiver(c, method)}{typeArgs}({Arguments(c.M, c.Args)})", 16);
            }
            case New n: return ($"new {QuireType(n.Ctor.Owner)}({Arguments(n.Ctor, n.Args)})", 16);
            case NewArray n: return ($"new {QuireType(n.Element)}[{Expression(n.Size, Decompiler.Int32)}]", 16);
            case Element x: return ($"{Expression(x.Array, null, 16)}[{Expression(x.Index, Decompiler.Int32)}]", 16);
            case Length l: return ($"{Expression(l.Array, null, 16)}.Length", 16);
            case As a: return ($"{Expression(a.Value, null, 10)} as {QuireType(a.To)}", 10);
            case Is i: return ($"{Expression(i.Value, null, 10)} is {QuireType(i.To)}", 10);
            case Conversion v when !v.Explicit && Widening(v.Value.Type, v.To) && Same(expected, v.To): return Write(v.Value, expected);
            case Conversion v: return ($"({QuireType(v.To)}){Expression(v.Value, null, 15)}", 15);
            case Unary u:
            {
                // `-(-x)`, not `--x`, a decrement.
                string operand = Expression(u.Value, u.Op == "!" ? Decompiler.Boolean : expected, 15);
                return (u.Op + (u.Op == "-" && operand.StartsWith('-') ? $"({operand})" : operand), 15);
            }
            // C# masks a shift count itself: `x << (y & 31)` in IL is `x << y` in source.
            case Binary { Op: "<<" or ">>", Right: Binary { Op: "&", Right: Const { Value: 31 or 63 } } mask } b:
                return Write(b with { Right = mask.Left }, expected);
            case Binary b:
            {
                int p = Precedence[b.Op];
                // A constant compared with a bool, char or enum takes the other side's type.
                var lt = b.Right is Const ? b.Left.Type : b.Op is "==" or "!=" or "<" or ">" or "<=" or ">=" ? b.Right.Type : expected;
                var rt = b.Left is Const ? b.Right.Type : b.Op is "==" or "!=" or "<" or ">" or "<=" or ">=" ? b.Left.Type : expected;
                return ($"{Expression(b.Left, lt, p)} {b.Op} {Expression(b.Right, rt, p + 1)}", p);
            }
            default: throw new NotMechanical($"expression {e.GetType().Name}");
        }
    }

    private string Indexed(Call c) => c.Target switch
    {
        This => c.Base ? "base" : "this",
        null => throw new NotMechanical("static indexer"),
        var t => Expression(t, null, 16),
    };

    private string? Property(Call c)
    {
        string name = c.M.Name[4..];
        if (c.Args.Length > 0) return name == "Item" ? name : null;
        try { return Member(c.M.Owner, name, Kind.Property); }
        catch (NotMechanical) { return null; }
    }

    private static bool Same(TypeSig? a, TypeSig? b) => a != null && b != null && a.Equals(b);

    private static readonly string[] Ladder = ["System.SByte", "System.Byte", "System.Int16", "System.UInt16", "System.Char",
        "System.Int32", "System.UInt32", "System.Int64", "System.UInt64", "System.Single", "System.Double"];

    /// <summary>C#'s implicit numeric conversions, which compile to the same conversion instruction.</summary>
    private static bool Widening(TypeSig? from, TypeSig to) => (from, to) switch
    {
        (NamedSig { Name: "System.Int32" or "System.Int16" or "System.SByte" }, NamedSig { Name: "System.Int64" or "System.Double" or "System.Single" }) => true,
        (NamedSig { Name: "System.Int64" or "System.UInt32" or "System.UInt64" }, NamedSig { Name: "System.Double" or "System.Single" }) => true,
        (NamedSig { Name: "System.Single" }, NamedSig { Name: "System.Double" }) => true,
        _ => false,
    };

    private (string, int) Constant(Const c, TypeSig? expected)
    {
        object? v = c.Value;
        string? type = (expected as NamedSig)?.Name;
        if (v is int i)
        {
            switch (type)
            {
                case "System.Boolean" when i is 0 or 1: return (i == 1 ? "true" : "false", 16);
                case "System.Char": return (Decompiler.Literal((char)i), 16);
                case "System.UInt32": return (Decompiler.Literal((uint)i), 16);
                case "System.Int64" or "System.UInt64" or "System.Double" or "System.Single" or "System.Int16" or "System.Byte"
                    or "System.SByte" or "System.UInt16" or "System.Int32" or "System.Object" or null:
                    return (Decompiler.Literal(c.ConstType is NamedSig { Name: "System.Int64" } ? (object)(long)i : i), i < 0 ? 15 : 16);
            }
            if (world.Enum(expected!) is { } e)
            {
                var member = e.Fields.FirstOrDefault(f => f.Const && System.Convert.ToInt64(f.Value) == i);
                return member != null ? ($"{QuireType(expected!)}.{Names.Identifier(member.Name)}", 16) : ($"({QuireType(expected!)}){(i < 0 ? $"({i})" : i)}", 15);
            }
            if (expected is NamedSig) return ($"({QuireType(expected)}){(i < 0 ? $"({i})" : i)}", 15);
        }
        if (v is double d && type == "System.Single") v = (float)d;
        string text = Decompiler.Literal(v);
        return (text, text.StartsWith('-') ? 15 : 16);
    }
}
