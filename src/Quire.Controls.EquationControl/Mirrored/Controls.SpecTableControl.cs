using Quire.Calculation.Core;
using Quire.Controls.Core;

namespace Quire.Controls;

public partial class SpecTableControl
{
    public bool CalcIsDisabled
    {
        get => (bool)GetValue(CalcIsDisabledProperty);
        set => SetValue(CalcIsDisabledProperty, value);
    }

    public bool IsEmpty => false;

    public bool IsActive
    {
        get;
        set;
    }

    public SynchronizationStatus SynchronizationStatus
    {
        get => (SynchronizationStatus)GetValue(SynchronizationStatusProperty);
        set => SetValue(SynchronizationStatusProperty, value);
    }

    public CalculatingStatus CalculatingStatus
    {
        get => (CalculatingStatus)GetValue(CalculatingStatusProperty);
        set => SetValue(CalculatingStatusProperty, value);
    }

    public bool CanHideFromCalculationContext => true;

    public MathFormat MathFormat
    {
        get;
        set;
    }
}
