namespace Quire.PersistentDataObjects;

/// <summary>Worksheet-wide settings.</summary>
public sealed record WorksheetSettings
{
    public string UnitSystem { get; init; } = "si";
    public bool AutoCalc { get; init; } = true;
    public int Decimals { get; init; } = 3;
    public int ExponentialThreshold { get; init; } = 3;
    public bool ShowTrailingZeros { get; init; }
}

/// <summary>A worksheet as stored on disk, independent of any UI.</summary>
public sealed class WorksheetDocument
{
    public WorksheetSettings Settings { get; set; } = new();
    public List<RegionPersistentData> Regions { get; } = [];
}

/// <summary>Base of every region's saved state. Positions are device-independent pixels (1/96 inch).</summary>
public abstract class RegionPersistentData
{
    public string Id { get; set; } = NewId();
    public double Top { get; set; }
    public double Left { get; set; }

    public static string NewId() => "R" + Guid.NewGuid().ToString("N")[..16];
}

/// <summary>Rich text; <see cref="SectionXaml"/> is a WPF <c>Section</c> element serialised as XAML.</summary>
public sealed class TextRegionPersistentData : RegionPersistentData
{
    public string SectionXaml { get; set; } = "";
    public double Width { get; set; } = 360;

    public static string PlainSection(string text) =>
        "<Section xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xml:space=\"preserve\">"
        + string.Concat(text.Split('\n').Select(line =>
            $"<Paragraph><Run>{System.Security.SecurityElement.Escape(line.TrimEnd('\r'))}</Run></Paragraph>"))
        + "</Section>";
}

public enum PlotKind
{
    XY,
    Polar,
}
