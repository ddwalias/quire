using System.Globalization;
using System.Reflection;

namespace Quire.Differential.Tests;

/// <summary>
/// Calls the same mplrun member on Quire's and Mathcad's side by name, and turns results into comparable text. Quire
/// objects are copied to Mathcad's side through Mathcad's public constructors, so both sides start from the same state.
/// </summary>
public static class MplRunSides
{
    public const BindingFlags All = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

    private static readonly Assembly QuireMpl = typeof(Quire.Mpl.Run.Sexpr).Assembly;

    /// <summary>Quire's type for an mplrun name such as "Mpl.Run.SCons".</summary>
    public static System.Type Q(string name) => QuireMpl.GetType("Quire." + name, throwOnError: true)!;

    /// <summary>Mathcad's type for an mplrun name.</summary>
    public static System.Type M(string name) => Mathcad.Type("mplrun", name);

    private static bool Mirrored(System.Type t) =>
        t.Namespace is "Mpl.Run" or "Quire.Mpl.Run" or "Mpl" or "Quire.Mpl";

    private static string MplName(System.Type t) => t.FullName!.StartsWith("Quire.", StringComparison.Ordinal) ? t.FullName![6..] : t.FullName!;

    /// <summary>The value or the exception type a call ends with, as text.</summary>
    public static string Outcome(Func<object?> f)
    {
        try { return Describe(f()); }
        catch (TargetInvocationException e) { return "throws " + e.InnerException!.GetType().Name; }
        catch (Exception e) { return "throws " + e.GetType().Name; }
    }

    public static void Same(List<string> diffs, string what, Func<object?> quire, Func<object?> mathcad)
    {
        string a = Outcome(quire), b = Outcome(mathcad);
        if (a != b) diffs.Add($"{what}: Quire {a}, Mathcad {b}");
    }

    public static void NoDiffs(List<string> diffs) =>
        Assert.True(diffs.Count == 0, $"{diffs.Count} differences:\n" + string.Join("\n", diffs.Take(40)));

    /// <summary>
    /// A structural description: mirrored objects by type name and their mplrun-declared instance fields, the
    /// shared empty list by name, types by mplrun name and generic arguments.
    /// </summary>
    public static string Describe(object? o, int depth = 0)
    {
        if (depth > 40) return "…";
        switch (o)
        {
            case null: return "null";
            case string s: return "\"" + s + "\"";
            case char c: return "'" + c + "'";
            case double d: return d.ToString("R", CultureInfo.InvariantCulture);
            case bool or int or uint or long: return Convert.ToString(o, CultureInfo.InvariantCulture)!;
            case System.Type t: return DescribeType(t);
            case FieldInfo fi: return $"field {DescribeType(fi.DeclaringType!)}.{fi.Name}:{DescribeType(fi.FieldType)}";
            case MethodInfo mi:
                return $"method {DescribeType(mi.DeclaringType!)}.{mi.Name}<{string.Join(",", mi.GetGenericArguments().Select(DescribeType))}>";
            case object[] a: return "[" + string.Join(", ", a.Select(x => Describe(x, depth + 1))) + "]";
            case Array a: return a.GetType().Name + "(" + a.Length + ")";
        }
        var type = o.GetType();
        if (!Mirrored(type)) return type.Name;
        if (type.IsEnum) return type.Name + "." + o;
        if (type.Name == "Nul" && ReferenceEquals(o, type.GetField("singleton")!.GetValue(null))) return "Nul.singleton";
        if (type.Name is "LocalEnv" or "MplEnv") return type.Name;
        var fields = new List<string>();
        for (var t = type; t != null && Mirrored(t); t = t.BaseType)
            foreach (var f in t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
                fields.Add(f.Name + "=" + Describe(f.GetValue(o), depth + 1));
        fields.Sort(StringComparer.Ordinal);
        return type.Name + "{" + string.Join(",", fields) + "}";
    }

    private static string DescribeType(System.Type t)
    {
        if (!Mirrored(t)) return t.IsArray ? DescribeType(t.GetElementType()!) + "[]" : t.FullName ?? t.Name;
        string name = MplName(t.IsGenericType ? t.GetGenericTypeDefinition() : t);
        return t.IsGenericType && !t.IsGenericTypeDefinition
            ? name + "[" + string.Join(",", t.GetGenericArguments().Select(DescribeType)) + "]"
            : name;
    }

    /// <summary>The members named on either side, matched by parameter type names.</summary>
    private static T Find<T>(IEnumerable<T> candidates, string[] sig) where T : MethodBase =>
        candidates.Single(m => m.GetParameters().Select(p => p.ParameterType.Name.TrimEnd('&')).SequenceEqual(sig));

    public static object? New(System.Type t, string[] sig, params object?[] args) =>
        Find(t.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance), sig).Invoke(args);

