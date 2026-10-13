using System.Globalization;
using System.Text;
using Quire.Mcd;
using Quire.Mpl.Run;

namespace Quire.Engine.Internal;

/// <summary>Access to Rl statements, which arrive as <see cref="Sexpr"/> lists of <see cref="Tok"/> atoms.</summary>
internal static class Rl
{
    /// <summary>The head symbol of a list, or null when the list is empty or headed by something else.</summary>
    public static string? Head(Sexpr? e) => e is SCons { Field1: Tok { Field1: Sym s } } ? s.name : null;

    /// <summary>The name of a symbol token, or null.</summary>
    public static string? SymName(Sexpr? e) => e is Tok { Field1: Sym s } ? s.name : null;

    public static int Count(SCons l) => (int)l.Length;

    /// <summary>Element <paramref name="i"/> of a list; 0 is the head.</summary>
    public static Sexpr Nth(SCons l, int i)
    {
        Sexpr e = l;
        for (; i > 0 && e is SCons c; i--) e = c.Field2;
        return e is SCons cell ? cell.Field1 : throw new CalcException("Malformed expression: an argument is missing");
    }

    public static Sexpr[] Items(Sexpr l)
    {
        var list = new List<Sexpr>();
        for (var e = l; e is SCons c; e = c.Field2) list.Add(c.Field1);
        return [.. list];
    }

    /// <summary>The elements after the head.</summary>
    public static Sexpr[] Args(SCons l) => Items(l.Field2);

    /// <summary>The sub-list headed by <paramref name="name"/> among the elements of <paramref name="l"/>.</summary>
    public static SCons? Part(SCons l, string name) =>
        Items(l).OfType<SCons>().FirstOrDefault(s => Head(s) == name);
}

/// <summary>
/// Reads Rl text into S-expressions: <c>( … )</c> lists, numbers, <c>"…"</c> strings and symbols (<c>|…|</c> when
/// they contain delimiters). Each token's handle is its 1-based offset in the text.
/// </summary>
internal sealed class RlReader
{
    private readonly string _src;
    private int _pos;

    private RlReader(string src) => _src = src;

    public static Sexpr Read(string src)
    {
        var r = new RlReader(src);
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
        uint handle = (uint)_pos + 1;
        if (c == '(')
        {
            _pos++;
            var items = new List<Sexpr>();
            for (;;)
            {
                SkipWs();
                if (_pos >= _src.Length) throw new FormatException("Missing ')'");
                if (_src[_pos] == ')') { _pos++; break; }
                items.Add(ReadOne());
            }
            Sexpr list = Nul.singleton;
            for (int i = items.Count - 1; i >= 0; i--) list = new SCons(items[i], list);
            return list;
        }
        if (c == ')') throw new FormatException($"Unexpected ')' at {_pos}");
        if (c == '"') return new Tok(new str_constant(ReadDelimited('"')), handle);
        if (c == '|') return new Tok(new Sym(ReadDelimited('|'), null), handle);

        int start = _pos;
        while (_pos < _src.Length && !char.IsWhiteSpace(_src[_pos]) && _src[_pos] is not ('(' or ')' or '"')) _pos++;
        var tok = _src[start.._pos];
        if (double.TryParse(tok, NumberStyles.Float, CultureInfo.InvariantCulture, out var d))
            return new Tok(new real_constant(d), handle);
        return new Tok(new Sym(tok, null), handle);
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

/// <summary>Constructors and views over the mirrored value types that the interpreter uses throughout.</summary>
internal static class Val
{
    public const int MaxElements = 10_000_000;

    public static readonly Dim Temperature = Dim.Parse("temperature");

    public static NumVal Num(double v, Dim d) => new(v, 0, d);

    public static NumVal Bool(bool b) => NumVal.McdBool(b);

    public static void CheckSize(int rows, int cols)
    {
        if (rows < 0 || cols < 0 || (long)rows * cols > MaxElements)
            throw new CalcException($"A {rows}×{cols} matrix is too large (limit {MaxElements:N0} elements)");
    }

    /// <summary>A matrix of numbers; its shared dimension is that of the first element.</summary>
    public static MatVal Create(int rows, int cols, Func<int, int, NumVal> f)
    {
        CheckSize(rows, cols);
        var items = new NumVal[rows, cols];
        for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++) items[r, c] = f(r, c);
        var m = new MatVal(rows, cols) { m_dim = rows > 0 && cols > 0 ? items[0, 0].m_dim : Dim.identity };
        for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++) m.set_2(r, c, items[r, c]);
        return m;
    }

    public static MatVal Column(IReadOnlyList<NumVal> items) => Create(items.Count, 1, (r, _) => items[r]);

    public static MatVal Map(this MatVal m, Func<NumVal, NumVal> f) => Create(m.Rows, m.Cols, (r, c) => f(m.At(r, c)));

    public static NumVal At(this MatVal m, int r, int c) =>
        m.get_2(r, c) as NumVal ?? throw new CalcException("The matrix holds an element that isn't a number");

    /// <summary>Element <paramref name="i"/> in row-major order.</summary>
    public static NumVal At(this MatVal m, int i) => m.At(i / m.Cols, i % m.Cols);

    public static int Length(this MatVal m) => m.Rows * m.Cols;

    /// <summary>The elements in row-major order.</summary>
    public static IReadOnlyList<NumVal> Elements(this MatVal m)
    {
        var list = new NumVal[m.Length()];
        for (int i = 0; i < list.Length; i++) list[i] = m.At(i);
        return list;
    }

    public static IEnumerable<NumVal> Values(this RngVal r)
    {
        for (int i = 0; i < r.Rsize; i++) yield return (NumVal)r.range_get(i);
    }

    public static MatVal ToColumn(this RngVal r) => r.asVector("range", 0);
}
