using Quire.Undo;

namespace Quire.Controls.Plots.PlotCommands;

internal abstract partial class PlotCommands : IUndoableCommand
{
    protected readonly PlotControl _plot;

    protected readonly string _name;

    public string Name => _name;

    protected PlotCommands(PlotControl plot, string commandName)
    {
        _plot = plot;
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
