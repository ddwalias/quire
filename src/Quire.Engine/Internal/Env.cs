using Quire.Mcd;
using Quire.Mpl.Internal;
using Quire.Mpl.Run;

namespace Quire.Engine.Internal;

/// <summary>
/// Immutable, persistent scope. Each definition creates a new layer, so the scope seen by a
/// region (and captured by functions defined there) is exactly what was defined above it.
/// </summary>
internal sealed class Env
{
    private readonly Env? _parent;
    private readonly string? _name;
    private readonly DimVal? _value;

    private Env(Env? parent, string? name, DimVal? value)
    {
        _parent = parent;
        _name = name;
        _value = value;
    }

    public static readonly Env Empty = new(null, null, null);

    public Env With(string name, DimVal value) => new(this, name, value);

    public bool TryLookupUser(string name, out DimVal value)
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
    public DimVal Lookup(string name)
    {
        if (TryLookupUser(name, out var v)) return v;
        if (BuiltinUnits.Constants.TryGetValue(name, out var c)) return c;
        if (BuiltinUnits.TryGet(name, out var u)) return Val.Num(u.Magnitude, BuiltinUnits.DimOf(u));
        if (BuiltinUnits.TryGetAffine(name, out var a)) return Val.Num(a.Scale, Val.Temperature);
        if (Builtins.TryGet(name, out var fn)) return fn;
        throw CalcException.Undefined(name);
    }

    /// <summary>Resolves a name in call position: user definitions, then built-in functions.</summary>
    public DimVal LookupFunction(string name)
    {
        if (TryLookupUser(name, out var v)) return v;
        if (Builtins.TryGet(name, out var fn)) return fn;
        return Lookup(name);
    }

    /// <summary>Bindings added on top of <paramref name="ancestor"/>, oldest first.</summary>
    public IReadOnlyList<(string Name, DimVal Value)> BindingsSince(Env ancestor)
    {
        var list = new List<(string, DimVal)>();
        for (var e = this; e != null && !ReferenceEquals(e, ancestor); e = e._parent)
            if (e._name != null) list.Add((e._name, e._value!));
        list.Reverse();
        return list;
    }
}

/// <summary>A function defined in the worksheet, <c>f(x, y) := body</c>.</summary>
internal sealed class UserFunction : Function
{
    public UserFunction(string name, IReadOnlyList<string> parameters, Sexpr body, Interpreter interpreter) : base(0, parameters.Count)
    {
        m_name = name;
        Parameters = parameters;
        Body = body;
        Interpreter = interpreter;
    }

    public IReadOnlyList<string> Parameters { get; }
    public Sexpr Body { get; }

    /// <summary>The interpreter that defined the function; calls run on it.</summary>
    public Interpreter Interpreter { get; }

    /// <summary>The defining scope, which includes the function itself so it can recurse.</summary>
    public Env Closure { get; set; } = Env.Empty;

    public override void invVec<R>(out R r, object[] args) =>
        r = (R)(object)Interpreter.Call(this, args.Cast<DimVal>().ToArray());

    public override string ToString() => $"{Name}({string.Join(", ", Parameters)})";
}

/// <summary>A built-in function taking <see cref="Function.MinArgs"/> to <see cref="MaxArgs"/> arguments.</summary>
internal sealed class BuiltinFunction : Function
{
    private readonly Func<DimVal[], Interpreter, DimVal> _impl;

    public BuiltinFunction(string name, int minArgs, int maxArgs, Func<DimVal[], Interpreter, DimVal> impl) : base(0, minArgs)
    {
        m_name = name;
        m_varargs = maxArgs > minArgs;
        MaxArgs = maxArgs;
        _impl = impl;
    }

    public int MaxArgs { get; }

    public DimVal Invoke(DimVal[] args, Interpreter interp)
    {
        if (args.Length < MinArgs || args.Length > MaxArgs)
        {
            var expected = MinArgs == MaxArgs ? $"{MinArgs}" : MaxArgs == int.MaxValue ? $"at least {MinArgs}" : $"{MinArgs}–{MaxArgs}";
            throw new CalcException($"{Name} expects {expected} argument(s), got {args.Length}", Name);
        }
        return _impl(args, interp);
    }

    /// <summary>Called from outside an interpreter: index-based built-ins read ORIGIN as 0.</summary>
    public override void invVec<R>(out R r, object[] args) =>
        r = (R)(object)Invoke(args.Cast<DimVal>().ToArray(), new Interpreter());

    public override string ToString() =>
        $"{Name}({string.Join(", ", Enumerable.Range(1, Math.Max(MinArgs, 1)).Select(i => $"x{i}"))})";
}
