namespace Quire.EquationEditor.Tree;

/// <summary>Expression tree edited by the equation editor and rendered in 2D.</summary>
public abstract record MathExpr;

/// <summary>An identifier. A trailing <c>_part</c> is a literal subscript (x_max renders as x with subscript max).</summary>
public sealed record IdNode(string Name) : MathExpr
{
    public string BaseName => Name.Contains('_') && Name.IndexOf('_') > 0 ? Name[..Name.IndexOf('_')] : Name;
    public string? Subscript => Name.IndexOf('_') is var i && i > 0 && i < Name.Length - 1 ? Name[(i + 1)..] : null;
}

/// <summary>A numeric literal; <see cref="Text"/> keeps what the user typed.</summary>
public sealed record NumberNode(string Text) : MathExpr
{
    public double Value => double.Parse(Text, System.Globalization.CultureInfo.InvariantCulture);
}

public sealed record StringNode(string Value) : MathExpr;

/// <summary>Prefix or postfix operator: <c>neg</c>, <c>not</c>, <c>factorial</c>.</summary>
public sealed record UnaryNode(string Op, MathExpr Operand) : MathExpr;

/// <summary>Binary operator: add, sub, mult, div, pow, eq, neq, lt, gt, leq, geq, and, or.</summary>
public sealed record BinaryNode(string Op, MathExpr Left, MathExpr Right, bool Implicit = false) : MathExpr;

public sealed record ParensNode(MathExpr Inner) : MathExpr;

/// <summary>Function application. Some names render specially: sqrt, nthroot, abs, transpose, sum, prod, integral, deriv.</summary>
public sealed record CallNode(string Name, IReadOnlyList<MathExpr> Args) : MathExpr;

public sealed record IndexNode(MathExpr Target, IReadOnlyList<MathExpr> Indices) : MathExpr;

/// <summary>Row-major matrix literal.</summary>
public sealed record MatrixNode(int Rows, int Cols, IReadOnlyList<MathExpr> Elements) : MathExpr
{
    public MathExpr this[int r, int c] => Elements[r * Cols + c];
}

public sealed record RangeNode(MathExpr Start, MathExpr? Second, MathExpr End) : MathExpr;

/// <summary>An empty slot the user still has to fill.</summary>
public sealed record PlaceholderNode : MathExpr
{
    public static readonly PlaceholderNode Instance = new();
}

/// <summary>One math region's statement.</summary>
public abstract record Statement;

public sealed record EmptyStatement : Statement
{
    public static readonly EmptyStatement Instance = new();
}

/// <summary><c>target := value</c> (or <c>≡</c> when global). <see cref="ShowResult"/> appends <c>= result</c>.</summary>
public sealed record DefineStatement(MathExpr Target, MathExpr Value, bool Global = false, bool ShowResult = false, MathExpr? Unit = null) : Statement;

/// <summary><c>expr = result</c>, optionally with a unit override placeholder.</summary>
public sealed record EvalStatement(MathExpr Expr, MathExpr? Unit = null) : Statement;

/// <summary>A bare expression, e.g. a solve-block constraint.</summary>
public sealed record ExprStatement(MathExpr Expr) : Statement;

/// <summary>Keyboard text that doesn't parse yet; kept verbatim so nothing the user typed is lost.</summary>
public sealed record UnparsedStatement(string Source) : Statement;
