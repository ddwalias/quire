using System.IO;
using System.IO.Compression;
using System.Text;
using Quire.Core;
using Quire.EquationEditor.Tree;
using Quire.EquationEditor.Tree.Parsing;
using Quire.EquationEditor.Tree.Transform;
using Quire.PersistentDataObjects;

namespace Quire.Core.Tests;

public class WorksheetPackageTests
{
    private static Statement P(string s) => LinearParser.ParseStatement(s);
    private static string Rl(Statement s) => MathNodeToRlTransform.Transform(s);

    [Fact]
    public void Save_and_load_round_trips_every_region_kind()
    {
        var doc = new WorksheetDocument { Settings = new WorksheetSettings { UnitSystem = "uscs", Decimals = 5, AutoCalc = false } };
        doc.Regions.Add(new MathRegionPersistentData { Top = 10, Left = 20, Statement = P("x := 3 m") });
        doc.Regions.Add(new TextRegionPersistentData { Top = 40, Left = 0, Width = 300, SectionXaml = TextRegionPersistentData.PlainSection("Hello & <world>") });
        var plot = new PlotRegionPersistentData { Top = 80, Kind = PlotKind.Polar, X = LinearParser.ParseExpression("θ"), XMin = -1, YUnit = new IdNode("mm") };
        plot.Y.Add(LinearParser.ParseExpression("sin(θ)"));
        doc.Regions.Add(plot);
        var sb = new SolveBlockRegionPersistentData { Top = 300, Solver = P("s := find(x)") };
        sb.Guesses.Add(P("x := 1"));
        sb.Constraints.Add(P("x^2 == 4"));
        doc.Regions.Add(sb);

        using var ms = new MemoryStream();
        WorksheetPackage.Save(doc, ms);
        ms.Position = 0;
        var back = WorksheetPackage.Load(ms);

        Assert.Equal("uscs", back.Settings.UnitSystem);
        Assert.Equal(5, back.Settings.Decimals);
        Assert.False(back.Settings.AutoCalc);
        Assert.Equal(4, back.Regions.Count);

        var m = Assert.IsType<MathRegionPersistentData>(back.Regions[0]);
        Assert.Equal((10.0, 20.0), (m.Top, m.Left));
        Assert.Equal("(define x (op_mult 3 m))", Rl(m.Statement));

        var t = Assert.IsType<TextRegionPersistentData>(back.Regions[1]);
        Assert.Contains("Hello &amp; &lt;world&gt;", t.SectionXaml);
        Assert.Equal(300, t.Width);

        var p = Assert.IsType<PlotRegionPersistentData>(back.Regions[2]);
        Assert.Equal(PlotKind.Polar, p.Kind);
        Assert.Equal(-1, p.XMin);
        Assert.Null(p.XMax);
        Assert.Equal("(op_apply sin θ)", MathNodeToRlTransform.Expr(Assert.Single(p.Y)));
        Assert.Equal("mm", MathNodeToRlTransform.Expr(p.YUnit!));

        var s = Assert.IsType<SolveBlockRegionPersistentData>(back.Regions[3]);
        Assert.Equal("(define x 1)", Rl(Assert.Single(s.Guesses)));
        Assert.Equal("(expr (op_eq (op_power x 2) 4))", Rl(Assert.Single(s.Constraints)));
        Assert.Equal("(define s (op_apply find x))", Rl(s.Solver));
    }

    [Fact]
    public void Loading_a_non_worksheet_fails_clearly()
    {
        using var ms = new MemoryStream();
        using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, leaveOpen: true)) zip.CreateEntry("other.txt");
        ms.Position = 0;
        Assert.Throws<InvalidDataException>(() => WorksheetPackage.Load(ms));
    }
}

public class WorksheetConverterTests(Xunit.Abstractions.ITestOutputHelper output)
{
    private const string Math50 = "http://schemas.mathsoft.com/math50";