    /// <summary>Calls a method; <paramref name="args"/> receives out parameters.</summary>
    public static object? Call(System.Type t, object? target, string name, string[] sig, object?[] args) =>
        Find(t.GetMethods(All).Where(m => m.Name == name), sig).Invoke(target, args);

    public static object? Call(object target, string name, string[] sig, params object?[] args) =>
        Call(target.GetType(), target, name, sig, args);

    public static object? Static(System.Type t, string name, string[] sig, params object?[] args) =>
        Call(t, null, name, sig, args);

    /// <summary>A property, or failing that a field, of an object or (for a type) a static one.</summary>
    public static object? Get(object target, string name)
    {
        var (t, o) = target is System.Type ty ? (ty, (object?)null) : (target.GetType(), target);
        for (var x = t; x != null; x = x.BaseType)
        {
            if (x.GetProperty(name, All | BindingFlags.DeclaredOnly) is { } p) return p.GetValue(o);
            if (x.GetField(name, All | BindingFlags.DeclaredOnly) is { } f) return f.IsLiteral ? f.GetRawConstantValue() : f.GetValue(o);
        }
        throw new MissingMemberException(t.Name, name);
    }

    public static void Set(object target, string name, object? value)
    {
        for (var x = target.GetType(); x != null; x = x.BaseType)
        {
            if (x.GetProperty(name, All | BindingFlags.DeclaredOnly) is { } p) { p.SetValue(target, value); return; }
            if (x.GetField(name, All | BindingFlags.DeclaredOnly) is { } f) { f.SetValue(target, value); return; }
        }
        throw new MissingMemberException(target.GetType().Name, name);
    }

    /// <summary>
    /// Mathcad's copy of a Quire S-expression, atom or <c>Cons</c> list, built through Mathcad's constructors with
    /// the same field values; shared objects stay shared and Quire's empty-list singleton maps to Mathcad's.
    /// </summary>
    public static object? ToMathcad(object? q, Dictionary<object, object>? seen = null)
    {
        if (q == null || !Mirrored(q.GetType())) return q;
        seen ??= new Dictionary<object, object>(ReferenceEqualityComparer.Instance);
        if (seen.TryGetValue(q, out var done)) return done;
        string name = MplName(q.GetType());
        var mt = M(name);
        object? F(string field) => ToMathcad(Get(q, field), seen);
        object m = q.GetType().Name switch
        {
            "Nul" => ReferenceEquals(q, Quire.Mpl.Run.Nul.singleton) ? Get(mt, "singleton")! : New(mt, []),
            "Sym" => New(mt, ["String", "String", "Int32"], Get(q, "Field1"), Get(q, "Field2"), Get(q, "m_tag")),
            "int_constant" => New(mt, ["Int32"], Get(q, "Field1")),
            "real_constant" => New(mt, ["Double"], Get(q, "Field1")),
            "str_constant" => New(mt, ["String"], Get(q, "Field1")),
            "char_constant" => New(mt, ["Char"], Get(q, "Field1")),
            "Tok" => New(mt, ["Atom", "UInt32"], F("Field1"), Get(q, "Field2")),
            "SCons" or "SAppend" => New(mt, ["Sexpr", "Sexpr"], F("Field1"), F("Field2")),
            "SList" => New(mt, ["Cons", "Sexpr"], F("Field1"), F("Field2")),
            "Qt" or "Bq" or "Cm" or "Ca" => New(mt, ["Sexpr"], F("Field1")),
            "Cons" => New(mt, ["Object", "Object"], F("Field1"), F("Field2")),
            _ => throw new NotSupportedException(name),
        };
        if (q.GetType().Name == "Sym") Set(m, "uid", Get(q, "uid"));
        if (q.GetType().GetField("m_tag") != null) Set(m, "m_tag", Get(q, "m_tag"));
        seen[q] = m;
        return m;
    }
}
