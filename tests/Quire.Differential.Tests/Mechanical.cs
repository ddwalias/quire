using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using Quire.Mirror;

namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons for members that MemberGen (tools/Quire.Tools.MemberGen) reconstructed mechanically. A test
/// names Mathcad's method by token; this finds Quire's counterpart through <c>[MirrorOf]</c>, and compares the two
/// on generated inputs: zero and null first, then distinct values for every parameter and every instance field of the
/// target, so a body that reads the wrong field or argument, or stores to the wrong one, differs. Targets are created
/// without running a constructor and filled field by field on both sides; objects render as their Mathcad type name
/// and fields. Maintainer-owned: every generated test class uses it.
/// </summary>
public static class Mechanical
{
    private const BindingFlags Declared = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
    private const int Rounds = 3;
    private const int Depth = 2;
    /// <summary>Sample numbers per round and per slot: a slot's values are <c>round*Round + index*Slot + k</c>.</summary>
    private const int Round = 1_000_000, Slot = 10_000;

    public static void Compare(System.Type quireOuter, int token)
    {
        var binding = MirrorOf(quireOuter) ?? throw new HarnessException($"{quireOuter} has no [MirrorOf]");
        string assembly = binding.Split(',')[1].Trim();
        var original = Mathcad.Load(assembly).ManifestModule.ResolveMethod(token)
            ?? throw new HarnessException($"no method 0x{token:x8} in {assembly}");
        if (assembly == "mplrun") Runtime.Sync();
        var mathcadType = original.DeclaringType!;
        var quireType = Nested(quireOuter, mathcadType);
        var quire = Counterpart(quireType, original);
        (quire, original) = (Close(quire), Close(original));
        if (quire is ConstructorInfo && quireType.IsAbstract)
            throw new HarnessException("a constructor of an abstract type cannot be invoked directly");
        string contract = Name(mathcadType).Split('+')[0];
        for (int round = 0; round < Rounds; round++)
        {
            var qParameters = quire.GetParameters();
            var mParameters = original.GetParameters();
            var qArgs = new object?[qParameters.Length];
            var mArgs = new object?[mParameters.Length];
            for (int i = 0; i < qArgs.Length; i++)
            {
                int k = 0;
                (qArgs[i], mArgs[i]) = Sample(Element(qParameters[i].ParameterType), Element(mParameters[i].ParameterType), round, i, 0, ref k);
            }
            object? qTarget = null, mTarget = null;
            if (!quire.IsStatic && quire is not ConstructorInfo)
            {
                int k = 0;
                (qTarget, mTarget) = Instance(quireType, mathcadType, quire, round, qArgs.Length, 0, ref k);
            }
            string target = qTarget == null ? "" : "target, ";
            MethodComparison.Compare($"round {round}: {target}{string.Join(", ", qArgs.Select(Show))}", contract,
                quire, qTarget, qArgs, original, mTarget, mArgs, Show, Show, Show, Show);
        }
    }

    private static System.Type Element(System.Type t) => t.IsByRef ? t.GetElementType()! : t;

    private static MethodBase Close(MethodBase m) => m is MethodInfo { IsGenericMethodDefinition: true } g
        ? g.MakeGenericMethod([.. g.GetGenericArguments().Select(_ => typeof(object))]) : m;

    /// <summary>The Mathcad type a Quire type mirrors, "Namespace.Type, assembly"; nested types inherit their outer
    /// type's assembly.</summary>
    private static string? MirrorOf(System.Type t) => t.GetCustomAttribute<MirrorOfAttribute>()?.MathcadType;

    private static System.Type Nested(System.Type outer, System.Type mathcad)
    {
        var path = new Stack<string>();
        for (var t = mathcad; t.DeclaringType != null; t = t.DeclaringType) path.Push(t.Name);
        var q = outer;
        foreach (string name in path)
            q = q.GetNestedType(name, BindingFlags.Public | BindingFlags.NonPublic) ?? q.GetNestedType(name + "_", BindingFlags.Public | BindingFlags.NonPublic)
                ?? throw new HarnessException($"no Quire counterpart of {mathcad}");
        return q;
    }

