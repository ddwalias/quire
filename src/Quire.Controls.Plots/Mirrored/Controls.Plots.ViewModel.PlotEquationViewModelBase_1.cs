#nullable disable
#pragma warning disable

using Quire.Calculation.Core;

namespace Quire.Controls.Plots.ViewModel;

public partial class PlotEquationViewModelBase<TModel>
{
    protected TModel Model { get; private set; }

    internal static partial class PropertyNames
    {
        internal const string PlotCalculatableError = "PlotCalculatableError";
    }

    public EngineError PlotCalculatableError
    {
        get
        {
            return Model.PlotCalculatableError;
        }
        set
        {
            TModel model = Model;
            model.PlotCalculatableError = value;
        }
    }

    internal PlotEquationViewModelBase(TModel equationModel)
    {
        Model = equationModel;
        Model.PropertyChanged += ModelPropertyChangedInner;
    }
}
