using System.Globalization;
using System.Text;

namespace Quire.EquationEditor.Tree.Parsing;

public sealed class ParseException(string message, int position) : Exception(message)
{
    public int Position { get; } = position;
}

internal enum TokenKind { Number, String, Id, Op, End }

internal readonly record struct Token(TokenKind Kind, string Text, int Pos, bool SpaceBefore);

/// <summary>
/// Parses the linear keyboard syntax of a math region into a <see cref="Statement"/>.
/// <code>
///   x := expr          definition              f(x, y) := expr     function
///   x ≡ expr  (or ~)   global definition       v[i] := expr        element definition
///   expr =             evaluation              expr = km/hr        evaluation with unit override
///   x := expr =        define and show         a == b              comparison / constraint (bold =)
///   x := 0, 0.5 .. 10  range variable          [1, 2; 3, 4]        matrix (rows separated by ;)
/// </code>
/// </summary>
public sealed class LinearParser
{
    private static readonly string[] Symbols =
    [
        ":=", "≡", "..", "==", "!=", "<=", ">=", "≠", "≤", "≥", "+", "-", "−", "*", "·", "×", "/", "÷", "^", "(", ")",
        "[", "]", ",", ";", "<", ">", "=", "!", "√", "|", ":", "~", "¬", "∧", "∨", "ᵀ", "∑", "Σ", "∏", "∫",
    ];

    private static readonly Dictionary<string, string> Aliases = new()
    {
        ["−"] = "-", ["·"] = "*", ["×"] = "*", ["÷"] = "/", ["≠"] = "!=", ["≤"] = "<=", ["≥"] = ">=", ["~"] = "≡",
        ["¬"] = "not", ["∧"] = "and", ["∨"] = "or", ["∑"] = "Σ",
    };

    private static readonly Dictionary<string, string> SymbolFunctions = new()
    {
        ["Σ"] = "sum", ["∏"] = "prod", ["∫"] = "integral", ["√"] = "sqrt",
    };

    private static readonly Dictionary<string, string> ComparisonOps = new()
    {
        ["=="] = "eq", ["!="] = "neq", ["<="] = "leq", [">="] = "geq", ["<"] = "lt", [">"] = "gt",
    };

    private readonly List<Token> _toks;
    private int _i;

    private LinearParser(string src) => _toks = Tokenize(src);

    public static Statement ParseStatement(string src) => new LinearParser(src).Statement();

    public static MathExpr ParseExpression(string src)
    {
        var p = new LinearParser(src);
        var e = p.Expr();
        p.End();
        return e;
    }

    /// <summary>Parses, returning null and the error instead of throwing.</summary>
    public static Statement? TryParseStatement(string src, out ParseException? error)
    {
        try
        {
            error = null;
            return ParseStatement(src);
        }
        catch (ParseException ex)
        {
            error = ex;
            return null;
        }
    }

    // ---------------------------------------------------------------- lexer

    internal static bool IsIdStart(char c) =>
        (char.IsLetter(c) && c != 'ᵀ') || c is '_' or '°' or 'µ' or 'Ω' or 'Å' or '∞' or '%' or '$' or 'Δ' or 'Γ' or 'π';

    internal static bool IsIdPart(char c) => IsIdStart(c) || char.IsDigit(c) || c == '\'';

