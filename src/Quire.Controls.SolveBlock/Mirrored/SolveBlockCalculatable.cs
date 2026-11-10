namespace Quire.Controls;

internal partial class SolveBlockCalculatable
{
    public event global::System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

    public SolveBlockCalculatable(global::Quire.Calculation.Core.ICalculatable wrappedCalculatable)
        : base(wrappedCalculatable)
    {
        UpdateEquationTypeOfSimpleCalculatable(wrappedCalculatable);
    }

    public SolveBlockEquationTypes EquationType
    {
        get => _equationType;
        protected set
        {
            if (_equationType == value)
                return;

            var eventArgs = new global::Quire.Wpf.PropertyChangedEventArgs<SolveBlockEquationTypes>(
                "EquationType", _equationType, value);
            _equationType = value;
            FirePropertyChanged(eventArgs);
        }
    }

    protected bool UpdateEquationTypeOfSimpleCalculatable(global::Quire.Calculation.Core.ICalculatable calc)
    {
        SolveBlockEquationTypes oldEquationType = EquationType;
        if (calc == null)
        {
            EquationType = (SolveBlockEquationTypes)0;
            return EquationType != oldEquationType;
        }

        EquationType = GetEquationTypeOfRl(calc.Rl);
        return EquationType != oldEquationType;
    }

    protected virtual bool UpdateEquationType(global::Quire.Calculation.Core.ICalculatable calc)
    {
        return UpdateEquationTypeOfSimpleCalculatable(calc);
    }

    protected void FirePropertyChanged(global::System.ComponentModel.PropertyChangedEventArgs eventArgs)
    {
        global::System.ComponentModel.PropertyChangedEventHandler propertyChanged = PropertyChanged;
        if (propertyChanged != null)
            propertyChanged(this, eventArgs);
    }

    protected override void HandleRequestEvaluation(global::Quire.Calculation.Core.ICalculatable calc)
    {
        if (!UpdateEquationType(WrappedCalculatable))
            base.HandleRequestEvaluation(calc);
    }

    public virtual bool ContainsError(global::Quire.Calculation.Core.EngineError error)
    {
        return error.ErrorSource == this;
    }

    public virtual void ShowError(global::Quire.Calculation.Core.EngineError error)
    {
        if (WrappedCalculatable is global::Quire.Calculation.Core.ICalculationErrorOwner owner)
            owner.ShowError(error);
    }

    public void RestoreResult()
    {
        if (WrappedCalculatable is global::Quire.Calculation.Core.ICalculatableResultOwner owner)
            owner.RestoreResult();
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