    /// <summary>Quire's declaration of Mathcad's method: same name (or MirrorGen's <c>_</c>-suffixed one), generic
    /// arity and parameter types under Mathcad's names.</summary>
    private static MethodBase Counterpart(System.Type quireType, MethodBase original)
    {
        var expected = original.GetParameters().Select(p => Name(p.ParameterType)).ToArray();
        var candidates = (original is ConstructorInfo ? quireType.GetConstructors(Declared).Cast<MethodBase>() : quireType.GetMethods(Declared))
            .Where(m => m.IsStatic == original.IsStatic && (m.Name == original.Name || m.Name == original.Name + "_")
                && m.IsGenericMethodDefinition == original.IsGenericMethodDefinition
                && m.GetParameters().Select(p => Name(p.ParameterType)).SequenceEqual(expected))
            .ToList();
        return candidates.Count == 1 ? candidates[0] : throw new HarnessException($"{candidates.Count} Quire counterparts of {original}");
    }

    /// <summary>A target pair of the declaring type (or, for an abstract one, a concrete subtype that inherits the
    /// method), created without a constructor, with each instance field set to the same sample value on both sides.</summary>
    private static (object?, object?) Instance(System.Type q, System.Type m, MethodBase? method, int round, int index, int depth, ref int k)
    {
        if (q.IsAbstract)
        {
            var concrete = q.Assembly.GetTypes().Where(t => !t.IsAbstract && q.IsAssignableFrom(t) && !t.ContainsGenericParameters
                    && Inherits(t, q, method) && MathcadType(t) != null)
                .OrderBy(t => t.FullName, StringComparer.Ordinal).FirstOrDefault();
            if (concrete == null) return (null, null);
            (q, m) = (concrete, MathcadType(concrete)!);
        }
        if (q.ContainsGenericParameters || m.ContainsGenericParameters) throw new HarnessException("open generic target");
        object qo = RuntimeHelpers.GetUninitializedObject(q), mo = RuntimeHelpers.GetUninitializedObject(m);
        if (round == 0) return (qo, mo);
        var mFields = Fields(m).ToDictionary(f => f.Name);
        foreach (var qf in Fields(q).OrderBy(f => f.Name, StringComparer.Ordinal))
        {
            if (!mFields.TryGetValue(qf.Name, out var mf) && !mFields.TryGetValue(qf.Name.TrimEnd('_'), out mf)) continue;
            var (qv, mv) = Sample(qf.FieldType, mf.FieldType, round, index, depth + 1, ref k);
            qf.SetValue(qo, qv);
            mf.SetValue(mo, mv);
        }
        return (qo, mo);
    }

    /// <summary>Whether no type from <paramref name="t"/> up to <paramref name="declaring"/> overrides the method.</summary>
    private static bool Inherits(System.Type t, System.Type declaring, MethodBase? method)
    {
        if (method is not MethodInfo { IsVirtual: true } virtualMethod) return true;
        for (var x = t; x != null && x != declaring; x = x.BaseType)
            if (x.GetMethods(Declared).Any(d => d.GetBaseDefinition() == virtualMethod.GetBaseDefinition())) return false;
        return true;
    }

    private static System.Type? MathcadType(System.Type quire)
    {
        var path = new Stack<string>();
        var t = quire;
        for (; t.DeclaringType != null; t = t.DeclaringType) path.Push(t.Name.TrimEnd('_'));
        if (MirrorOf(t) is not { } binding) return null;
        string[] parts = binding.Split(',');
        try { return Mathcad.Type(parts[1].Trim(), string.Join("+", path.Prepend(parts[0].Trim()))); }
        catch (HarnessException) { return null; }
    }