    private static List<Token> Tokenize(string src)
    {
        var toks = new List<Token>();
        int i = 0;
        while (i < src.Length)
        {
            int start = i;
            char c = src[i];
            if (char.IsWhiteSpace(c)) { i++; continue; }
            bool space = start > 0 && char.IsWhiteSpace(src[start - 1]);

            if (char.IsDigit(c) || (c == '.' && i + 1 < src.Length && char.IsDigit(src[i + 1])))
            {
                while (i < src.Length && char.IsDigit(src[i])) i++;
                if (i < src.Length && src[i] == '.' && !(i + 1 < src.Length && src[i + 1] == '.'))
                {
                    i++;
                    while (i < src.Length && char.IsDigit(src[i])) i++;
                }
                if (i + 1 < src.Length && src[i] is 'e' or 'E'
                    && (char.IsDigit(src[i + 1]) || (src[i + 1] is '+' or '-' && i + 2 < src.Length && char.IsDigit(src[i + 2]))))
                {
                    i += 2;
                    while (i < src.Length && char.IsDigit(src[i])) i++;
                }
                toks.Add(new Token(TokenKind.Number, src[start..i], start, space));
                continue;
            }
            if (c == '"')
            {
                int end = src.IndexOf('"', i + 1);
                if (end < 0) throw new ParseException("Unterminated string", start);
                toks.Add(new Token(TokenKind.String, src[(i + 1)..end], start, space));
                i = end + 1;
                continue;
            }
            if (c == '`')
            {
                int end = src.IndexOf('`', i + 1);
                if (end < 0) throw new ParseException("Unterminated `name`", start);
                toks.Add(new Token(TokenKind.Id, src[(i + 1)..end], start, space));
                i = end + 1;
                continue;
            }
            if (IsIdStart(c))
            {
                while (i < src.Length && IsIdPart(src[i])) i++;
                toks.Add(new Token(TokenKind.Id, src[start..i], start, space));
                continue;
            }
            var sym = Symbols.FirstOrDefault(s => string.CompareOrdinal(src, i, s, 0, s.Length) == 0);
            if (sym == null) throw new ParseException($"Unexpected character '{c}'", i);
            i += sym.Length;
            toks.Add(new Token(TokenKind.Op, Aliases.GetValueOrDefault(sym, sym), start, space));
        }
        toks.Add(new Token(TokenKind.End, "", src.Length, false));
        return toks;
    }

    // ---------------------------------------------------------------- parser

    private Token Peek => _toks[_i];
    private Token Next() => _toks[_i++];
    private bool Is(string op) => Peek.Kind == TokenKind.Op && Peek.Text == op;
    private bool IsWord(string w) => Peek.Kind == TokenKind.Id && Peek.Text == w;

    private bool Eat(string op)
    {
        if (!Is(op)) return false;
        _i++;
        return true;
    }

    private void Expect(string op)
    {
        if (!Eat(op)) throw new ParseException($"Expected '{op}'", Peek.Pos);
    }

    private void End()
    {
        if (Peek.Kind != TokenKind.End) throw new ParseException($"Unexpected '{Peek.Text}'", Peek.Pos);
    }

    private Statement Statement()
    {
        if (Peek.Kind == TokenKind.End) return EmptyStatement.Instance;
        int lhsPos = Peek.Pos;
        var lhs = Expr();
        if (Is(":=") || Is(":") || Is("≡"))
        {
            bool global = Next().Text == "≡";
            var target = ToTarget(lhs, lhsPos);
            var value = RangeOrExpr();
            bool show = false;
            MathExpr? unit = null;
            if (Eat("="))
            {
                show = true;
                if (Peek.Kind != TokenKind.End) unit = Expr();
            }
            End();
            return new DefineStatement(target, value, global, show, unit);
        }
        if (Eat("="))
        {
            var unit = Peek.Kind == TokenKind.End ? null : Expr();
            End();
            return new EvalStatement(lhs, unit);
        }
        End();
        return new ExprStatement(lhs);
    }

    private static MathExpr ToTarget(MathExpr lhs, int pos) => lhs switch
    {
        IdNode => lhs,
        CallNode c when c.Args.All(a => a is IdNode) => lhs,
        IndexNode { Target: IdNode } => lhs,
        _ => throw new ParseException("The left side of a definition must be a name, f(x, …) or v[i]", pos),
    };

    private MathExpr RangeOrExpr()
    {
        var first = Expr();
        if (Is(",") && _toks.Skip(_i).Any(t => t.Kind == TokenKind.Op && t.Text == ".."))
        {
            Next();
            var second = Expr();
            Expect("..");
            return new RangeNode(first, second, Expr());
        }
        if (Eat("..")) return new RangeNode(first, null, Expr());
        return first;
    }

    private MathExpr Expr() => Or();

    private MathExpr Or()
    {
        var l = And();
        while (Is("or") || IsWord("or")) { Next(); l = new BinaryNode("or", l, And()); }
        return l;
    }

    private MathExpr And()
    {
        var l = Not();
        while (Is("and") || IsWord("and")) { Next(); l = new BinaryNode("and", l, Not()); }
        return l;
    }

