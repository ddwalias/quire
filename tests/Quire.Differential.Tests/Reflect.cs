using System.Collections;
using System.Reflection;

namespace Quire.Differential.Tests;

public sealed class HarnessException(string message, Exception? inner = null) : Exception(message, inner);

/// <summary>
/// Reflection helpers for calling the same member on Mathcad's and Quire's types, which share names, and for
/// reducing results to comparable text.
/// </summary>
public static class Reflect
{
    private const BindingFlags All = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;

    /// <summary>Calls the method <paramref name="name"/> whose parameter types match the arguments' (null matches
    /// any reference type). <paramref name="target"/> is an instance, or a <see cref="System.Type"/> for statics.</summary>
    public static object? Call(object target, string name, params object?[] args)
    {
        var type = target as System.Type ?? target.GetType();
        var method = Select(Methods(type, name).Where(m => m.IsStatic == (target is System.Type) && !m.ContainsGenericParameters).Where(m => Fits(m.GetParameters(), args)), $"{type.FullName}.{name}");
        return Invoke(() => method.Invoke(target is System.Type ? null : target, args), method.ToString()!);
    }

    /// <summary>Calls an overload chosen by its parameter type names, for overloads that the arguments don't settle.</summary>
    public static object? CallSig(object target, string name, string[] parameterTypes, params object?[] args)
    {
        var type = target as System.Type ?? target.GetType();
        var method = Select(Methods(type, name).Where(m => m.IsStatic == (target is System.Type) && !m.ContainsGenericParameters).Where(m => m.GetParameters().Select(p => p.ParameterType.Name).SequenceEqual(parameterTypes)), $"{type.FullName}.{name}({string.Join(", ", parameterTypes)})");
        if (!Fits(method.GetParameters(), args))
            throw new HarnessException($"Arguments do not fit {method}");
        return Invoke(() => method.Invoke(target is System.Type ? null : target, args), method.ToString()!);
    }

    /// <summary>The methods named <paramref name="name"/>, most derived first; an override hides what it overrides.</summary>
    private static IEnumerable<MethodInfo> Methods(System.Type type, string name)
    {
        var slots = new HashSet<MethodInfo>();
        for (var t = type; t != null; t = t.BaseType)
            foreach (var m in t.GetMethods(All | BindingFlags.DeclaredOnly).Where(m => m.Name == name))
            {
                if (m.IsVirtual && !slots.Add(m.GetBaseDefinition())) continue;
                yield return m;
            }
    }

    private static T Select<T>(IEnumerable<T> candidates, string member)
    {
        var matches = candidates.Take(2).ToArray();
        if (matches.Length != 1)
            throw new HarnessException($"{member}: expected exactly one matching declaration, found {(matches.Length == 2 ? "multiple" : "none")}. Use CallSig for ambiguous arguments.");
        return matches[0];
    }

    private static bool Fits(ParameterInfo[] ps, object?[] args) =>
        ps.Length == args.Length && ps.Zip(args).All(p =>
        {
            var parameterType = p.First.ParameterType;
            if (parameterType.IsByRef) parameterType = parameterType.GetElementType()!;
            return p.Second == null ? p.First.IsOut || !parameterType.IsValueType
                : parameterType.IsInstanceOfType(p.Second);
        });

    /// <summary>Creates an instance through the constructor whose parameters match the arguments; private ones too.</summary>
    public static object New(System.Type type, params object?[] args)
    {
        var ctor = Select(type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .Where(c => Fits(c.GetParameters(), args)), $"{type.FullName} constructor");
        return Invoke(() => ctor.Invoke(args), type.FullName + " constructor")!;
    }

    /// <summary>A field or property value, public or not, instance or static.</summary>
    public static object? Get(object target, string name)
    {
        var type = target as System.Type ?? target.GetType();
        object? instance = target is System.Type ? null : target;
        for (var t = type; t != null; t = t.BaseType)
        {
            if (t.GetField(name, All | BindingFlags.DeclaredOnly) is { } f)
                return Invoke(() => f.GetValue(instance), $"{type.FullName}.{name}");
            if (t.GetProperty(name, All | BindingFlags.DeclaredOnly) is { } p)
                return Invoke(() => p.GetValue(instance), $"{type.FullName}.{name}");
        }
        throw new HarnessException($"Missing member: {type.FullName}.{name}");
    }