    /// <summary>Instance fields of the type and its mirrored base types; Quire-only fields have no counterpart.</summary>
    private static IEnumerable<FieldInfo> Fields(System.Type t)
    {
        for (var x = t; x != null && (Mirrored(x) || IsMathcad(x)); x = x.BaseType)
            foreach (var f in x.GetFields(Declared & ~BindingFlags.Static))
                if (!f.IsDefined(typeof(QuireOnlyAttribute))) yield return f;
    }

    /// <summary>The same value on both sides: zero, false or null in round 0; otherwise a value that differs between
    /// rounds, between slots (<paramref name="index"/>: an argument, or the target) and between the values within a
    /// slot (<paramref name="k"/> counts the slot's elements and fields): <c>round*Round + index*Slot + k</c>. Booleans
    /// and enums alternate with the same three.</summary>
    private static (object?, object?) Sample(System.Type q, System.Type m, int round, int index, int depth, ref int k)
    {
        if (q.IsPointer || m.IsPointer) throw new HarnessException("pointer parameter");
        if (round == 0 || depth > Depth) return (Default(q), Default(m));
        int value = k++, n = Number(round, index, value);
        if (q.IsEnum && m.IsEnum)
        {
            var values = Enum.GetValues(q);
            if (values.Length == 0) return (Default(q), Default(m));
            object v = values.GetValue(Choice(round, index, value, values.Length))!;
            return (v, Enum.ToObject(m, System.Convert.ToInt64(v, CultureInfo.InvariantCulture)));
        }
        object? Scalar(System.Type t) => System.Type.GetTypeCode(t) switch
        {
            TypeCode.Boolean => Choice(round, index, value, 2) == 1,
            TypeCode.Char => (char)('a' + n % 26),
            TypeCode.String => $"s{n}",
            TypeCode.Double => n + 0.5,
            TypeCode.Single => n + 0.5f,
            TypeCode.Decimal => n + 0.5m,
            // Narrow integers cannot hold every sample number: they wrap within their range.
            TypeCode.SByte or TypeCode.Byte => System.Convert.ChangeType(n % 127, t, CultureInfo.InvariantCulture),
            TypeCode.Int16 or TypeCode.UInt16 => System.Convert.ChangeType(n % 32_767, t, CultureInfo.InvariantCulture),
            TypeCode.Int32 or TypeCode.UInt32
                or TypeCode.Int64 or TypeCode.UInt64 => System.Convert.ChangeType(n, t, CultureInfo.InvariantCulture),
            _ => null,
        };
        if (q == m && !q.IsEnum && Scalar(q) is { } scalar) return (scalar, scalar);
        if (q == typeof(object) && m == typeof(object)) return round == 1 ? (n, n) : ($"o{index}", $"o{index}");
        if (q == typeof(System.Type) && m == typeof(System.Type)) return (typeof(int), typeof(int));
        if (q.IsArray && m.IsArray)
        {
            var qa = Array.CreateInstance(q.GetElementType()!, round);
            var ma = Array.CreateInstance(m.GetElementType()!, round);
            for (int i = 0; i < round; i++)
            {
                var (qe, me) = Sample(q.GetElementType()!, m.GetElementType()!, round, index, depth + 1, ref k);
                qa.SetValue(qe, i);
                ma.SetValue(me, i);
            }
            return (qa, ma);
        }
        if (Mirrored(q) && !q.IsInterface && !typeof(Delegate).IsAssignableFrom(q))
            return Instance(q, m, null, round, index, depth, ref k);
        return (Default(q), Default(m));
    }

    /// <summary>The sample number of a slot's <paramref name="value"/>th value; no two slots or rounds share one.</summary>
    internal static int Number(int round, int index, int value) => value < Slot ? round * Round + index * Slot + value + 1
        : throw new HarnessException($"more than {Slot} sample values in one slot");