    private MathExpr Not()
    {
        if (Is("not") || IsWord("not")) { Next(); return new UnaryNode("not", Not()); }
        return Comparison();
    }

    private MathExpr Comparison()
    {
        var l = Additive();
        if (Peek.Kind == TokenKind.Op && ComparisonOps.TryGetValue(Peek.Text, out var op))
        {
            Next();
            return new BinaryNode(op, l, Additive());
        }
        return l;
    }

    private MathExpr Additive()
    {
        var l = Multiplicative();
        while (Is("+") || Is("-"))
        {
            var op = Next().Text == "+" ? "add" : "sub";
            l = new BinaryNode(op, l, Multiplicative());
        }
        return l;
    }

    private MathExpr Multiplicative()
    {
        var l = Unary();
        for (;;)
        {
            if (Is("*") || Is("/"))
            {
                var op = Next().Text == "*" ? "mult" : "div";
                l = new BinaryNode(op, l, Unary());
            }
            else if (StartsImplicitFactor()) l = new BinaryNode("mult", l, Power(), Implicit: true);
            else return l;
        }
    }

    private bool StartsImplicitFactor()
    {
        var t = Peek;
        return t.Kind switch
        {
            TokenKind.Number => true,
            TokenKind.Id => t.Text is not ("and" or "or" or "not"),
            TokenKind.Op => t.Text is "(" or "√" or "Σ" or "∏" or "∫",
            _ => false,
        };
    }

    private MathExpr Unary()
    {
        if (Eat("-")) return new UnaryNode("neg", Unary());
        if (Eat("+")) return Unary();
        return Power();
    }

    private MathExpr Power()
    {
        var b = Postfix();
        if (!Eat("^")) return b;
        var exp = Eat("-") ? new UnaryNode("neg", Power()) : Power();
        return new BinaryNode("pow", b, exp);
    }

    private MathExpr Postfix()
    {
        var x = Primary();
        for (;;)
        {
            if (Is("[") && !Peek.SpaceBefore)
            {
                Next();
                x = new IndexNode(x, List("]"));
            }
            else if (Eat("!")) x = new UnaryNode("factorial", x);
            else if (Eat("ᵀ")) x = new CallNode("transpose", [x]);
            else return x;
        }
    }

    private List<MathExpr> List(string close)
    {
        var items = new List<MathExpr>();
        if (Eat(close)) return items;
        do items.Add(Expr()); while (Eat(","));
        Expect(close);
        return items;
    }

    private MathExpr Primary()
    {
        var t = Next();
        switch (t.Kind)
        {
            case TokenKind.Number:
                return new NumberNode(t.Text);
            case TokenKind.String:
                return new StringNode(t.Text);
            case TokenKind.Id:
                if (Is("(") && !Peek.SpaceBefore)
                {
                    Next();
                    return new CallNode(t.Text, List(")"));
                }
                return new IdNode(t.Text);
            case TokenKind.Op:
                switch (t.Text)
                {
                    case "(":
                    {
                        var x = Expr();
                        Expect(")");
                        return new ParensNode(x);
                    }
                    case "[":
                        return Matrix(t.Pos);
                    case "|":
                    {
                        var x = Expr();
                        Expect("|");
                        return new CallNode("abs", [x]);
                    }
                    default:
                        if (SymbolFunctions.TryGetValue(t.Text, out var fn))
                        {
                            if (Eat("(")) return new CallNode(fn, List(")"));
                            return new CallNode(fn, [Power()]);
                        }
                        break;
                }
                break;
        }
        throw new ParseException(t.Kind == TokenKind.End ? "Incomplete expression" : $"Unexpected '{t.Text}'", t.Pos);
    }

    private MatrixNode Matrix(int pos)
    {
        var rows = new List<List<MathExpr>> { new() };
        if (Eat("]")) throw new ParseException("Empty matrix", pos);
        for (;;)
        {
            rows[^1].Add(Expr());
            if (Eat(",")) continue;
            if (Eat(";")) { rows.Add([]); continue; }
            Expect("]");
            break;
        }
        int cols = rows[0].Count;
        if (rows.Any(r => r.Count != cols)) throw new ParseException("Matrix rows have different lengths", pos);
        return new MatrixNode(rows.Count, cols, rows.SelectMany(r => r).ToArray());
    }
}
