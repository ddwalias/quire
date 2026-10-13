using System.Windows;
using System.Windows.Media;
using Quire.EquationEditor.Tree;
using Quire.Values;

namespace Quire.EquationEditor.View;

/// <summary>Fonts and colours used to draw math.</summary>
public sealed record MathStyle
{
    public FontFamily TextFont { get; init; } = new("Segoe UI");
    public FontFamily SymbolFont { get; init; } = new("Cambria Math");
    public double FontSize { get; init; } = 14.667;
    public Brush Foreground { get; init; } = Brushes.Black;
    public Brush UnitBrush { get; init; } = Freeze(new SolidColorBrush(Color.FromRgb(0x1F, 0x5F, 0x8B)));
    public Brush ResultBrush { get; init; } = Brushes.Black;
    public Brush ErrorHighlight { get; init; } = Freeze(new SolidColorBrush(Color.FromArgb(0x60, 0xE0, 0x30, 0x30)));
    public Brush PlaceholderBrush { get; init; } = Brushes.Gray;
    public Brush TableBrush { get; init; } = Brushes.Gray;
    public double PixelsPerDip { get; init; } = 1;

    public static readonly MathStyle Default = new();

    private static Brush Freeze(Brush b)
    {
        b.Freeze();
        return b;
    }
}

/// <summary>Builds layout boxes for statements, expressions and formatted results.</summary>
public sealed class VisualNodeBuilder(MathStyle style, string? errorSymbol = null)
{
    private const double ScriptScale = 0.72;

    private static readonly Dictionary<string, string> DisplayNames = new()
    {
        ["pi"] = "π", ["inf"] = "∞", ["alpha"] = "α", ["beta"] = "β", ["gamma"] = "γ", ["delta"] = "δ",
        ["theta"] = "θ", ["lambda"] = "λ", ["mu"] = "μ", ["sigma"] = "σ", ["omega"] = "ω", ["phi"] = "φ",
        ["epsilon"] = "ε", ["rho"] = "ρ", ["tau"] = "τ", ["eta"] = "η", ["Delta"] = "Δ", ["Omega"] = "Ω",
    };

    private static readonly Dictionary<string, (string Text, int Prec)> BinaryDisplay = new()
    {
        ["or"] = (" ∨ ", 1), ["and"] = (" ∧ ", 2),
        ["eq"] = (" = ", 4), ["neq"] = (" ≠ ", 4), ["lt"] = (" < ", 4), ["gt"] = (" > ", 4), ["leq"] = (" ≤ ", 4), ["geq"] = (" ≥ ", 4),
        ["add"] = (" + ", 5), ["sub"] = (" − ", 5), ["mult"] = ("·", 6), ["div"] = ("/", 6), ["pow"] = ("^", 8),
    };

    private double _size = style.FontSize;

    private double Axis => _size * 0.28;

    // ---------------------------------------------------------------- statements

    /// <param name="result">Formatted result tree to show after "=", if any.</param>
    /// <param name="resultIsTable">Render a column result as a framed table.</param>
    public Box BuildStatement(Statement statement, MathExpr? result = null, bool resultIsTable = false)
    {
        _size = style.FontSize;
        switch (statement)
        {
            case DefineStatement d:
            {
                var parts = new List<Box> { BuildTarget(d.Target), Op(d.Global ? " ≡ " : " := "), Build(d.Value, 0) };
                if (d.ShowResult) parts.AddRange(ResultParts(result, d.Unit, resultIsTable));
                return new HBox(parts);
            }
            case EvalStatement e:
                return new HBox([Build(e.Expr, 0), .. ResultParts(result, e.Unit, resultIsTable)]);
            case ExprStatement x:
                return Build(x.Expr, 0);
            case UnparsedStatement u:
                return Text(u.Source, highlight: style.ErrorHighlight);
            default:
                return Placeholder();
        }
    }

    private IEnumerable<Box> ResultParts(MathExpr? result, MathExpr? unit, bool table)
    {
        yield return Op(" = ");
        if (result == null)
        {
            yield return Placeholder();
            yield break;
        }
        yield return table && result is MatrixNode m ? Table(m) : Build(result, 0);
        if (unit != null)
        {
            yield return new SpaceBox(_size * 0.25);
            yield return Build(unit, 0);
        }
    }

