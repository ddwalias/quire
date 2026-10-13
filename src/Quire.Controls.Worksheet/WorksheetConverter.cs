using System.IO;
using System.Globalization;
using System.IO.Compression;
using System.Xml.Linq;
using Quire.EquationEditor.Tree;
using Quire.EquationEditor.Tree.Transform;
using Quire.PersistentDataObjects;

namespace Quire.Core;

/// <summary>Result of importing another application's worksheet.</summary>
public sealed record ImportResult(WorksheetDocument Document, IReadOnlyList<string> Warnings);

/// <summary>
/// Imports Mathcad Prime worksheets (<c>.mcdx</c>) and templates (<c>.mctx</c>). These are zip packages whose
/// <c>mathcad/worksheet.xml</c> lists positioned regions; math uses the same element vocabulary as Quire's
/// own format and text regions reference nested packages holding a WPF <c>Section</c>.
/// Regions that can't be represented are replaced by a text note, and listed in the warnings.
/// </summary>
public static class WorksheetConverter
{
    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

    public static ImportResult ImportMcdx(string path)
    {
        using var fs = File.OpenRead(path);
        return ImportMcdx(fs);
    }

    public static ImportResult ImportMcdx(Stream stream)
    {
        using var zip = new ZipArchive(stream, ZipArchiveMode.Read, leaveOpen: true);
        var ws = Load(zip, "mathcad/worksheet.xml") ?? throw new InvalidDataException("Not a Mathcad Prime worksheet (missing mathcad/worksheet.xml)");
        var rels = Load(zip, "mathcad/_rels/worksheet.xml.rels");
        var relTargets = rels?.Root?.Elements().ToDictionary(r => (string)r.Attribute("Id")!, r => ((string)r.Attribute("Target")!).TrimStart('/'))
                         ?? [];
        var warnings = new List<string>();
        var doc = new WorksheetDocument { Settings = ReadSettings(zip) };

        if (ReadOrigin(zip) is int origin and not 0)
            doc.Regions.Add(new MathRegionPersistentData
            {
                Top = -24, Left = 0,
                Statement = new DefineStatement(new IdNode("ORIGIN"), new NumberNode(origin.ToString(Inv)), Global: true),
            });

        var regions = ws.Root!.Elements().FirstOrDefault(e => e.Name.LocalName == "regions");
        if (regions != null) ReadRegions(regions, 0, 0, zip, relTargets, doc, warnings);

        // keep everything at non-negative coordinates
        double minTop = doc.Regions.Count > 0 ? doc.Regions.Min(r => r.Top) : 0;
        if (minTop < 0) foreach (var r in doc.Regions) r.Top -= minTop;
        return new ImportResult(doc, warnings);
    }

    private static XDocument? Load(ZipArchive zip, string name)
    {
        var entry = zip.GetEntry(name);
        if (entry == null) return null;
        using var s = entry.Open();
        return XDocument.Load(s);
    }

    private static WorksheetSettings ReadSettings(ZipArchive zip)
    {
        var calc = Load(zip, "mathcad/settings/calculation.xml")?.Root;
        var unitSystem = calc?.Descendants().FirstOrDefault(e => e.Name.LocalName == "currentUnitSystem")?.Attribute("name")?.Value;
        var behavior = calc?.Descendants().FirstOrDefault(e => e.Name.LocalName == "calculationBehavior");
        return new WorksheetSettings
        {
            UnitSystem = unitSystem is "uscs" or "cgs" ? unitSystem : "si",
            AutoCalc = behavior?.Attribute("automatic-recalculation")?.Value != "false",
        };
    }

    private static int? ReadOrigin(ZipArchive zip)
    {
        var calc = Load(zip, "mathcad/settings/calculation.xml")?.Root;
        var v = calc?.Descendants().FirstOrDefault(e => e.Name.LocalName == "builtInVariables")?.Attribute("array-origin")?.Value;
        return int.TryParse(v, NumberStyles.Integer, Inv, out var o) ? o : null;
    }

