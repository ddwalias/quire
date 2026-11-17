using System.ComponentModel;
using Quire.Calculation.Core;

namespace Quire.Controls.Plots.Model;

internal partial class PlotEquationModelBase : IPlotEquationModelBase, INotifyPropertyChanged
{
    internal static partial class PropertyNames
    {
        internal const string PlotCalculatableError = "PlotCalculatableError";
    }

    private EngineError _plotCalculatableError;

    public EngineError PlotCalculatableError
    {
        get
        {
            return _plotCalculatableError;
        }
        set
        {
            _plotCalculatableError = value;
            FirePropertyChanged(new PropertyChangedEventArgs("PlotCalculatableError"));
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void FirePropertyChanged(PropertyChangedEventArgs propertyChangedEventArgs)
    {
        // IL requires an explicit local to emit stloc.0 rather than dup.
        PropertyChangedEventHandler handler = PropertyChanged;
        if (handler != null)
        {
            handler(this, propertyChangedEventArgs);
        }
    }

    public PlotEquationModelBase()
    {
    }
}
