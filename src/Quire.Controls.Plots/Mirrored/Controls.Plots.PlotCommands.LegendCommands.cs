using Quire.Undo;

namespace Quire.Controls.Plots.PlotCommands;

internal abstract partial class LegendCommands : IUndoableCommand
{
    protected readonly EquationList _equationList;

    protected readonly string _name;

    public string Name => _name;

    protected LegendCommands(EquationList equationList, string commandName)
    {
        _equationList = equationList;
        _name = commandName;
    }

    public abstract bool Do();

    public abstract void Undo();

    public abstract void Redo();

    public void Execute()
    {
        CommandBuffer.Execute(this);
    }
}
