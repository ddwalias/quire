namespace Quire.Controls.Plots.PlotCommands;

internal partial class AxisSetEndExpressionCommand : AxisCommand
{
    private readonly string _endExpression;

    private string _prevEndExpression;

    public AxisSetEndExpressionCommand(PlotGraphAxis axis, string endExpression)
        : base(axis, "Set End Expression Command")
    {
        _endExpression = endExpression;
    }

    public override bool Do()
    {
        _prevEndExpression = _axis.EndExpressionEqn.EquationSExpression;
        _axis.EndExpressionEqn.EquationSExpression = _endExpression;
        _axis.EndExpressionEqn.Commit();
        return true;
    }

    public override void Undo()
    {
        _axis.EndExpressionEqn.EquationSExpression = _prevEndExpression;
        _axis.EndExpressionEqn.Commit();
    }

    public override void Redo()
    {
        Do();
    }
}
