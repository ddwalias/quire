using System.Globalization;
using System.Text;

namespace Quire.EquationEditor.Tree.Transform;

/// <summary>Serialises statements and expressions into the engine's Rl S-expression form.</summary>
public static class MathNodeToRlTransform
{
    private static readonly Dictionary<string, string> BinaryOps = new()
    {
        ["add"] = "op_add", ["sub"] = "op_sub", ["mult"] = "op_mult", ["div"] = "op_div", ["pow"] = "op_power",
        ["eq"] = "op_eq", ["neq"] = "op_neq", ["lt"] = "op_lt", ["gt"] = "op_gt", ["leq"] = "op_leq", ["geq"] = "op_geq",
        ["and"] = "op_and", ["or"] = "op_or",
    };

    private static readonly Dictionary<string, string> UnaryOps = new()
    {
        ["neg"] = "op_negate", ["not"] = "op_not", ["factorial"] = "op_factorial",
    };

    public static string Transform(Statement statement) => statement switch
    {
        EmptyStatement or UnparsedStatement => "(noop)",
        DefineStatement d => Define(d),
        EvalStatement e => e.Unit == null ? $"(eval {Expr(e.Expr)})" : $"(eval {Expr(e.Expr)} {Expr(e.Unit)})",
        ExprStatement x => $"(expr {Expr(x.Expr)})",
        _ => throw new ArgumentException($"Unknown statement {statement.GetType().Name}"),
    };

    private static string Define(DefineStatement d)
    {
        string head = d.Global ? "global" : d.ShowResult ? "define-eval" : "define";
        string target = d.Target switch
        {
            IdNode id => Symbol(id.Name),
            CallNode c => $"({Symbol(c.Name)}{string.Concat(c.Args.Select(a => " " + Symbol(((IdNode)a).Name)))})",
            IndexNode { Target: IdNode v } ix => $"(op_index {Symbol(v.Name)}{string.Concat(ix.Indices.Select(i => " " + Expr(i)))})",
            _ => throw new ArgumentException("Invalid definition target"),
        };
        var unit = d.ShowResult && !d.Global && d.Unit != null ? " " + Expr(d.Unit) : "";
        return $"({head} {target} {Expr(d.Value)}{unit})";
    }

    public static string Expr(MathNode node)
    {
        var sb = new StringBuilder();
        Write(node, sb);
        return sb.ToString();
    }

    private static void Write(MathNode node, StringBuilder sb)
    {
        switch (node)
        {
            case IdNode id:
                sb.Append(Symbol(id.Name));
                break;
            case NumberNode n:
                sb.Append(n.Value.ToString("R", CultureInfo.InvariantCulture));
                break;
            case StringNode s:
                sb.Append('"').Append(s.Value.Replace("\\", "\\\\").Replace("\"", "\\\"")).Append('"');
                break;
            case UnaryNode u:
                List(sb, UnaryOps[u.Op], u.Operand);
                break;
            case BinaryNode b:
                List(sb, BinaryOps[b.Op], b.Left, b.Right);
                break;
            case ParensNode p:
                Write(p.Inner, sb);
                break;
            case CallNode c:
                Call(c, sb);
                break;
            case IndexNode ix:
                List(sb, "op_index", [ix.Target, .. ix.Indices]);
                break;
            case MatrixNode m:
                sb.Append($"(matrix {m.Rows} {m.Cols}");
                foreach (var e in m.Elements) { sb.Append(' '); Write(e, sb); }
                sb.Append(')');
                break;
            case RangeNode r when r.Second == null:
                List(sb, "op_range2", r.Start, r.End);
                break;
            case RangeNode r:
                List(sb, "op_range3", r.Start, r.Second!, r.End);
                break;
            case PlaceholderNode:
                sb.Append("|■|");
                break;
            default:
                throw new ArgumentException($"Unknown node {node.GetType().Name}");
        }
    }

    private static void Call(CallNode c, StringBuilder sb)
    {
        var a = c.Args;
        switch (c.Name, a.Count)
        {
            case ("sqrt", 1): List(sb, "op_sqrt", a[0]); return;
            case ("nthroot", 2): List(sb, "op_rootn", a[1], a[0]); return;
            case ("abs", 1): List(sb, "op_abs", a[0]); return;
            case ("transpose", 1): List(sb, "op_transpose", a[0]); return;
            case ("sum", 4): List(sb, "op_sigmasum", [.. a]); return;
            case ("prod", 4): List(sb, "op_sigmaproduct", [.. a]); return;
            case ("integral", 4): List(sb, "op_integral", [.. a]); return;
            case ("deriv", 2): List(sb, "op_deriv_1", a[0], a[1]); return;
            case ("deriv", 3): List(sb, "op_deriv_n", a[0], a[1], a[2]); return;
        }
        sb.Append("(op_apply ").Append(Symbol(c.Name));
        foreach (var arg in a) { sb.Append(' '); Write(arg, sb); }
        sb.Append(')');
    }

    private static void List(StringBuilder sb, string head, params MathNode[] args)
    {
        sb.Append('(').Append(head);
        foreach (var x in args) { sb.Append(' '); Write(x, sb); }
        sb.Append(')');
    }

    public static string Symbol(string name)
    {
        bool quote = name.Length == 0 || name.Any(ch => char.IsWhiteSpace(ch) || ch is '(' or ')' or '"' or '|')
                     || double.TryParse(name, NumberStyles.Float, CultureInfo.InvariantCulture, out _);
        return quote ? "|" + name.Replace("|", "\\|") + "|" : name;
    }
}
