namespace Quire.Engine;

/// <summary>A calculation error raised while evaluating a node. <see cref="Symbol"/> names the offending identifier, if any.</summary>
public sealed class CalcException(string message, string? symbol = null) : Exception(message)
{
    public string? Symbol { get; } = symbol;

    public static CalcException Undefined(string name) => new($"'{name}' is not defined", name);
    public static CalcException UnitsMismatch(string op) => new($"Units don't match ({op})");
}
