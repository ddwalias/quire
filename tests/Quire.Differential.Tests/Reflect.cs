using System.Collections;
using System.Reflection;

namespace Quire.Differential.Tests;

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
        var method = Methods(type, name).Single(m => Fits(m.GetParameters(), args));
        try { return method.Invoke(target is System.Type ? null : target, args); }
        catch (TargetInvocationException e) when (e.InnerException != null)
        {
            System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(e.InnerException).Throw();
            throw;
        }
    }

    /// <summary>Calls an overload chosen by its parameter type names, for overloads that the arguments don't settle.</summary>
    public static object? CallSig(object target, string name, string[] parameterTypes, params object?[] args)
    {
        var type = target as System.Type ?? target.GetType();
        var method = Methods(type, name).Single(m => m.GetParameters().Select(p => p.ParameterType.Name).SequenceEqual(parameterTypes));
        try { return method.Invoke(target is System.Type ? null : target, args); }
        catch (TargetInvocationException e) when (e.InnerException != null)
        {
            System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(e.InnerException).Throw();
            throw;
        }
    }

    /// <summary>The methods named <paramref name="name"/>, most derived first; an override hides what it overrides.</summary>
    private static IEnumerable<MethodInfo> Methods(System.Type type, string name)
    {
        var overridden = new HashSet<MethodInfo>();
        for (var t = type; t != null; t = t.BaseType)
            foreach (var m in t.GetMethods(All | BindingFlags.DeclaredOnly).Where(m => m.Name == name))
            {
                if (overridden.Contains(m)) continue;
                for (var b = m; b.GetBaseDefinition() != b && b.DeclaringType!.BaseType != null; )
                {
                    var parent = b.DeclaringType.BaseType.GetMethod(b.Name, All, b.GetParameters().Select(p => p.ParameterType).ToArray());
                    if (parent == null || parent == b) break;
                    overridden.Add(parent);
                    b = parent;
                }
                yield return m;
            }
    }

    private static bool Fits(ParameterInfo[] ps, object?[] args) =>
        ps.Length == args.Length && ps.Zip(args).All(p => p.Second == null ? !p.First.ParameterType.IsValueType
            : p.First.ParameterType.IsInstanceOfType(p.Second) || p.First.ParameterType.IsByRef);

    /// <summary>Creates an instance through the constructor whose parameters match the arguments; private ones too.</summary>
    public static object New(System.Type type, params object?[] args)
    {
        var ctor = type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .Single(c => Fits(c.GetParameters(), args));
        try { return ctor.Invoke(args); }
        catch (TargetInvocationException e) when (e.InnerException != null)
        {
            System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(e.InnerException).Throw();
            throw;
        }
    }

    /// <summary>A field or property value, public or not, instance or static.</summary>
    public static object? Get(object target, string name)
    {
        var type = target as System.Type ?? target.GetType();
        object? instance = target is System.Type ? null : target;
        for (var t = type; t != null; t = t.BaseType)
        {
            if (t.GetField(name, All | BindingFlags.DeclaredOnly) is { } f) return f.GetValue(instance);
            if (t.GetProperty(name, All | BindingFlags.DeclaredOnly) is { } p)
            {
                try { return p.GetValue(instance); }
                catch (TargetInvocationException e) when (e.InnerException != null)
                {
                    System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(e.InnerException).Throw();
                    throw;
                }
            }
        }
        throw new MissingMemberException(type.FullName, name);
    }

    public static void Set(object target, string name, object? value)
    {
        var type = target as System.Type ?? target.GetType();
        object? instance = target is System.Type ? null : target;
        for (var t = type; t != null; t = t.BaseType)
        {
            if (t.GetField(name, All | BindingFlags.DeclaredOnly) is { } f) { f.SetValue(instance, value); return; }
            if (t.GetProperty(name, All | BindingFlags.DeclaredOnly) is { } p)
            {
                try { p.SetValue(instance, value); return; }
                catch (TargetInvocationException e) when (e.InnerException != null)
                {
                    System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(e.InnerException).Throw();
                    throw;
                }
            }
        }
        throw new MissingMemberException(type.FullName, name);
    }

    /// <summary>A nested type, private ones included.</summary>
    public static System.Type Nested(System.Type outer, string name) =>
        outer.GetNestedType(name, BindingFlags.Public | BindingFlags.NonPublic) ?? throw new MissingMemberException(outer.FullName, name);

    /// <summary>The outcome of <paramref name="f"/> as text: <paramref name="show"/> of the value, or the exception's
    /// type and message.</summary>
    public static string Outcome(Func<object?> f, Func<object?, string>? show = null)
    {
        try { return (show ?? Text)(f()); }
        catch (Exception e) { return Thrown(e); }
    }

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
