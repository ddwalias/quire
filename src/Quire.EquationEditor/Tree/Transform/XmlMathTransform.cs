using System.Globalization;
using System.Xml.Linq;

namespace Quire.EquationEditor.Tree.Transform;

public sealed class UnsupportedMathXmlException(string element) : Exception($"Unsupported math element <{element}>")
{
    public string Element { get; } = element;
}

/// <summary>
/// Converts between the tree and the math XML vocabulary used in worksheet files
/// (define, globalDefine, eval, apply, id, real, str, matrix, range, parens, unitOverride, …).
/// Elements are matched by local name, so worksheets that use the same vocabulary under another
/// namespace (such as the math50 namespace of .mcdx files) can be read too.
/// </summary>
public static class XmlMathTransform
{
    public static readonly XNamespace Ns = "urn:quire:math";

    private static readonly Dictionary<string, string> BinaryToXml = new()
    {
        ["add"] = "plus", ["sub"] = "minus", ["mult"] = "mult", ["div"] = "div", ["pow"] = "pow",
        ["eq"] = "equal", ["neq"] = "notEqual", ["lt"] = "lessThan", ["gt"] = "greaterThan",
        ["leq"] = "lessOrEqual", ["geq"] = "greaterOrEqual", ["and"] = "and", ["or"] = "or",
    };

    private static readonly Dictionary<string, string> XmlToBinary = BinaryToXml.ToDictionary(kv => kv.Value, kv => kv.Key);

    private static readonly Dictionary<string, string> CallToXml = new()
    {
        ["sqrt"] = "sqrt", ["nthroot"] = "nthRoot", ["abs"] = "absval", ["transpose"] = "transpose",
        ["sum"] = "summation", ["prod"] = "product", ["integral"] = "integral", ["deriv"] = "derivative",
        ["det"] = "determinant",
    };

    private static readonly Dictionary<string, string> XmlToCall = CallToXml.ToDictionary(kv => kv.Value, kv => kv.Key);

    // ---------------------------------------------------------------- tree -> XML

    public static XElement ToXml(Statement statement) => statement switch
    {
        EmptyStatement => new XElement(Ns + "placeholder"),
        UnparsedStatement u => new XElement(Ns + "unparsed", new XAttribute(XNamespace.Xml + "space", "preserve"), u.Source),
        DefineStatement { ShowResult: true } d => EvalElement(DefineElement(d with { ShowResult = false, Unit = null }), d.Unit),
        DefineStatement d => DefineElement(d),
        EvalStatement e => EvalElement(ToXml(e.Expr), e.Unit),
        ExprStatement x => ToXml(x.Expr),
        _ => throw new ArgumentException(statement.GetType().Name),
    };

    private static XElement DefineElement(DefineStatement d)
    {
        XElement target = d.Target switch
        {
            CallNode c => new XElement(Ns + "function", Id(c.Name),
                new XElement(Ns + "boundVars", c.Args.Select(a => Id(((IdNode)a).Name)))),
            var t => ToXml(t),
        };
        return new XElement(Ns + (d.Global ? "globalDefine" : "define"), target, ToXml(d.Value));
    }

    private static XElement EvalElement(XElement inner, MathExpr? unit) =>
        new(Ns + "eval", inner, unit == null ? null : new XElement(Ns + "unitOverride", ToXml(unit)));

    private static XElement Id(string name) => new(Ns + "id", new XAttribute(XNamespace.Xml + "space", "preserve"), name);

    public static XElement ToXml(MathExpr node) => node switch
    {
        IdNode id => Id(id.Name),
        NumberNode n => new XElement(Ns + "real", n.Text),
        StringNode s => new XElement(Ns + "str", new XAttribute(XNamespace.Xml + "space", "preserve"), s.Value),
        UnaryNode u => Apply(u.Op, u.Operand),
        BinaryNode b => new XElement(Ns + "apply", new XElement(Ns + BinaryToXml[b.Op], b.Implicit ? new XAttribute("style", "implicit") : null),
            ToXml(b.Left), ToXml(b.Right)),
        ParensNode p => new XElement(Ns + "parens", ToXml(p.Inner)),
        CallNode c when CallToXml.TryGetValue(c.Name, out var op) => new XElement(Ns + "apply", new XElement(Ns + op), c.Args.Select(ToXml)),
        CallNode c => new XElement(Ns + "apply", Id(c.Name), new XElement(Ns + "sequence", c.Args.Select(ToXml))),
        IndexNode ix => new XElement(Ns + "apply", new XElement(Ns + "indexer"), ToXml(ix.Target), ix.Indices.Select(ToXml)),
        MatrixNode m => new XElement(Ns + "matrix", new XAttribute("rows", m.Rows), new XAttribute("cols", m.Cols),
            // column-major element order
            Enumerable.Range(0, m.Cols).SelectMany(c => Enumerable.Range(0, m.Rows).Select(r => ToXml(m[r, c])))),
        RangeNode r => new XElement(Ns + "range",
            r.Second == null ? ToXml(r.Start) : new XElement(Ns + "sequence", ToXml(r.Start), ToXml(r.Second)), ToXml(r.End)),
        PlaceholderNode => new XElement(Ns + "placeholder"),
        _ => throw new ArgumentException(node.GetType().Name),
    };

