using System.Collections.Generic;
using System.Linq;
using Quire.Controls.Plots.Presenter;
using Quire.Undo;

namespace Quire.Controls.Plots.PlotCommands;

internal abstract partial class TraceCommands : IUndoableCommand
{
    protected readonly IList<IPlotTraceStyle> _traceList;

    protected readonly string _name;

    public string Name => _name;

    protected TraceCommands(IEnumerable<IPlotTraceStyle> traceList, string commandName)
    {
        _traceList = traceList.ToList();
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
