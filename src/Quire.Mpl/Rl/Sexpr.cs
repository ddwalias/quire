using System.Globalization;
using System.Text;

namespace Quire.Mpl.Rl;

/// <summary>
/// S-expression node. The UI layer serialises every calculatable region into an
/// "Rl" S-expression such as <c>(define x (op_mult 5 m))</c> which the engine evaluates.
/// Mathcad's own S-expression types are mirrored separately, in Quire.Mpl.Run.
/// </summary>
public abstract class Sexpr
{
    public override string ToString() => SexprWriter.Write(this);
}

public sealed class Sym(string name) : Sexpr
{
    public string Name { get; } = name;
}

public sealed class Atom : Sexpr
{
    private Atom(double? number, string? text) { Number = number; Text = text; }

    public double? Number { get; }
    public string? Text { get; }
    public bool IsNumber => Number.HasValue;

    public static Atom Num(double v) => new(v, null);
    public static Atom Str(string s) => new(null, s);
}

public sealed class SList(IReadOnlyList<Sexpr> items) : Sexpr
{
    public IReadOnlyList<Sexpr> Items { get; } = items;
    public int Count => Items.Count;
    public Sexpr this[int i] => Items[i];

    /// <summary>The head symbol name, or null when the list is empty or headed by a non-symbol.</summary>
    public string? Head => Items.Count > 0 && Items[0] is Sym s ? s.Name : null;

    public IEnumerable<Sexpr> Args => Items.Skip(1);

    public static SList Of(params Sexpr[] items) => new(items);
    public static SList Of(string head, params Sexpr[] args) => new([new Sym(head), .. args]);
}

public static class SexprWriter
{
    public static string Write(Sexpr e)
    {
        var sb = new StringBuilder();
        Write(e, sb);
        return sb.ToString();
    }

    private static void Write(Sexpr e, StringBuilder sb)
    {
        switch (e)
        {
            case Sym s:
                sb.Append(NeedsQuoting(s.Name) ? "|" + s.Name.Replace("|", "\\|") + "|" : s.Name);
                break;
            case Atom { IsNumber: true } a:
                sb.Append(a.Number!.Value.ToString("R", CultureInfo.InvariantCulture));
                break;
            case Atom a:
                sb.Append('"').Append(a.Text!.Replace("\\", "\\\\").Replace("\"", "\\\"")).Append('"');
                break;
            case SList l:
                sb.Append('(');
                for (int i = 0; i < l.Count; i++)
                {
                    if (i > 0) sb.Append(' ');
                    Write(l[i], sb);
                }
                sb.Append(')');
                break;
        }
    }

    private static bool NeedsQuoting(string name) =>
        name.Length == 0 || name.Any(c => char.IsWhiteSpace(c) || c is '(' or ')' or '"' or '|')
        || double.TryParse(name, NumberStyles.Float, CultureInfo.InvariantCulture, out _);
}

public sealed class SexprReader
{
    private readonly string _src;
    private int _pos;

    private SexprReader(string src) => _src = src;

    public static Sexpr Read(string src)
    {
        var r = new SexprReader(src);
        var e = r.ReadOne();
        r.SkipWs();
        if (r._pos != src.Length) throw new FormatException($"Trailing input at {r._pos}");
        return e;
    }

    private void SkipWs()
    {
        while (_pos < _src.Length && char.IsWhiteSpace(_src[_pos])) _pos++;
    }

    private Sexpr ReadOne()
    {
        SkipWs();
        if (_pos >= _src.Length) throw new FormatException("Unexpected end of S-expression");
        char c = _src[_pos];
        if (c == '(')
        {
            _pos++;
            var items = new List<Sexpr>();
            for (;;)
            {
                SkipWs();
                if (_pos >= _src.Length) throw new FormatException("Missing ')'");
                if (_src[_pos] == ')') { _pos++; return new SList(items); }
                items.Add(ReadOne());
            }
        }
        if (c == ')') throw new FormatException($"Unexpected ')' at {_pos}");
        if (c == '"') return Atom.Str(ReadDelimited('"'));
        if (c == '|') return new Sym(ReadDelimited('|'));

        int start = _pos;
        while (_pos < _src.Length && !char.IsWhiteSpace(_src[_pos]) && _src[_pos] is not ('(' or ')' or '"')) _pos++;
        var tok = _src[start.._pos];
        if (double.TryParse(tok, NumberStyles.Float, CultureInfo.InvariantCulture, out var d)) return Atom.Num(d);
        return new Sym(tok);
    }

    private string ReadDelimited(char delim)
    {
        _pos++;
        var sb = new StringBuilder();
        while (_pos < _src.Length && _src[_pos] != delim)
        {
            if (_src[_pos] == '\\' && _pos + 1 < _src.Length) _pos++;
            sb.Append(_src[_pos++]);
        }
        if (_pos >= _src.Length) throw new FormatException($"Missing closing {delim}");
        _pos++;
        return sb.ToString();
    }
}
