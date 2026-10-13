using System.Text;
using Quire.EquationEditor.Tree.Parsing;

namespace Quire.EquationEditor.Tree.Transform;

/// <summary>Writes a tree back as the linear keyboard syntax accepted by <see cref="LinearParser"/>.</summary>
public static class MathNodeToLinearTransform
{
    public static string Transform(Statement statement) => statement switch
    {
        EmptyStatement => "",
        UnparsedStatement u => u.Source,
        DefineStatement d => $"{Write(d.Target)} {(d.Global ? "≡" : ":=")} {Write(d.Value)}"
                             + (d.ShowResult ? " =" + (d.Unit != null ? " " + Write(d.Unit) : "") : ""),
        EvalStatement e => Write(e.Expr) + " =" + (e.Unit != null ? " " + Write(e.Unit) : ""),
        ExprStatement x => Write(x.Expr),
        _ => throw new ArgumentException(statement.GetType().Name),
    };

    private static readonly Dictionary<string, (string Sym, int Prec)> Binary = new()
    {
        ["or"] = (" or ", 1), ["and"] = (" and ", 2),
        ["eq"] = (" == ", 4), ["neq"] = (" != ", 4), ["lt"] = (" < ", 4), ["gt"] = (" > ", 4), ["leq"] = (" <= ", 4), ["geq"] = (" >= ", 4),
        ["add"] = (" + ", 5), ["sub"] = (" - ", 5), ["mult"] = ("*", 6), ["div"] = ("/", 6), ["pow"] = ("^", 8),
    };

    public static string Write(MathNode node) => Write(node, 0);

    private static string Write(MathNode node, int parentPrec)
    {
        switch (node)
        {
            case IdNode id:
                return NeedsQuoting(id.Name) ? $"`{id.Name}`" : id.Name;
            case NumberNode n:
                return n.Text;
            case StringNode s:
                return $"\"{s.Value}\"";
            case UnaryNode { Op: "neg" } u:
                return Wrap($"-{Write(u.Operand, 7)}", 7, parentPrec);
            case UnaryNode { Op: "not" } u:
                return Wrap($"not {Write(u.Operand, 3)}", 3, parentPrec);
            case UnaryNode { Op: "factorial" } u:
                return $"{Write(u.Operand, 9)}!";
            case BinaryNode b:
            {
                var (sym, prec) = Binary[b.Op];
                // left-assoc: right operand needs higher precedence; pow is right-assoc; comparisons don't chain
                int lp = b.Op == "pow" || prec == 4 ? prec + 1 : prec, rp = b.Op == "pow" ? prec : prec + 1;
                if (b.Op == "mult" && b.Implicit)
                {
                    // juxtaposition only re-parses as multiplication for simple right operands
                    bool simple = b.Right is IdNode or NumberNode or ParensNode or CallNode { Name: not ("abs" or "transpose") }
                                  or BinaryNode { Op: "pow", Left: IdNode or NumberNode or ParensNode or CallNode { Name: not ("abs" or "transpose") } };
                    var right = Write(b.Right, rp);
                    return Wrap(Write(b.Left, lp) + " " + (simple ? right : $"({Write(b.Right, 0)})"), prec, parentPrec);
                }
                if (b.Op == "pow" && b.Right is UnaryNode { Op: "neg" } neg)
                    return Wrap($"{Write(b.Left, lp)}^-{Write(neg.Operand, 9)}", prec, parentPrec);
                return Wrap(Write(b.Left, lp) + sym + Write(b.Right, rp), prec, parentPrec);
            }
            case ParensNode p:
                return $"({Write(p.Inner, 0)})";
            case CallNode { Name: "abs", Args.Count: 1 } c:
                return $"|{Write(c.Args[0], 0)}|";
            case CallNode { Name: "transpose", Args.Count: 1 } c:
                return $"{Write(c.Args[0], 9)}ᵀ";
            case CallNode c:
                return $"{(NeedsQuoting(c.Name) ? $"`{c.Name}`" : c.Name)}({string.Join(", ", c.Args.Select(a => Write(a, 0)))})";
            case IndexNode ix:
                return $"{Write(ix.Target, 9)}[{string.Join(", ", ix.Indices.Select(i => Write(i, 0)))}]";
            case MatrixNode m:
            {
                var rows = Enumerable.Range(0, m.Rows).Select(r =>
                    string.Join(", ", Enumerable.Range(0, m.Cols).Select(c => Write(m[r, c], 0))));
                return $"[{string.Join("; ", rows)}]";
            }
            case RangeNode r:
                return r.Second == null
                    ? $"{Write(r.Start, 0)} .. {Write(r.End, 0)}"
                    : $"{Write(r.Start, 0)}, {Write(r.Second, 0)} .. {Write(r.End, 0)}";
            case PlaceholderNode:
                return "■";
            default:
                throw new ArgumentException(node.GetType().Name);
        }
    }

    private static string Wrap(string s, int prec, int parentPrec) => prec < parentPrec ? $"({s})" : s;

    private static bool NeedsQuoting(string name) =>
        name.Length == 0 || !LinearParser.IsIdStart(name[0]) || name.Any(c => !LinearParser.IsIdPart(c))
        || name is "and" or "or" or "not";
}