    /// <summary>Builds a minimal package with the layout Mathcad Prime uses.</summary>
    private static MemoryStream SyntheticMcdx(string regionsXml, int origin = 0)
    {
        var ms = new MemoryStream();
        using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, leaveOpen: true))
        {
            void Put(string name, string content)
            {
                using var w = new StreamWriter(zip.CreateEntry(name).Open(), new UTF8Encoding(false));
                w.Write(content);
            }
            Put("mathcad/worksheet.xml",
                $"<worksheet xmlns=\"http://schemas.mathsoft.com/worksheet50\" xmlns:ml=\"{Math50}\"><regions>{regionsXml}</regions></worksheet>");
            Put("mathcad/_rels/worksheet.xml.rels",
                "<Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\">" +
                "<Relationship Type=\"flowDocument\" Target=\"/mathcad/xaml/FlowDocument0.XamlPackage\" Id=\"T1\" /></Relationships>");
            Put("mathcad/settings/calculation.xml",
                $"<calculation xmlns=\"http://schemas.ptc.com/mathcad/settings/calculation10\"><builtInVariables array-origin=\"{origin}\" />" +
                "<calculationBehavior automatic-recalculation=\"true\" /><units><currentUnitSystem name=\"uscs\" /></units></calculation>");

            var inner = new MemoryStream();
            using (var pkg = new ZipArchive(inner, ZipArchiveMode.Create, leaveOpen: true))
            {
                using var w = new StreamWriter(pkg.CreateEntry("Xaml/Document.xaml").Open());
                w.Write("<Section xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"><Paragraph><Run>Beam design</Run></Paragraph></Section>");
            }
            using (var e = zip.CreateEntry("mathcad/xaml/FlowDocument0.XamlPackage").Open()) e.Write(inner.ToArray());
        }
        ms.Position = 0;
        return ms;
    }

    [Fact]
    public void Imports_math_text_settings_and_area_offsets()
    {
        const string regions = """
            <region top="0" left="0" width="400"><text item-idref="T1" /></region>
            <region top="40" left="20"><math><ml:define xmlns:ml="http://schemas.mathsoft.com/math50"><ml:id>L</ml:id><ml:apply><ml:mult /><ml:real>3</ml:real><ml:id>m</ml:id></ml:apply></ml:define></math></region>
            <region top="100" left="0"><Area><regions>
              <region top="10" left="30"><math><ml:eval xmlns:ml="http://schemas.mathsoft.com/math50"><ml:id>L</ml:id><ml:unitOverride><ml:id>ft</ml:id></ml:unitOverride></ml:eval></math></region>
            </regions></Area></region>
            """;
        var result = WorksheetConverter.ImportMcdx(SyntheticMcdx(regions));
        var doc = result.Document;

        Assert.Empty(result.Warnings);
        Assert.Equal("uscs", doc.Settings.UnitSystem);
        Assert.Equal(3, doc.Regions.Count);
        Assert.Contains("Beam design", Assert.IsType<TextRegionPersistentData>(doc.Regions[0]).SectionXaml);
        Assert.Equal(400, ((TextRegionPersistentData)doc.Regions[0]).Width);
        Assert.Equal("(define L (op_mult 3 m))", MathNodeToRlTransform.Transform(((MathRegionPersistentData)doc.Regions[1]).Statement));

        var inArea = Assert.IsType<MathRegionPersistentData>(doc.Regions[2]);
        Assert.Equal((110.0, 30.0), (inArea.Top, inArea.Left));
        Assert.Equal("(eval L ft)", MathNodeToRlTransform.Transform(inArea.Statement));
    }

    [Fact]
    public void Unsupported_content_becomes_a_note_with_a_warning()
    {
        const string regions = """
            <region top="0" left="0"><math><ml:program xmlns:ml="http://schemas.mathsoft.com/math50" /></math></region>
            <region top="50" left="0"><chartComponent /></region>
            """;
        var result = WorksheetConverter.ImportMcdx(SyntheticMcdx(regions));
        Assert.Equal(2, result.Warnings.Count);
        Assert.All(result.Document.Regions, r => Assert.IsType<TextRegionPersistentData>(r));
    }

    [Fact]
    public void Array_origin_becomes_a_global_definition()
    {
        var doc = WorksheetConverter.ImportMcdx(SyntheticMcdx("", origin: 1)).Document;
        var m = Assert.IsType<MathRegionPersistentData>(Assert.Single(doc.Regions));
        Assert.Equal("(global ORIGIN 1)", MathNodeToRlTransform.Transform(m.Statement));
        Assert.True(m.Top >= 0);
    }

    /// <summary>Uses the templates shipped with a local Mathcad Prime install when present (not redistributed).</summary>
    [Fact]
    public void Imports_installed_Mathcad_templates_when_available()
    {
        var dir = @"C:\Program Files\PTC\Mathcad Prime 12.0.1.0\Default Templates\en-US";
        if (!Directory.Exists(dir)) return;
        int math = 0;
        foreach (var file in Directory.GetFiles(dir, "*.mctx"))
        {
            var result = WorksheetConverter.ImportMcdx(file);
            foreach (var w in result.Warnings) output.WriteLine($"{Path.GetFileName(file)}: {w}");
            foreach (var m in result.Document.Regions.OfType<MathRegionPersistentData>())
            {
                output.WriteLine($"{Path.GetFileName(file)}: {MathNodeToLinearTransform.Transform(m.Statement)}");
                Assert.NotNull(MathNodeToRlTransform.Transform(m.Statement));
                math++;
            }
        }
        Assert.True(math > 0, "expected the templates to contain math regions");
    }
}

public class AtomicSaveTests
{
    [Fact]
    public void A_failed_save_leaves_the_existing_file_intact()
    {
        var path = Path.Combine(Path.GetTempPath(), $"quire-{Guid.NewGuid():N}.qwx");
        try
        {
            var good = new WorksheetDocument();
            good.Regions.Add(new MathRegionPersistentData { Statement = LinearParser.ParseStatement("x := 1") });
            WorksheetPackage.Save(good, path);

            var bad = new WorksheetDocument();
            bad.Regions.Add(new MathRegionPersistentData { Statement = new ExprStatement(new StringNode("bad \u0001 char")) });
            Assert.ThrowsAny<Exception>(() => WorksheetPackage.Save(bad, path));

            Assert.Single(WorksheetPackage.Load(path).Regions);
            Assert.Empty(Directory.GetFiles(Path.GetDirectoryName(path)!, "." + Path.GetFileName(path) + "*.tmp"));
        }
        finally
        {
            File.Delete(path);
        }
    }
}
