using Quire.Undo;

namespace Quire.Controls.Plots.PlotCommands;

internal abstract partial class AxisCommand : IUndoableCommand
{
    protected readonly PlotGraphAxis _axis;

    protected readonly string _name;

    public string Name => _name;

    protected AxisCommand(PlotGraphAxis axis, string commandName)
    {
        _axis = axis;
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
