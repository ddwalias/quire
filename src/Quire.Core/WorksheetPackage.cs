using System.IO;
using System.Globalization;
using System.IO.Compression;
using System.Xml.Linq;
using Quire.EquationEditor.Tree;
using Quire.EquationEditor.Tree.Transform;
using Quire.PersistentDataObjects;

namespace Quire.Core;

/// <summary>
/// Reads and writes Quire worksheets (<c>.qwx</c>): a zip package containing <c>quire/worksheet.xml</c>,
/// in which math is stored with the <see cref="XmlMathTransform"/> vocabulary.
/// </summary>
public static class WorksheetPackage
{
    public const string Extension = ".qwx";
    public static readonly XNamespace Ws = "urn:quire:worksheet";
    private static readonly XNamespace Ml = XmlMathTransform.Ns;
    private const string WorksheetPart = "quire/worksheet.xml";
    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

    /// <summary>Saves atomically: the existing file is only replaced once the new one is complete.</summary>
    public static void Save(WorksheetDocument doc, string path)
    {
        var full = Path.GetFullPath(path);
        var temp = Path.Combine(Path.GetDirectoryName(full)!, "." + Path.GetFileName(full) + "." + Guid.NewGuid().ToString("N")[..8] + ".tmp");
        try
        {
            using (var fs = File.Create(temp)) Save(doc, fs);
            File.Move(temp, full, overwrite: true);
        }
        finally
        {
            if (File.Exists(temp)) File.Delete(temp);
        }
    }

    public static void Save(WorksheetDocument doc, Stream stream)
    {
        using var zip = new ZipArchive(stream, ZipArchiveMode.Create, leaveOpen: true);
        WriteEntry(zip, "[Content_Types].xml", new XDocument(
            new XElement(XNamespace.Get("http://schemas.openxmlformats.org/package/2006/content-types") + "Types",
                new XElement(XNamespace.Get("http://schemas.openxmlformats.org/package/2006/content-types") + "Default",
                    new XAttribute("Extension", "xml"), new XAttribute("ContentType", "application/xml")),
                new XElement(XNamespace.Get("http://schemas.openxmlformats.org/package/2006/content-types") + "Default",
                    new XAttribute("Extension", "rels"), new XAttribute("ContentType", "application/vnd.openxmlformats-package.relationships+xml")))));
        WriteEntry(zip, "_rels/.rels", new XDocument(
            new XElement(XNamespace.Get("http://schemas.openxmlformats.org/package/2006/relationships") + "Relationships",
                new XElement(XNamespace.Get("http://schemas.openxmlformats.org/package/2006/relationships") + "Relationship",
                    new XAttribute("Type", "urn:quire:worksheet"), new XAttribute("Target", "/" + WorksheetPart), new XAttribute("Id", "R1")))));
        WriteEntry(zip, WorksheetPart, new XDocument(ToXml(doc)));
    }

    private static void WriteEntry(ZipArchive zip, string name, XDocument xml)
    {
        using var s = zip.CreateEntry(name, CompressionLevel.Optimal).Open();
        xml.Save(s);
    }

    public static WorksheetDocument Load(string path)
    {
        using var fs = File.OpenRead(path);
        return Load(fs);
    }

    public static WorksheetDocument Load(Stream stream)
    {
        using var zip = new ZipArchive(stream, ZipArchiveMode.Read, leaveOpen: true);
        var entry = zip.GetEntry(WorksheetPart) ?? throw new InvalidDataException("Not a Quire worksheet (missing quire/worksheet.xml)");
        using var s = entry.Open();
        return FromXml(XDocument.Load(s).Root!);
    }

    // ---------------------------------------------------------------- XML

    public static XElement ToXml(WorksheetDocument doc)
    {
        var st = doc.Settings;
        return new XElement(Ws + "worksheet",
            new XAttribute("version", "1"),
            new XAttribute(XNamespace.Xmlns + "ml", Ml.NamespaceName),
            new XElement(Ws + "settings",
                new XAttribute("unitSystem", st.UnitSystem), new XAttribute("autoCalc", st.AutoCalc),
                new XAttribute("decimals", st.Decimals), new XAttribute("expThreshold", st.ExponentialThreshold),
                new XAttribute("trailingZeros", st.ShowTrailingZeros)),
            new XElement(Ws + "regions", doc.Regions.Select(RegionToXml)));
    }