    public static void Set(object target, string name, object? value)
    {
        var type = target as System.Type ?? target.GetType();
        object? instance = target is System.Type ? null : target;
        for (var t = type; t != null; t = t.BaseType)
        {
            if (t.GetField(name, All | BindingFlags.DeclaredOnly) is { } f)
            {
                Invoke(() => { f.SetValue(instance, value); return null; }, $"{type.FullName}.{name}");
                return;
            }
            if (t.GetProperty(name, All | BindingFlags.DeclaredOnly) is { } p)
            {
                Invoke(() => { p.SetValue(instance, value); return null; }, $"{type.FullName}.{name}");
                return;
            }
        }
        throw new HarnessException($"Missing member: {type.FullName}.{name}");
    }

    private static object? Invoke(Func<object?> invocation, string member)
    {
        try { return invocation(); }
        catch (TargetInvocationException e) when (e.InnerException != null)
        {
            System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(e.InnerException).Throw();
            throw;
        }
        catch (Exception e)
        {
            throw new HarnessException($"Reflection invocation failed for {member}: {e.Message}", e);
        }
    }

    /// <summary>A nested type, private ones included.</summary>
    public static System.Type Nested(System.Type outer, string name) =>
        outer.GetNestedType(name, BindingFlags.Public | BindingFlags.NonPublic) ?? throw new HarnessException($"Missing nested type: {outer.FullName}.{name}");

    /// <summary>The outcome of <paramref name="f"/> as text: <paramref name="show"/> of the value, or the exception's
    /// type and message.</summary>
    public static string Outcome(Func<object?> f, Func<object?, string>? show = null)
    {
        object? value;
        try { value = Materialize(f()); }
        catch (HarnessException) { throw; }
        catch (Exception e) { return Thrown(e); }
        // A broken result renderer is a harness failure, never a runtime outcome.
        return (show ?? Text)(value);
    }

    // Iterator execution belongs to the runtime operation. Render only its captured values,
    // so iterator exceptions are compared but formatter bugs still fail the test.
    private static object? Materialize(object? value) => value is IEnumerable sequence && value is not string && value is not Array
        ? sequence.Cast<object?>().Select(Materialize).ToArray() : value;

    public static string Thrown(Exception e)
    {
        while (e is TargetInvocationException { InnerException: { } inner }) e = inner;
        // Quire's names are Mathcad's under Quire. (mapping.md §2), and messages that name types show it
        string text = $"throws {e.GetType().Name}: {System.Text.RegularExpressions.Regex.Replace(e.Message, @"\bQuire\.(?=(Mcd|Mpl)\.)", "")}";
        if (e.GetType().GetProperty("Argument") != null && e.GetType().GetProperty("Handle") != null)
            text += $" (argument {Get(e, "Argument")}, handle {Get(e, "Handle")})";
        return e.InnerException == null ? text : text + " <- " + Thrown(e.InnerException);
    }

    /// <summary>Plain text for scalars, strings and arrays; type name and ToString otherwise.</summary>
    public static string Text(object? o) => o switch
    {
        null => "null",
        string s => "\"" + s + "\"",
        double d => d.ToString("R"),
        bool or int or uint or char or long => o.ToString()!,
        IEnumerable e => "[" + string.Join(", ", e.Cast<object?>().Select(Text)) + "]",
        _ => o.ToString()!,
    };

    /// <summary>An <c>Mcd.Units</c> value: its text, both magnitudes and its base-unit powers. Maintainer-owned, since
    /// <see cref="Side"/> renders units with it.</summary>
    public static string UnitsText(object? u) => u == null ? "null"
        : $"{Outcome(u.ToString)} m={Text(Get(u, "Magnitude"))}/{Text(Get(u, "magnitude"))} [" + string.Join(" ", ((Array)Get(u, "Powers")!).Cast<object>()
            .Select(p => $"{Get(Get(p, "Base")!, "Symbol")}^{Get(Get(p, "Exponent")!, "Numerator")}/{Get(Get(p, "Exponent")!, "Denominator")}")) + "]";

    /// <summary>Records a difference between the two outcomes.</summary>
    public static void Same(List<string> diffs, string what, string quire, string mathcad)
    {
        if (quire != mathcad) diffs.Add($"{what}:\n  Quire   {quire}\n  Mathcad {mathcad}");
    }

    public static void Same(List<string> diffs, string what, Func<object?> quire, Func<object?> mathcad, Func<object?, string>? show = null) =>
        Same(diffs, what, Outcome(quire, show), Outcome(mathcad, show));

    public static void NoDiffs(List<string> diffs) =>
        Assert.True(diffs.Count == 0, $"{diffs.Count} differences:\n" + string.Join("\n", diffs.Take(25)));
}