    private Box BuildTarget(MathExpr target) => target switch
    {
        IdNode id => Identifier(id, function: true),
        CallNode c => new HBox(Identifier(new IdNode(c.Name)), Fence(Join(c.Args.Select(a => Build(a, 0))), '(', ')')),
        _ => Build(target, 0),
    };

    // ---------------------------------------------------------------- expressions

    public Box BuildExpression(MathExpr node)
    {
        _size = style.FontSize;
        return Build(node, 0);
    }

    private static int Precedence(MathExpr n) => n switch
    {
        BinaryNode b => BinaryDisplay[b.Op].Prec,
        UnaryNode { Op: "neg" } => 7,
        UnaryNode { Op: "not" } => 3,
        RangeNode => 0,
        _ => 10,
    };

    private Box Build(MathExpr node, int minPrec)
    {
        var box = BuildCore(node);
        return Precedence(node) < minPrec ? Fence(box, '(', ')') : box;
    }

    private Box BuildCore(MathExpr node)
    {
        switch (node)
        {
            case IdNode id:
                return Identifier(id);
            case NumberNode n:
                return Text(n.Text);
            case StringNode s:
                return Text($"“{s.Value}”");
            case UnaryNode { Op: "neg" } u:
                return new HBox(Op("−"), Build(u.Operand, 7));
            case UnaryNode { Op: "not" } u:
                return new HBox(Op("¬"), Build(u.Operand, 3));
            case UnaryNode { Op: "factorial" } u:
                return new HBox(Build(u.Operand, 9), Op("!"));
            case BinaryNode { Op: "div" } b:
                return new FractionBox(Scaled(0.9, () => Build(Unwrap(b.Left), 0)), Scaled(0.9, () => Build(Unwrap(b.Right), 0)),
                    Axis, Math.Max(1, _size / 16), style.Foreground);
            case BinaryNode { Op: "pow" } b:
                return new ScriptBox(Build(b.Left, 9), Script(() => Build(Unwrap(b.Right), 0)), null, _size);
            case BinaryNode { Op: "mult" } b:
            {
                bool thin = b.Implicit || IsUnit(b.Right);
                var sep = thin ? new SpaceBox(_size * 0.2) : Op("·");
                return new HBox(Build(b.Left, 6), sep, Build(b.Right, 7));
            }
            case BinaryNode b:
            {
                var (text, prec) = BinaryDisplay[b.Op];
                var op = b.Op == "eq" ? Op(text, bold: true) : Op(text);
                return new HBox(Build(b.Left, prec), op, Build(b.Right, prec + 1));
            }
            case ParensNode p:
                return Fence(Build(p.Inner, 0), '(', ')');
            case CallNode c:
                return Call(c);
            case IndexNode ix:
                return new ScriptBox(Build(ix.Target, 9), null, Script(() => Join(ix.Indices.Select(i => Build(i, 0)))), _size);
            case MatrixNode m:
                return Fence(Grid(m, table: false), '[', ']');
            case RangeNode r:
                return r.Second == null
                    ? new HBox(Build(r.Start, 1), Op(" .. "), Build(r.End, 1))
                    : new HBox(Build(r.Start, 1), Op(", "), Build(r.Second, 1), Op(" .. "), Build(r.End, 1));
            case PlaceholderNode:
                return Placeholder();
            default:
                return Text("?");
        }
    }

    private static MathExpr Unwrap(MathExpr n) => n is ParensNode p ? p.Inner : n;