    /// <summary>One of <paramref name="count"/> choices (a boolean, an enum value), alternating between consecutive
    /// values of a slot as well as between slots and rounds.</summary>
    internal static int Choice(int round, int index, int value, int count) => (round + index + value) % count;

    private static object? Default(System.Type t) => t.IsValueType && t != typeof(void) ? RuntimeHelpers.GetUninitializedObject(t) : null;

    private static bool Mirrored(System.Type t)
    {
        for (var x = t; x != null; x = x.DeclaringType)
            if (MirrorOf(x) != null) return true;
        return false;
    }

    private static bool IsMathcad(System.Type t) => AssemblyLoadContext.GetLoadContext(t.Assembly)?.Name == "Mathcad";

    /// <summary>A type as Mathcad names it: Quire's mirrored types under their <c>[MirrorOf]</c> name.</summary>
    private static string Name(System.Type t)
    {
        if (t.IsByRef) return Name(t.GetElementType()!) + "&";
        if (t.IsArray) return Name(t.GetElementType()!) + "[" + new string(',', t.GetArrayRank() - 1) + "]";
        if (t.IsGenericParameter) return (t.DeclaringMethod != null ? "!!" : "!") + t.GenericParameterPosition;
        string name = MirrorOf(t) is { } binding ? binding.Split(',')[0].Trim()
            : t.DeclaringType != null ? Name(t.DeclaringType.IsGenericType ? t.DeclaringType.GetGenericTypeDefinition() : t.DeclaringType) + "+" + t.Name.TrimEnd('_')
            : t.Namespace == null ? t.Name : t.Namespace + "." + t.Name;
        return t.IsGenericType && !t.IsGenericTypeDefinition ? name + "<" + string.Join(",", t.GetGenericArguments().Select(Name)) + ">" : name;
    }

    /// <summary>Comparable text: scalars invariant, mirrored objects as their Mathcad type and fields (recursively,
    /// to a fixed depth, shared references once), collections by their items, other objects by type.</summary>
    public static string Show(object? o) => Show(o, 0, new HashSet<object>(ReferenceEqualityComparer.Instance));

    private static string Show(object? o, int depth, HashSet<object> seen)
    {
        switch (o)
        {
            case null: return "null";
            case string s: return "\"" + s + "\"";
            case char c: return "'" + c + "'";
            case bool b: return b ? "true" : "false";
            case double d: return d.ToString("R", CultureInfo.InvariantCulture);
            case float f: return f.ToString("R", CultureInfo.InvariantCulture);
            case Enum e: return Name(e.GetType()) + "." + e;
            case IFormattable x when o.GetType().IsPrimitive || o is decimal: return x.ToString(null, CultureInfo.InvariantCulture);
            case System.Type t: return "typeof(" + Name(t) + ")";
            case MemberInfo mi: return $"{mi.MemberType} {(mi.DeclaringType == null ? "" : Name(mi.DeclaringType))}.{mi.Name}";
            case Delegate d: return $"delegate {Name(d.GetType())} to {d.Method.Name}";
        }
        var type = o.GetType();
        if (depth > 4 || !type.IsValueType && !seen.Add(o)) return Name(type) + "{…}";
        if (o is Array a) return Name(type) + "[" + string.Join(", ", a.Cast<object?>().Select(x => Show(x, depth + 1, seen))) + "]";
        if (Mirrored(type) || IsMathcad(type))
            return Name(type) + "{" + string.Join(", ", Fields(type).OrderBy(f => f.Name.TrimEnd('_'), StringComparer.Ordinal)
                .Select(f => f.Name.TrimEnd('_') + "=" + Show(f.GetValue(o), depth + 1, seen))) + "}";
        if (o is IEnumerable items && type.Namespace?.StartsWith("System.Collections", StringComparison.Ordinal) == true)
            return Name(type) + "[" + string.Join(", ", items.Cast<object?>().Select(x => Show(x, depth + 1, seen))) + "]";
        return Name(type);
    }
}
