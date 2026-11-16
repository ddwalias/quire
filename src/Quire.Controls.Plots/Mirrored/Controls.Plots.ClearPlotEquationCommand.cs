using Quire.Controls.Plots.Presenter;
using Quire.Undo;

namespace Quire.Controls.Plots;

internal partial class ClearPlotEquationCommand : IUndoableCommand
{
    private readonly IPlotEquation _plotEquation;

    private CompoundCommand _nestedCommand;

    private EquationControl _lastFocusedEquation;

    public string Name => "Clear Plot Equation content";

    public ClearPlotEquationCommand(IPlotEquation plotEquation)
    {
        _plotEquation = plotEquation;
    }

    public bool Do()
    {
        if (_plotEquation.IsPlotEquationEmpty)
        {
            return false;
        }
        _nestedCommand = new CompoundCommand();
        _lastFocusedEquation = _plotEquation.ActiveEquation;
        using (new CommandScope(_nestedCommand))
        {
            _plotEquation.EquationSExpression = _plotEquation.EmptyPlotEquationSExp;
        }
        _plotEquation.NavigateRegionStart();
        return true;
    }

    public void Undo()
    {
        _nestedCommand.Undo();
        _lastFocusedEquation.Focus();
    }

    public void Redo()
    {
        _nestedCommand.Redo();
        _plotEquation.NavigateRegionStart();
    }
}