    private Box Call(CallNode c)
    {
        var a = c.Args;
        switch (c.Name, a.Count)
        {
            case ("sqrt", 1):
                return new RadicalBox(Build(a[0], 0), null, _size, style.Foreground);
            case ("nthroot", 2):
                return new RadicalBox(Build(a[0], 0), Script(() => Build(a[1], 0)), _size, style.Foreground);
            case ("abs", 1):
                return Fence(Build(a[0], 0), '|', '|');
            case ("transpose", 1):
                return new ScriptBox(Build(a[0], 9), Script(() => Text("T")), null, _size);
            case ("sum" or "prod", 4):
                return new HBox(new BigOpBox(BigSymbol(c.Name == "sum" ? "∑" : "∏"),
                        Script(() => new HBox(Build(a[1], 0), Op("="), Build(a[2], 0))), Script(() => Build(a[3], 0)), false, _size),
                    new SpaceBox(_size * 0.15), Build(a[0], 6));
            case ("sum" or "prod", 2):
                return new HBox(new BigOpBox(BigSymbol(c.Name == "sum" ? "∑" : "∏"), Script(() => Build(a[1], 0)), null, false, _size),
                    new SpaceBox(_size * 0.15), Build(a[0], 6));
            case ("integral", 4):
                return new HBox(new BigOpBox(BigSymbol("∫", 1.6), Script(() => Build(a[2], 0)), Script(() => Build(a[3], 0)), true, _size),
                    new SpaceBox(_size * 0.1), Build(a[0], 6), new SpaceBox(_size * 0.2), Text("d"), Build(a[1], 10));
            case ("deriv", 2 or 3):
            {
                Box d = Text("d");
                Box num = a.Count == 3 ? new ScriptBox(Text("d"), Script(() => Build(a[2], 0)), null, _size) : d;
                Box dx = a.Count == 3
                    ? new HBox(Text("d"), new ScriptBox(Build(a[1], 10), Script(() => Build(a[2], 0)), null, _size))
                    : new HBox(Text("d"), Build(a[1], 10));
                return new HBox(new FractionBox(num, dx, Axis, Math.Max(1, _size / 16), style.Foreground), new SpaceBox(_size * 0.1), Build(a[0], 6));
            }
        }
        return new HBox(Identifier(new IdNode(c.Name), function: true), Fence(Join(a.Select(x => Build(x, 0))), '(', ')'));
    }

    // ---------------------------------------------------------------- primitives

    private Typeface Face(bool bold = false, bool symbol = false, bool italic = false) =>
        new(symbol ? style.SymbolFont : style.TextFont, italic ? FontStyles.Italic : FontStyles.Normal,
            bold ? FontWeights.Bold : FontWeights.Normal, FontStretches.Normal);

    private Box Text(string s, Brush? brush = null, Brush? highlight = null) =>
        new GlyphBox(s, Face(), _size, brush ?? style.Foreground, style.PixelsPerDip, highlight);

    private Box Op(string s, bool bold = false) =>
        new GlyphBox(s, Face(bold: bold), _size, style.Foreground, style.PixelsPerDip);

    private Box BigSymbol(string s, double scale = 1.35) =>
        new GlyphBox(s, Face(symbol: true), _size * scale, style.Foreground, style.PixelsPerDip);

    private Box Placeholder() => new GlyphBox("■", Face(), _size * 0.8, style.PlaceholderBrush, style.PixelsPerDip);

    private static string Display(string name) => DisplayNames.GetValueOrDefault(name, name);

    private static bool IsUnit(MathExpr n) => n switch
    {
        IdNode id => Units.IsUnit(id.Name),
        BinaryNode { Op: "pow" } p => IsUnit(p.Left),
        BinaryNode { Op: "div" or "mult" } b => IsUnit(b.Left) && IsUnit(b.Right),
        _ => false,
    };

    private Box Identifier(IdNode id, bool function = false)
    {
        var highlight = errorSymbol != null && id.Name == errorSymbol ? style.ErrorHighlight : null;
        var brush = !function && Units.IsUnit(id.Name) ? style.UnitBrush : style.Foreground;
        var sub = id.Subscript;
        if (sub == null) return Text(Display(id.Name), brush, highlight);
        var baseBox = Text(Display(id.BaseName), brush, highlight);
        return new ScriptBox(baseBox, null, Script(() => Text(Display(sub), brush, highlight)), _size);
    }

    private Box Fence(Box content, char open, char close) => new FenceBox(content, open, close, _size, style.Foreground);

    private Box Join(IEnumerable<Box> items)
    {
        var list = new List<Box>();
        foreach (var b in items)
        {
            if (list.Count > 0) list.Add(Op(", "));
            list.Add(b);
        }
        return new HBox(list);
    }

    private Box Script(Func<Box> build) => Scaled(ScriptScale, build);

    private Box Scaled(double factor, Func<Box> build)
    {
        var saved = _size;
        _size = Math.Max(saved * factor, style.FontSize * 0.5);
        try { return build(); }
        finally { _size = saved; }
    }

    private Box Grid(MatrixNode m, bool table)
    {
        var cells = new Box[m.Rows, m.Cols];
        for (int r = 0; r < m.Rows; r++)
            for (int c = 0; c < m.Cols; c++)
                cells[r, c] = Build(m[r, c], 0);
        return new GridBox(cells, _size, Axis, table ? style.TableBrush : null);
    }

    private Box Table(MatrixNode m) => Grid(m, table: true);
}
