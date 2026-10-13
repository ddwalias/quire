using System.Text;

namespace Quire.Tools.MirrorGen;

/// <summary>
/// Turns Mathcad names into C# identifiers. Valid identifiers pass through unchanged. Others (Mpl operators such as
/// <c>==.</c>, primed names such as <c>abs'</c>) are spelled out: <c>_eq_eq_dot</c>, <c>abs_prime</c>. [MirrorOf] keeps
/// the original type name; the generator reports any two names in one scope that would spell the same.
/// </summary>
public static class Identifier
{
    private static readonly Dictionary<char, string> Tokens = new()
    {
        ['!'] = "bang", ['='] = "eq", ['+'] = "plus", ['-'] = "minus", ['*'] = "star", ['/'] = "slash", ['%'] = "pct",
        ['>'] = "gt", ['<'] = "lt", ['.'] = "dot", [':'] = "colon", ['#'] = "hash", ['\''] = "prime", ['?'] = "q",
        ['&'] = "amp", ['|'] = "bar", ['^'] = "caret", ['~'] = "tilde", ['@'] = "at", ['$'] = "dollar", [' '] = "sp",
    };

    public static bool IsValid(string name)
    {
        if (name.Length == 0 || !(char.IsLetter(name[0]) || name[0] == '_')) return false;
        foreach (char c in name) if (!(char.IsLetterOrDigit(c) || c == '_')) return false;
        return true;
    }

    /// <summary>Encodes a simple name. A generic arity suffix (<c>`2</c>) is kept as is.</summary>
    public static string Encode(string name)
    {
        int tick = name.LastIndexOf('`');
        string suffix = "";
        if (tick > 0 && int.TryParse(name.AsSpan(tick + 1), out _)) { suffix = name[tick..]; name = name[..tick]; }
        if (IsValid(name)) return name + suffix;
        var sb = new StringBuilder();
        foreach (char c in name)
        {
            if (char.IsLetterOrDigit(c) || c == '_') sb.Append(c);
            else sb.Append('_').Append(Tokens.TryGetValue(c, out var t) ? t : $"u{(int)c:X4}");
        }
        if (char.IsDigit(sb[0])) sb.Insert(0, '_');
        return sb + suffix;
    }

    private static readonly HashSet<string> Keywords =
    [
        "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked", "class", "const", "continue",
        "decimal", "default", "delegate", "do", "double", "else", "enum", "event", "explicit", "extern", "false", "finally",
        "fixed", "float", "for", "foreach", "goto", "if", "implicit", "in", "int", "interface", "internal", "is", "lock",
        "long", "namespace", "new", "null", "object", "operator", "out", "override", "params", "private", "protected",
        "public", "readonly", "ref", "return", "sbyte", "sealed", "short", "sizeof", "stackalloc", "static", "string",
        "struct", "switch", "this", "throw", "true", "try", "typeof", "uint", "ulong", "unchecked", "unsafe", "ushort",
        "using", "virtual", "void", "volatile", "while",
    ];

    /// <summary>The identifier as written in C# source: keywords get an @ prefix; the arity suffix is dropped.</summary>
    public static string Source(string encoded)
    {
        int tick = encoded.LastIndexOf('`');
        if (tick > 0) encoded = encoded[..tick];
        return Keywords.Contains(encoded) ? "@" + encoded : encoded;
    }
}
