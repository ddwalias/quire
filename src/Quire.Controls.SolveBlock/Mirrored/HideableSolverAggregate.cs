using System.ComponentModel;

namespace Quire.Controls;

internal partial class HideableSolverAggregate
{
    public bool CanHideFromCalculationContext
    {
        get
        {
            if (WrappedCalculatable is global::Quire.Calculation.Core.ICalculationHideableChild)
                return (WrappedCalculatable as global::Quire.Calculation.Core.ICalculationHideableChild).CanHideFromCalculationContext;
            return false;
        }
    }

    public bool HideFromCalculationContext
    {
        get
        {
            if (WrappedCalculatable is global::Quire.Calculation.Core.ICalculationHideableChild)
                return (WrappedCalculatable as global::Quire.Calculation.Core.ICalculationHideableChild).HideFromCalculationContext;
            return false;
        }
        set
        {
            if (WrappedCalculatable is not global::Quire.Calculation.Core.ICalculationHideableChild child)
                return;
            child.HideFromCalculationContext = value;
        }
    }

    public HideableSolverAggregate(global::Quire.Calculation.Core.ICalculatable calculatable)
        : base(calculatable)
    {
        if (calculatable is global::Quire.Calculation.Core.ICalculationHideableChild child)
            child.PropertyChanged += WrappedCalculatablePropertyChanged;
    }

    private void WrappedCalculatablePropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        FirePropertyChanged(e);
    }
}
