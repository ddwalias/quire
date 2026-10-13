using Quire.Mpl.Rl;
using Quire.Values;

namespace Quire.Engine;

/// <summary>
/// Immutable, persistent scope. Each definition creates a new layer, so the scope seen by a
/// region (and captured by functions defined there) is exactly what was defined above it.
/// </summary>
public sealed class Env
{
    private readonly Env? _parent;
    private readonly string? _name;
    private readonly Value? _value;

    private Env(Env? parent, string? name, Value? value)
    {
        _parent = parent;
        _name = name;
        _value = value;
    }

    public static readonly Env Empty = new(null, null, null);

    public Env With(string name, Value value) => new(this, name, value);

    public bool TryLookupUser(string name, out Value value)
    {
        for (var e = this; e != null; e = e._parent)
        {
            if (e._name == name)
            {
                value = e._value!;
                return true;
            }
        }
        value = null!;
        return false;
    }

    public bool IsUserDefined(string name) => TryLookupUser(name, out _);

    /// <summary>Resolves a name used as a value: user definitions, then constants, units and built-in functions
    /// (so <c>min</c> and <c>sec</c> are minutes and seconds here).</summary>
    public Value Lookup(string name)
    {
        if (TryLookupUser(name, out var v)) return v;
        if (Units.Constants.TryGetValue(name, out var c)) return c;
        if (Units.TryGet(name, out var u)) return new NumVal(u.Factor, u.Dim);
        if (Units.TryGetAffine(name, out var a)) return new NumVal(a.Scale, Dim.Temperature);
        if (Builtins.TryGet(name, out var fn)) return fn;
        throw CalcException.Undefined(name);
    }

    /// <summary>Resolves a name in call position: user definitions, then built-in functions.</summary>
    public Value LookupFunction(string name)
    {
        if (TryLookupUser(name, out var v)) return v;
        if (Builtins.TryGet(name, out var fn)) return fn;
        return Lookup(name);
    }

    /// <summary>Bindings added on top of <paramref name="ancestor"/>, oldest first.</summary>
    public IReadOnlyList<(string Name, Value Value)> BindingsSince(Env ancestor)
    {
        var list = new List<(string, Value)>();
        for (var e = this; e != null && !ReferenceEquals(e, ancestor); e = e._parent)
            if (e._name != null) list.Add((e._name, e._value!));
        list.Reverse();
        return list;
    }

    /// <summary>Names defined in this scope, innermost first, without duplicates.</summary>
    public IEnumerable<(string Name, Value Value)> UserBindings()
    {
        var seen = new HashSet<string>();
        for (var e = this; e?._name != null; e = e._parent)
            if (seen.Add(e._name)) yield return (e._name, e._value!);
    }
}

public sealed class UserFunction(string name, IReadOnlyList<string> parameters, Sexpr body) : FunctionValue
{
    public override string Name { get; } = name;
    public override IReadOnlyList<string> Parameters { get; } = parameters;
    public Sexpr Body { get; } = body;

    /// <summary>The defining scope, which includes the function itself so it can recurse.</summary>
    public Env Closure { get; internal set; } = Env.Empty;
}

public sealed class BuiltinFunction(string name, int minArgs, int maxArgs, Func<Value[], Interpreter, Value> impl) : FunctionValue
{
    public override string Name { get; } = name;
    public override IReadOnlyList<string> Parameters { get; } = Enumerable.Range(1, Math.Max(minArgs, 1)).Select(i => $"x{i}").ToArray();
    public int MinArgs { get; } = minArgs;
    public int MaxArgs { get; } = maxArgs;

    public Value Invoke(Value[] args, Interpreter interp)
    {
        if (args.Length < MinArgs || args.Length > MaxArgs)
        {
            var expected = MinArgs == MaxArgs ? $"{MinArgs}" : MaxArgs == int.MaxValue ? $"at least {MinArgs}" : $"{MinArgs}–{MaxArgs}";
            throw new CalcException($"{Name} expects {expected} argument(s), got {args.Length}", Name);
        }
        return impl(args, interp);
    }
}