    private static XElement Apply(string op, MathExpr operand) => new(Ns + "apply", new XElement(Ns + op), ToXml(operand));

    // ---------------------------------------------------------------- XML -> tree

    public static Statement StatementFromXml(XElement e)
    {
        switch (e.Name.LocalName)
        {
            case "define" or "globalDefine":
            {
                var kids = e.Elements().ToArray();
                if (kids.Length < 2) throw new UnsupportedMathXmlException(e.Name.LocalName);
                return new DefineStatement(TargetFromXml(kids[0]), NodeFromXml(kids[1]), e.Name.LocalName == "globalDefine");
            }
            case "eval":
            {
                var kids = e.Elements().Where(k => k.Name.LocalName is not ("unitOverride" or "result" or "symResult")).ToArray();
                var unitEl = e.Elements().FirstOrDefault(k => k.Name.LocalName == "unitOverride")?.Elements().FirstOrDefault();
                var unit = unitEl == null || unitEl.Name.LocalName == "placeholder" ? null : NodeFromXml(unitEl);
                if (kids.Length == 0) throw new UnsupportedMathXmlException("eval");
                if (kids[0].Name.LocalName is "define" or "globalDefine")
                {
                    var d = (DefineStatement)StatementFromXml(kids[0]);
                    return d with { ShowResult = true, Unit = unit };
                }
                return new EvalStatement(NodeFromXml(kids[0]), unit);
            }
            case "placeholder":
                return EmptyStatement.Instance;
            case "unparsed":
                return new UnparsedStatement(e.Value);
            default:
                return new ExprStatement(NodeFromXml(e));
        }
    }

    private static MathExpr TargetFromXml(XElement e)
    {
        if (e.Name.LocalName != "function") return NodeFromXml(e);
        var kids = e.Elements().ToArray();
        var name = IdText(kids[0]);
        var vars = kids.FirstOrDefault(k => k.Name.LocalName == "boundVars")?.Elements().Select(v => (MathExpr)new IdNode(IdText(v))).ToArray() ?? [];
        return new CallNode(name, vars);
    }

    /// <summary>Identifier text; literal subscripts (attribute or nested Subscript span) become <c>base_sub</c>.</summary>
    public static string IdText(XElement id)
    {
        var sub = id.Attribute("subscript")?.Value;
        string text;
        if (id.HasElements)
        {
            var baseText = string.Concat(id.Elements().SelectMany(s => s.Nodes()).OfType<XText>().Select(t => t.Value));
            var subEl = id.Descendants().FirstOrDefault(d => d.Name.LocalName == "Subscript");
            text = baseText.Length > 0 ? baseText : id.Value;
            if (subEl != null) sub = subEl.Value;
        }
        else text = id.Value;
        text = text.Trim();
        return string.IsNullOrEmpty(sub) ? text : $"{text}_{sub}";
    }

