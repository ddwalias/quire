namespace Quire.Controls.Plots.PlotCommands;

internal partial class AxisSetStartExpressionCommand : AxisCommand
{
    private readonly string _startExpression;

    private string _prevStartExpression;

    public AxisSetStartExpressionCommand(PlotGraphAxis axis, string startExpression)
        : base(axis, "Set Start Expression Command")
    {
        _startExpression = startExpression;
    }

    public override bool Do()
    {
        _prevStartExpression = _axis.StartExpressionEqn.EquationSExpression;
        _axis.StartExpressionEqn.EquationSExpression = _startExpression;
        _axis.StartExpressionEqn.Commit();
        return true;
    }

    public override void Undo()
    {
        _axis.StartExpressionEqn.EquationSExpression = _prevStartExpression;
        _axis.StartExpressionEqn.Commit();
    }

    public override void Redo()
    {
        Do();
    }
}
