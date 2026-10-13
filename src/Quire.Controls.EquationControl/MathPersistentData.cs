using Quire.EquationEditor.Tree;

namespace Quire.PersistentDataObjects;

public sealed class MathRegionPersistentData : RegionPersistentData
{
    public Statement Statement { get; set; } = EmptyStatement.Instance;

    /// <summary>Keyboard text kept when it doesn't parse yet; <see cref="Statement"/> is then empty.</summary>
    public string? Source { get; set; }
}

public sealed class PlotRegionPersistentData : RegionPersistentData
{
    public PlotKind Kind { get; set; } = PlotKind.XY;
    public MathExpr? X { get; set; }
    public List<MathExpr> Y { get; } = [];
    public MathExpr? XUnit { get; set; }
    public MathExpr? YUnit { get; set; }
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