    private static XElement RegionToXml(RegionPersistentData r)
    {
        var e = new XElement(Ws + "region", new XAttribute("id", r.Id),
            new XAttribute("top", r.Top.ToString("R", Inv)), new XAttribute("left", r.Left.ToString("R", Inv)));
        switch (r)
        {
            case MathRegionPersistentData m:
                e.Add(m.Source != null
                    ? new XElement(Ws + "math", new XAttribute("source", m.Source))
                    : new XElement(Ws + "math", XmlMathTransform.ToXml(m.Statement)));
                break;
            case TextRegionPersistentData t:
                e.Add(new XElement(Ws + "text", new XAttribute("width", t.Width.ToString("R", Inv)), ParseSection(t.SectionXaml)));
                break;
            case PlotRegionPersistentData p:
                e.Add(new XElement(Ws + "plot",
                    new XAttribute("kind", p.Kind.ToString().ToLowerInvariant()),
                    new XAttribute("width", p.Width.ToString("R", Inv)), new XAttribute("height", p.Height.ToString("R", Inv)),
                    Limit("xmin", p.XMin), Limit("xmax", p.XMax), Limit("ymin", p.YMin), Limit("ymax", p.YMax),
                    p.X == null ? null : new XElement(Ws + "x", XmlMathTransform.ToXml(p.X)),
                    new XElement(Ws + "y", p.Y.Select(XmlMathTransform.ToXml)),
                    p.XUnit == null ? null : new XElement(Ws + "xunit", XmlMathTransform.ToXml(p.XUnit)),
                    p.YUnit == null ? null : new XElement(Ws + "yunit", XmlMathTransform.ToXml(p.YUnit))));
                break;
            case SolveBlockRegionPersistentData sb:
                e.Add(new XElement(Ws + "solveblock",
                    new XElement(Ws + "guesses", sb.Guesses.Select(g => new XElement(Ws + "math", XmlMathTransform.ToXml(g)))),
                    new XElement(Ws + "constraints", sb.Constraints.Select(c => new XElement(Ws + "math", XmlMathTransform.ToXml(c)))),
                    new XElement(Ws + "solver", new XElement(Ws + "math", XmlMathTransform.ToXml(sb.Solver)))));
                break;
        }
        return e;
    }

    private static XAttribute? Limit(string name, double? v) => v.HasValue ? new XAttribute(name, v.Value.ToString("R", Inv)) : null;

    private static XElement ParseSection(string xaml) =>
        string.IsNullOrWhiteSpace(xaml) ? XElement.Parse(TextRegionPersistentData.PlainSection("")) : XElement.Parse(xaml);

    public static WorksheetDocument FromXml(XElement root)
    {
        var doc = new WorksheetDocument();
        var s = root.Element(Ws + "settings");
        if (s != null)
        {
            doc.Settings = new WorksheetSettings
            {
                UnitSystem = (string?)s.Attribute("unitSystem") ?? "si",
                AutoCalc = (bool?)s.Attribute("autoCalc") ?? true,
                Decimals = (int?)s.Attribute("decimals") ?? 3,
                ExponentialThreshold = (int?)s.Attribute("expThreshold") ?? 3,
                ShowTrailingZeros = (bool?)s.Attribute("trailingZeros") ?? false,
            };
        }
        foreach (var r in root.Element(Ws + "regions")?.Elements(Ws + "region") ?? [])
        {
            var data = RegionFromXml(r);
            if (data == null) continue;
            data.Id = (string?)r.Attribute("id") ?? data.Id;
            data.Top = Num(r, "top") ?? 0;
            data.Left = Num(r, "left") ?? 0;
            doc.Regions.Add(data);
        }
        return doc;
    }

    private static double? Num(XElement e, string name) =>
        e.Attribute(name) is { } a ? double.Parse(a.Value, Inv) : null;

    private static Statement Stmt(XElement? mathEl) =>
        mathEl?.Elements().FirstOrDefault() is { } x ? XmlMathTransform.StatementFromXml(x) : EmptyStatement.Instance;

    private static MathNode? Node(XElement? holder) =>
        holder?.Elements().FirstOrDefault() is { } x ? XmlMathTransform.NodeFromXml(x) : null;

    private static RegionPersistentData? RegionFromXml(XElement r)
    {
        if (r.Element(Ws + "math") is { } m)
            return new MathRegionPersistentData { Statement = Stmt(m), Source = (string?)m.Attribute("source") };
        if (r.Element(Ws + "text") is { } t)
            return new TextRegionPersistentData
            {
                Width = Num(t, "width") ?? 360,
                SectionXaml = t.Elements().FirstOrDefault()?.ToString(SaveOptions.DisableFormatting) ?? "",
            };
        if (r.Element(Ws + "plot") is { } p)
        {
            var plot = new PlotRegionPersistentData
            {
                Kind = Enum.TryParse<PlotKind>((string?)p.Attribute("kind"), true, out var k) ? k : PlotKind.XY,
                Width = Num(p, "width") ?? 360, Height = Num(p, "height") ?? 240,
                XMin = Num(p, "xmin"), XMax = Num(p, "xmax"), YMin = Num(p, "ymin"), YMax = Num(p, "ymax"),
                X = Node(p.Element(Ws + "x")), XUnit = Node(p.Element(Ws + "xunit")), YUnit = Node(p.Element(Ws + "yunit")),
            };
            plot.Y.AddRange(p.Element(Ws + "y")?.Elements().Select(XmlMathTransform.NodeFromXml) ?? []);
            return plot;
        }
        if (r.Element(Ws + "solveblock") is { } sb)
        {
            var data = new SolveBlockRegionPersistentData { Solver = Stmt(sb.Element(Ws + "solver")?.Element(Ws + "math")) };
            data.Guesses.AddRange(sb.Element(Ws + "guesses")?.Elements(Ws + "math").Select(Stmt) ?? []);
            data.Constraints.AddRange(sb.Element(Ws + "constraints")?.Elements(Ws + "math").Select(Stmt) ?? []);
            return data;
        }
        return null;
    }
}
