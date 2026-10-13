using Quire.EquationEditor.Tree;

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

public sealed class MathRegionPersistentData : RegionPersistentData
{
    public Statement Statement { get; set; } = EmptyStatement.Instance;

    /// <summary>Keyboard text kept when it doesn't parse yet; <see cref="Statement"/> is then empty.</summary>
    public string? Source { get; set; }
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

public sealed class PlotRegionPersistentData : RegionPersistentData
{
    public PlotKind Kind { get; set; } = PlotKind.XY;
    public MathNode? X { get; set; }
    public List<MathNode> Y { get; } = [];
    public MathNode? XUnit { get; set; }
    public MathNode? YUnit { get; set; }
    public double Width { get; set; } = 360;
    public double Height { get; set; } = 240;
    public double? XMin { get; set; }
    public double? XMax { get; set; }
    public double? YMin { get; set; }
    public double? YMax { get; set; }
}

/// <summary>Solve block: guess values, constraints and one solver definition (find, minerr, minimize, maximize).</summary>
public sealed class SolveBlockRegionPersistentData : RegionPersistentData
{
    public List<Statement> Guesses { get; } = [];
    public List<Statement> Constraints { get; } = [];
    public Statement Solver { get; set; } = EmptyStatement.Instance;
}
