using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Quire.Mpl.Internal;

namespace Quire;

/// <summary>Insert palettes: clicking an entry types its text into the region being edited.</summary>
internal static class Palettes
{
    private static void Section(Panel host, string title, IEnumerable<(string Label, string Insert, string Tip)> items, Action<string> insert)
    {
        host.Children.Add(new TextBlock { Text = title, FontWeight = FontWeights.SemiBold, Foreground = Brushes.SlateGray, Margin = new Thickness(2, 6, 0, 2), FontSize = 11 });
        var wrap = new WrapPanel();
        foreach (var (label, text, tip) in items)
        {
            var b = new Button { Content = label, ToolTip = tip, Style = (Style)Application.Current.Resources["PaletteButton"] };
            b.Click += (_, _) => insert(text);
            wrap.Children.Add(b);
        }
        host.Children.Add(wrap);
    }

    public static void BuildOperators(Panel host, Action<string> insert)
    {
        Section(host, "Definition and evaluation", [
            (":=", ":=", "Definition — or type :"), ("≡", "≡", "Global definition — or type ~"),
            ("=", "=", "Evaluate"), ("= unit", " = ", "Evaluate, then type a unit to convert to"),
        ], insert);
        Section(host, "Arithmetic", [
            ("+", "+", "Add"), ("−", "-", "Subtract"), ("·", "*", "Multiply"), ("÷", "/", "Divide (shown as a fraction)"),
            ("xⁿ", "^", "Power"), ("√", "sqrt(", "Square root"), ("ⁿ√", "nthroot(", "nthroot(x, n)"), ("|x|", "|", "Absolute value / determinant / magnitude"),
            ("n!", "!", "Factorial"), ("( )", "(", "Parentheses"),
        ], insert);
        Section(host, "Comparison and logic", [
            ("= (bold)", "==", "Equal to — Ctrl+= — used for solve-block constraints"), ("≠", "≠", "Not equal"),
            ("<", "<", "Less than"), (">", ">", "Greater than"), ("≤", "≤", "Less or equal"), ("≥", "≥", "Greater or equal"),
            ("∧", " and ", "And"), ("∨", " or ", "Or"), ("¬", "not ", "Not"),
        ], insert);
        Section(host, "Matrices and ranges", [
            ("[ ]", "[1, 2; 3, 4]", "Matrix: commas between columns, ; between rows"), ("v[i]", "[", "Index (ORIGIN = 0)"),
            ("Mᵀ", "ᵀ", "Transpose"), ("M⁻¹", "^-1", "Inverse"), ("a..b", " .. ", "Range variable"), ("a,b..c", ", .. ", "Stepped range: first, second .. last"),
        ], insert);
        Section(host, "Calculus", [
            ("Σ", "sum(", "sum(expr, i, a, b) or sum(v)"), ("∏", "prod(", "prod(expr, i, a, b)"),
            ("∫", "integral(", "integral(f, x, a, b)"), ("d/dx", "deriv(", "deriv(f, x) at the current x; deriv(f, x, n)"),
        ], insert);
    }

    public static void BuildGreek(Panel host, Action<string> insert)
    {
        const string lower = "αβγδεζηθικλμνξπρστυφχψω", upper = "ΓΔΘΛΞΠΣΥΦΨΩ";
        Section(host, "Lower case", lower.Select(c => (c.ToString(), c.ToString(), $"{c}")), insert);
        Section(host, "Upper case", upper.Select(c => (c.ToString(), c.ToString(), $"{c}")), insert);
        Section(host, "Constants", [("π", "π", "pi"), ("e", "e", "Euler's number"), ("g", "g", "standard gravity"), ("∞", "∞", "infinity")], insert);
    }

    public static void BuildUnits(Panel host, Action<string> insert)
    {
        foreach (var group in BuiltinUnits.All.Values.GroupBy(u => u.Quantity).OrderBy(g => g.Key))
            Section(host, group.Key, group.Where(u => !u.Name.StartsWith('Δ')).Select(u =>
                (u.Name, " " + (u.Name.Contains('·') ? $"`{u.Name}`" : u.Name), $"{u.Name} = {u.Magnitude:G6} SI")), insert);
        Section(host, "Temperature scales", [("°C", " °C", "Celsius (absolute temperatures)"), ("°F", " °F", "Fahrenheit (absolute temperatures)")], insert);
    }

    public static void BuildFunctions(Panel host, Action<string> insert)
    {
        (string, string, string) F(string name, string tip) => (name, name + "(", tip);
        Section(host, "Elementary", [
            F("sin", "sine"), F("cos", "cosine"), F("tan", "tangent"), F("asin", "arcsine"), F("acos", "arccosine"),
            F("atan", "arctangent"), F("atan2", "atan2(x, y)"), F("sinh", ""), F("cosh", ""), F("tanh", ""),
            F("exp", ""), F("ln", "natural log"), F("log", "log(x) base 10, log(x, b)"), F("sqrt", ""), F("nthroot", "nthroot(x, n)"),
            F("abs", ""), F("floor", ""), F("ceil", ""), F("round", "round(x, n)"), F("mod", "mod(x, y)"), F("erf", ""), F("Γ", "gamma"),
        ], insert);
        Section(host, "Vectors and matrices", [
            F("length", ""), F("rows", ""), F("cols", ""), F("last", ""), F("identity", "identity(n)"), F("transpose", ""),
            F("inverse", ""), F("det", ""), F("tr", "trace"), F("rank", ""), F("lsolve", "lsolve(M, v)"), F("augment", ""), F("stack", ""),
            F("submatrix", "submatrix(A, ir, jr, ic, jc)"), F("col", "col(M, j)"), F("row", "row(M, i)"), F("matrix", "matrix(m, n, f)"),
            F("linspace", "linspace(a, b, n)"), F("sort", ""), F("reverse", ""), F("cross", ""), F("dot", ""), F("norme", "Euclidean norm"),
        ], insert);
        Section(host, "Statistics and interpolation", [
            F("mean", ""), F("median", ""), F("stdev", "population"), F("Stdev", "sample"), F("var", "population"), F("Var", "sample"),
            F("max", ""), F("min", ""), F("sum", ""), F("linterp", "linterp(vx, vy, x)"), F("spline", "spline(vx, vy, x)"),
        ], insert);
        Section(host, "Solving and calculus", [
            F("root", "root(f(x), x) or root(f(x), x, a, b)"), F("find", "find(x, y) — in a solve block"),
            F("minerr", "minerr(x, y) — in a solve block"), F("minimize", "minimize(f, x, y)"), F("maximize", "maximize(f, x, y)"),
            F("integral", "integral(f, x, a, b)"), F("deriv", "deriv(f, x)"), F("if", "if(condition, a, b)"),
        ], insert);
        Section(host, "Strings and units", [
            F("concat", ""), F("strlen", ""), F("num2str", ""), F("str2num", ""), F("UnitsOf", "UnitsOf(x)"), F("error", "error(\"message\")"),
        ], insert);
    }
}
