using Quire.Controls.Core;
using Quire.Controls.Plots;
using Quire.EquationEditor.Tree;
using Quire.PersistentDataObjects;

namespace Quire.Controls.Worksheet;

public interface IControlFactory
{
    RegionControl CreateRegion(RegionPersistentData data);
    EquationControl CreateEquation();
    TextRegionControl CreateText();
    XYPlotControl CreateXYPlot();
    XYPlotControl CreatePolarPlot();
    SolveBlockControl CreateSolveBlock();
}

public sealed class ControlFactory : IControlFactory
{
    public static readonly ControlFactory Instance = new();

    public RegionControl CreateRegion(RegionPersistentData data) => data switch
    {
        MathRegionPersistentData m => EquationControl.FromData(m),
        TextRegionPersistentData t => new TextRegionControl(t),
        PlotRegionPersistentData p => new XYPlotControl(p),
        SolveBlockRegionPersistentData s => new SolveBlockControl(s),
        _ => throw new ArgumentException($"Unknown region type {data.GetType().Name}"),
    };

    public EquationControl CreateEquation() => new(EmptyStatement.Instance);

    public TextRegionControl CreateText() => new();

    public XYPlotControl CreateXYPlot() => new();

    public XYPlotControl CreatePolarPlot() => new(new PlotRegionPersistentData { Kind = PlotKind.Polar, Width = 280, Height = 280 });

    public SolveBlockControl CreateSolveBlock() => new();
}
