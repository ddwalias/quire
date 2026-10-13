using Quire.EquationEditor.Tree.Parsing;
using Quire.PersistentDataObjects;

namespace Quire;

/// <summary>The "Getting started" worksheet shown on first launch.</summary>
internal static class SampleWorksheet
{
    public static WorksheetDocument Create()
    {
        var doc = new WorksheetDocument();
        double y = 0;

        void Text(string text, double left = 0, double width = 680, double gap = 34)
        {
            doc.Regions.Add(new TextRegionPersistentData { Top = y, Left = left, Width = width, SectionXaml = TextRegionPersistentData.PlainSection(text) });
            y += gap;
        }
        void Math(string src, double left = 0) =>
            doc.Regions.Add(new MathRegionPersistentData { Top = y, Left = left, Statement = LinearParser.ParseStatement(src) });
        void Row(double gap, params string[] cells)
        {
            double left = 0;
            foreach (var c in cells) { Math(c, left); left += 232; }
            y += gap;
        }

        doc.Regions.Add(new TextRegionPersistentData
        {
            Top = y, Width = 680,
            SectionXaml = "<Section xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"><Paragraph FontSize=\"22\" FontWeight=\"SemiBold\" Foreground=\"#1F3A5F\"><Run>Getting started with Quire</Run></Paragraph></Section>",
        });
        y += 48;
        Text("Click anywhere and start typing to create math. Use : for :=, = to evaluate, and type a unit after = to convert. " +
             "Regions calculate top to bottom, left to right — drag them around and watch the results update.", gap: 56);

        Text("1. Units are checked and converted automatically");
        Row(40, "mass := 1200 kg", "a := 2.5 m/s^2", "F_net := mass*a =");
        Row(40, "v := 100 km/hr", "v = mph", "E_k := 1/2 mass v^2 = kJ");
        Row(48, "T_air := 20 °C", "T_air =", "T_air = °F");

        Text("2. Functions, ranges and plots");
        Row(40, "g_1(x) := x^2 e^(-x/2)", "x := 0, 0.25 .. 20", "x_peak := root(deriv(g_1(x), x), x, 1, 8) =");
        doc.Regions.Add(Plot(y, 0, "x", ["g_1(x)", "2 sin(x)/x"], width: 300));
        Math("t := 0 s, 0.05 s .. 2 s", 432);
        y += 34;
        Math("h(t) := 10 m/s*t - 1/2 g t^2", 432);
        doc.Regions.Add(Plot(y + 40, 432, "t", ["h(t)"], width: 230, height: 170));
        y += 270;

        Text("3. Matrices and linear systems");
        Row(70, "M := [4, -2, 1; -2, 4, -2; 1, -2, 4]", "b := [11; -16; 17]", "x_s := lsolve(M, b) =");
        Row(56, "|M| =", "M^-1 =");
        y += 30;

        Text("4. Calculus");
        Row(64, "sum(k^2, k, 1, 100) =", "integral(e^(-x^2), x, -∞, ∞) =", "√(π) =");

        Text("5. Solve blocks — guess values, constraints, solver");
        var sb = new SolveBlockRegionPersistentData { Top = y, Left = 0, Solver = LinearParser.ParseStatement("sol := find(w, h)") };
        sb.Guesses.Add(LinearParser.ParseStatement("w := 1 m"));
        sb.Guesses.Add(LinearParser.ParseStatement("h := 2 m"));
        sb.Constraints.Add(LinearParser.ParseStatement("w*h == 12 m^2"));
        sb.Constraints.Add(LinearParser.ParseStatement("2 w + 2 h == 14 m"));
        doc.Regions.Add(sb);
        Math("sol =", 400);
        y += 260;

        Text("Press F1 for the quick reference. File ▸ Import opens Mathcad Prime worksheets (.mcdx).", gap: 30);
        return doc;
    }

    private static PlotRegionPersistentData Plot(double top, double left, string x, string[] ys, double width = 360, double height = 220)
    {
        var p = new PlotRegionPersistentData { Top = top, Left = left, Width = width, Height = height, X = LinearParser.ParseExpression(x) };
        foreach (var y in ys) p.Y.Add(LinearParser.ParseExpression(y));
        return p;
    }
}
