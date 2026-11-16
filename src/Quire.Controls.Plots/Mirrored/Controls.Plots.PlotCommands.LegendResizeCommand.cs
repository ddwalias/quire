namespace Quire.Controls.Plots.PlotCommands;

internal partial class LegendResizeCommand : LegendCommands
{
    private double _maxWidth;

    private double _minWidth;

    private double _maxListWidth;

    private double _width;

    private double _newValueMaxWidth = -1.0;

    private double _newValueMinWidth;

    private double _newValueMaxListWidth;

    private double _newValueWidth;

    public LegendResizeCommand(EquationList equationList)
        : base(equationList, "LegendResizeCommand")
    {
        _maxWidth = _equationList.MaxWidth;
        _minWidth = _equationList.MinWidth;
        _maxListWidth = _equationList.MaxListWidth;
        _width = _equationList.Width;
    }

    public override bool Do()
    {
        return true;
    }

    public override void Undo()
    {
        if (_newValueMaxWidth == -1.0)
        {
            _newValueMaxWidth = _equationList.MaxWidth;
            _newValueMinWidth = _equationList.MinWidth;
            _newValueMaxListWidth = _equationList.MaxListWidth;
            _newValueWidth = _equationList.Width;
        }
        _equationList.MaxListWidth = _maxListWidth;
        _equationList.MaxWidth = _maxWidth;
        _equationList.EquationListGrid.MaxWidth = _maxWidth;
        _equationList.Width = _width;
        _equationList.MinWidth = _minWidth;
    }

    public override void Redo()
    {
        _equationList.MaxListWidth = _newValueMaxListWidth;
        _equationList.MaxWidth = _newValueMaxWidth;
        _equationList.EquationListGrid.MaxWidth = _newValueMaxWidth;
        _equationList.Width = _newValueWidth;
        _equationList.MinWidth = _newValueMinWidth;
    }
}
