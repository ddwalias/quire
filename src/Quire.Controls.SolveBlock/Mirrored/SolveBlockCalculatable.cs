namespace Quire.Controls;

internal partial class SolveBlockCalculatable
{
    public SolveBlockCalculatable(global::Quire.Calculation.Core.ICalculatable wrappedCalculatable)
        : base(wrappedCalculatable)
    {
        UpdateEquationTypeOfSimpleCalculatable(wrappedCalculatable);
    }

    public override global::Quire.Calculation.Core.ICalculatable WrappedCalculatable
    {
        get => base.WrappedCalculatable;
        protected set
        {
            base.WrappedCalculatable = value;
            UpdateEquationType(base.WrappedCalculatable);
        }
    }

    public global::Quire.Calculation.Core.EngineResult EngineResult
    {
        get
        {
            if (!(WrappedCalculatable is global::Quire.Calculation.Core.ICalculatableResultOwner owner))
                return default;
            return owner.EngineResult;
        }
        set
        {
            if (WrappedCalculatable is global::Quire.Calculation.Core.ICalculatableResultOwner owner)
                owner.EngineResult = value;
        }
    }

    public bool InterceptAndUpdateResult
    {
        get
        {
            if (!(WrappedCalculatable is global::Quire.Calculation.Core.ICalculatableResultOwner owner))
                return false;
            return owner.InterceptAndUpdateResult;
        }
    }

    public bool UseICalculatableToRestoreResult
    {
        get
        {
            if (!(WrappedCalculatable is global::Quire.Calculation.Core.ICalculatableResultOwner owner))
                return false;
            return owner.UseICalculatableToRestoreResult;
        }
    }

    public global::Quire.Calculation.Core.EngineError CurrentError
    {
        get
        {
            if (WrappedCalculatable is global::Quire.Calculation.Core.ICalculationErrorOwner owner)
                return owner.CurrentError;
            return null;
        }
    }

    public static int FindMatchingParenthesis(int startParenPos, string str)
    {
        int nesting = 1;
        for (int i = startParenPos + 1; i < str.Length; i++)
        {
            if (str[i] == ')')
                nesting--;
            else if (str[i] == '(')
                nesting++;

            if (nesting == 0)
                return i;
        }

        return -1;
    }

    public static SolveBlockEquationTypes GetEquationTypeOfCalc(global::Quire.Calculation.Core.ICalculatable calc)
    {
        if (calc == null)
            return (SolveBlockEquationTypes)0;

        if (calc is not global::Quire.Calculation.Core.ICalculatableAggregate aggregate ||
            global::Quire.EnumerableExtensions.IsNullOrEmpty(aggregate.CalculatableChildren))
            return GetEquationTypeOfRl(calc.Rl);

        return GetEquationTypeOfRl(
            SolveBlockCalculatableAggregate.ComposeSolveblockChildRlFromEquationControl(
                global::Quire.Calculation.Core.CalculatableAggregateExtensions.GetAggregateRl(aggregate)));
    }
}
