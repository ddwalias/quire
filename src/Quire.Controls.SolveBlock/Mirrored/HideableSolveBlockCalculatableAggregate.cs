using System.ComponentModel;

namespace Quire.Controls;

internal partial class HideableSolveBlockCalculatableAggregate
{
    public override global::Quire.Calculation.Core.ICalculatable WrappedCalculatable
    {
        get => base.WrappedCalculatable;
        protected set
        {
            // The reference branches to the update body on inequality and falls through to return on equality.
            if (base.WrappedCalculatable != value)
                goto UpdateWrappedCalculatable;
            return;

        UpdateWrappedCalculatable:
            UnhookHideableEvents(base.WrappedCalculatable);
            base.WrappedCalculatable = value;
            HookupHideableEvents(base.WrappedCalculatable);
        }
    }

    public bool CanHideFromCalculationContext
    {
        get
        {
            if (WrappedCalculatable is global::Quire.Calculation.Core.ICalculationHideableChild child)
                return child.CanHideFromCalculationContext;
            return false;
        }
    }

    public bool HideFromCalculationContext
    {
        get
        {
            if (WrappedCalculatable is global::Quire.Calculation.Core.ICalculationHideableChild child)
                return child.HideFromCalculationContext;
            return false;
        }
        set
        {
            if (WrappedCalculatable is global::Quire.Calculation.Core.ICalculationHideableChild child)
                child.HideFromCalculationContext = value;
        }
    }

    public HideableSolveBlockCalculatableAggregate(
        global::Quire.Calculation.Core.ICalculatableAggregate calculatableAggregate)
        : base(calculatableAggregate)
    {
        HookupHideableEvents(calculatableAggregate);
    }

    private void WrappedCalculatablePropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        FirePropertyChanged(e);
    }

    public override void Dispose()
    {
        base.Dispose();
        UnhookHideableEvents(base.WrappedCalculatable);
    }

    private void HookupHideableEvents(global::Quire.Calculation.Core.ICalculatable calc)
    {
        if (calc is global::Quire.Calculation.Core.ICalculationHideableChild child)
            child.PropertyChanged += WrappedCalculatablePropertyChanged;
    }

    private void UnhookHideableEvents(global::Quire.Calculation.Core.ICalculatable calc)
    {
        if (calc is global::Quire.Calculation.Core.ICalculationHideableChild child)
            child.PropertyChanged -= WrappedCalculatablePropertyChanged;
    }
}
