using Quire.Undo;

namespace Quire.Controls.Plots.PlotCommands;

internal partial class PlotLegendResizeCommand : IUndoableCommand
{
    private readonly PlotLegend _plotLegend;

    private readonly double _maxWidth;

    private readonly double _minWidth;

    private readonly double _maxItemWidth;

    private readonly double _width;

    private readonly double _minHeight;

    private double _newValueMaxWidth = -1.0;

    private double _newValueMinWidth;

    private double _newValueMaxListWidth;

    private double _newValueWidth;

    private double _newValueMinHeight;

    public string Name { get; }

    public PlotLegendResizeCommand(PlotLegend plotLegend)
    {
        _plotLegend = plotLegend;
        Name = "PlotLegendResizeCommand";
        _maxWidth = _plotLegend.MaxWidth;
        _minWidth = _plotLegend.MinWidth;
        _maxItemWidth = _plotLegend.MaxItemWidth;
        _width = _plotLegend.Width;
        _minHeight = _plotLegend.MinHeight;
    }

    public bool Do()
    {
        return true;
    }

    public void Undo()
    {
        if (_newValueMaxWidth == -1.0)
        {
            _newValueMaxWidth = _plotLegend.MaxWidth;
            _newValueMinWidth = _plotLegend.MinWidth;
            _newValueMaxListWidth = _plotLegend.MaxItemWidth;
            _newValueWidth = _plotLegend.Width;
            _newValueMinHeight = _plotLegend.MinHeight;
        }
        _plotLegend.MaxItemWidth = _maxItemWidth;
        _plotLegend.MaxWidth = _maxWidth;
        _plotLegend.PlotLegendGrid.MaxWidth = _maxWidth;
        _plotLegend.Width = _width;
        _plotLegend.MinWidth = _minWidth;
        _plotLegend.MinHeight = _minHeight;
    }

    public void Redo()
    {
        _plotLegend.MaxItemWidth = _newValueMaxListWidth;
        _plotLegend.MaxWidth = _newValueMaxWidth;
        _plotLegend.PlotLegendGrid.MaxWidth = _newValueMaxWidth;
        _plotLegend.Width = _newValueWidth;
        _plotLegend.MinWidth = _newValueMinWidth;
        _plotLegend.MinHeight = _newValueMinHeight;
    }

    public void Execute()
    {
        CommandBuffer.Execute(this);
    }
}
