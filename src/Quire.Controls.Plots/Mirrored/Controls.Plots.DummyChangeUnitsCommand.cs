namespace Quire.Controls.Plots;

internal partial class DummyChangeUnitsCommand
{
    public DummyChangeUnitsCommand(DelegateFunction0<bool> doMethod, Delegate0 undoMethod)
        : base("UpdatePlotEquationUnits", doMethod, undoMethod)
    {
    }
}
