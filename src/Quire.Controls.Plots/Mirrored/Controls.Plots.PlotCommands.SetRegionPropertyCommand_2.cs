using Quire;
using Quire.Undo;

namespace Quire.Controls.Plots.PlotCommands;

internal partial class SetRegionPropertyCommand<TRegion, TProperty> : IUndoableCommand
{
    protected readonly TRegion _region;

    protected readonly TProperty _newPropValue;

    protected readonly TProperty _oldPropValue;

    private readonly DelegateFunction1<TProperty, TRegion> _getPropValue;

    private readonly Delegate2<TRegion, TProperty> _setPropValue;

    public string Name { get; private set; }

    public SetRegionPropertyCommand(TRegion region, TProperty newPropValue, string name, DelegateFunction1<TProperty, TRegion> getPropValue, Delegate2<TRegion, TProperty> setPropValue)
    {
        Name = name;
        _getPropValue = getPropValue;
        _setPropValue = setPropValue;
        _region = region;
        _newPropValue = newPropValue;
        _oldPropValue = getPropValue(_region);
    }

    public bool Do()
    {
        if (!object.Equals(_getPropValue(_region), _newPropValue))
        {
            _setPropValue(_region, _newPropValue);
            return true;
        }
        return false;
    }

    public void Undo()
    {
        _setPropValue(_region, _oldPropValue);
    }

    public void Redo()
    {
        Do();
    }

    public void Execute()
    {
        CommandBuffer.Execute(this);
    }
}