    public static MathExpr NodeFromXml(XElement e)
    {
        var kids = e.Elements().ToArray();
        switch (e.Name.LocalName)
        {
            case "id":
                return new IdNode(IdText(e));
            case "real":
                return new NumberNode(e.Value.Trim());
            case "str":
                return new StringNode(e.Value);
            case "parens":
                return new ParensNode(NodeFromXml(kids[0]));
            case "placeholder":
                return PlaceholderNode.Instance;
            case "matrix":
            {
                int rows = int.Parse(e.Attribute("rows")!.Value, CultureInfo.InvariantCulture);
                int cols = int.Parse(e.Attribute("cols")!.Value, CultureInfo.InvariantCulture);
                var colMajor = kids.Select(NodeFromXml).ToArray();
                if (colMajor.Length != rows * cols) throw new UnsupportedMathXmlException("matrix");
                var rowMajor = new MathExpr[rows * cols];
                for (int c = 0; c < cols; c++)
                    for (int r = 0; r < rows; r++)
                        rowMajor[r * cols + c] = colMajor[c * rows + r];
                return new MatrixNode(rows, cols, rowMajor);
            }
            case "range":
            {
                if (kids.Length != 2) throw new UnsupportedMathXmlException("range");
                if (kids[0].Name.LocalName == "sequence")
                {
                    var s = kids[0].Elements().ToArray();
                    return new RangeNode(NodeFromXml(s[0]), s.Length > 1 ? NodeFromXml(s[1]) : null, NodeFromXml(kids[1]));
                }
                return new RangeNode(NodeFromXml(kids[0]), null, NodeFromXml(kids[1]));
            }
            case "apply":
                return ApplyFromXml(e, kids);
            default:
                throw new UnsupportedMathXmlException(e.Name.LocalName);
        }
    }

    private static MathExpr ApplyFromXml(XElement e, XElement[] kids)
    {
        if (kids.Length == 0) throw new UnsupportedMathXmlException("apply");
        var op = kids[0];
        var args = kids.Skip(1).ToArray();
        var name = op.Name.LocalName;

        if (name == "id")
        {
            var callArgs = args.Length == 1 && args[0].Name.LocalName == "sequence" ? args[0].Elements().ToArray() : args;
            return new CallNode(IdText(op), callArgs.Select(NodeFromXml).ToArray());
        }
        if (XmlToBinary.TryGetValue(name, out var bin))
        {
            if (args.Length != 2) throw new UnsupportedMathXmlException(name);
            bool implicitMult = op.Attribute("style")?.Value == "implicit";
            return new BinaryNode(bin, NodeFromXml(args[0]), NodeFromXml(args[1]), implicitMult);
        }
        switch (name)
        {
            case "neg" or "not" or "factorial":
                return new UnaryNode(name, NodeFromXml(args[0]));
            case "scale":
                // unit attached to a number in some files: treat as implicit multiplication
                return new BinaryNode("mult", NodeFromXml(args[0]), NodeFromXml(args[1]), Implicit: true);
            case "indexer":
                return new IndexNode(NodeFromXml(args[0]), args.Skip(1).Select(NodeFromXml).ToArray());
            case "summation" or "product" or "integral" or "derivative":
                return IteratedFromXml(XmlToCall[name], args);
        }
        if (XmlToCall.TryGetValue(name, out var fn)) return new CallNode(fn, args.Select(NodeFromXml).ToArray());
        throw new UnsupportedMathXmlException(name);
    }

    /// <summary>Reads either our flat form (body, var, lo, hi) or a lambda/bounds form.</summary>
    private static MathExpr IteratedFromXml(string fn, XElement[] args)
    {
        var lambda = args.FirstOrDefault(a => a.Name.LocalName == "lambda");
        if (lambda == null) return new CallNode(fn, args.Select(NodeFromXml).ToArray());
        var v = lambda.Elements().FirstOrDefault(x => x.Name.LocalName == "boundVars")?.Elements().FirstOrDefault()
                ?? throw new UnsupportedMathXmlException("lambda");
        var body = lambda.Elements().FirstOrDefault(x => x.Name.LocalName != "boundVars")
                   ?? throw new UnsupportedMathXmlException("lambda");
        var list = new List<MathExpr> { NodeFromXml(body), new IdNode(IdText(v)) };
        var bounds = args.FirstOrDefault(a => a.Name.LocalName == "bounds");
        if (bounds != null)
        {
            var lo = bounds.Elements().FirstOrDefault(b => b.Name.LocalName == "lowerBound")?.Elements().FirstOrDefault();
            var hi = bounds.Elements().FirstOrDefault(b => b.Name.LocalName == "upperBound")?.Elements().FirstOrDefault();
            if (lo != null && hi != null) { list.Add(NodeFromXml(lo)); list.Add(NodeFromXml(hi)); }
        }
        var degree = args.FirstOrDefault(a => a.Name.LocalName == "degree")?.Elements().FirstOrDefault();
        if (degree != null) list.Add(NodeFromXml(degree));
        return new CallNode(fn, list);
    }
}