    private static void ReadRegions(XElement regions, double dy, double dx, ZipArchive zip,
        Dictionary<string, string> rels, WorksheetDocument doc, List<string> warnings)
    {
        foreach (var region in regions.Elements().Where(e => e.Name.LocalName == "region"))
        {
            double top = dy + Attr(region, "top"), left = dx + Attr(region, "left");
            var content = region.Elements().FirstOrDefault();
            if (content == null) continue;
            switch (content.Name.LocalName)
            {
                case "math":
                    doc.Regions.Add(ImportMath(content, top, left, warnings));
                    break;
                case "text":
                    doc.Regions.Add(ImportText(content, region, top, left, zip, rels, warnings));
                    break;
                case "Area" or "area":
                    // collapsible area: flatten its regions, offset by the area position
                    var inner = content.Elements().FirstOrDefault(e => e.Name.LocalName == "regions");
                    if (inner != null) ReadRegions(inner, top, left, zip, rels, doc, warnings);
                    break;
                case "pageBreak":
                    break; // Quire paginates automatically
                default:
                    warnings.Add($"Region at ({left:0}, {top:0}): <{content.Name.LocalName}> is not supported and was replaced by a note.");
                    doc.Regions.Add(Note($"[Imported worksheet: {content.Name.LocalName} region not supported]", top, left));
                    break;
            }
        }
    }

    private static double Attr(XElement e, string name) =>
        e.Attribute(name) is { } a && double.TryParse(a.Value, NumberStyles.Float, Inv, out var v) ? v : 0;

    private static RegionPersistentData ImportMath(XElement math, double top, double left, List<string> warnings)
    {
        var expr = math.Elements().FirstOrDefault();
        if (expr == null) return new MathRegionPersistentData { Top = top, Left = left };
        try
        {
            return new MathRegionPersistentData { Top = top, Left = left, Statement = XmlMathTransform.StatementFromXml(expr) };
        }
        catch (Exception ex) when (ex is UnsupportedMathXmlException or InvalidCastException or IndexOutOfRangeException or FormatException
                                       or NullReferenceException or InvalidOperationException or OverflowException or KeyNotFoundException)
        {
            warnings.Add($"Math at ({left:0}, {top:0}): {ex.Message}; replaced by a note.");
            return Note($"[Imported math not supported: {ex.Message}]", top, left);
        }
    }

    private static RegionPersistentData ImportText(XElement text, XElement region, double top, double left, ZipArchive zip,
        Dictionary<string, string> rels, List<string> warnings)
    {
        double width = Attr(region, "width") is > 0 and var w ? w : 360;
        var id = text.Attribute("item-idref")?.Value;
        if (id != null && rels.TryGetValue(id, out var target) && zip.GetEntry(target) is { } entry)
        {
            try
            {
                using var outer = entry.Open();
                using var buffer = new MemoryStream();
                outer.CopyTo(buffer);
                buffer.Position = 0;
                using var package = new ZipArchive(buffer, ZipArchiveMode.Read);
                var docEntry = package.Entries.FirstOrDefault(e => e.FullName.EndsWith(".xaml", StringComparison.OrdinalIgnoreCase));
                if (docEntry != null)
                {
                    using var s = docEntry.Open();
                    var section = XDocument.Load(s).Root!;
                    return new TextRegionPersistentData { Top = top, Left = left, Width = width, SectionXaml = section.ToString(SaveOptions.DisableFormatting) };
                }
            }
            catch (Exception ex) when (ex is InvalidDataException or System.Xml.XmlException)
            {
                warnings.Add($"Text at ({left:0}, {top:0}) could not be read: {ex.Message}");
            }
        }
        return new TextRegionPersistentData { Top = top, Left = left, Width = width, SectionXaml = TextRegionPersistentData.PlainSection(text.Value) };
    }

    private static TextRegionPersistentData Note(string message, double top, double left) =>
        new() { Top = top, Left = left, SectionXaml = TextRegionPersistentData.PlainSection(message) };
}
