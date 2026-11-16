namespace Quire.Calculation;

public partial class CalculationBinder
{
    public CalculationBinder()
    {
    }

    public virtual void BindTo(global::Quire.Calculation.Core.ICalculatableContext context)
    {
        CalculationController.BindTo(context);
    }
}
