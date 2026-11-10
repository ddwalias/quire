using System;
using Quire;
using Quire.Calculation.Core;
using Quire.Calculation.Core.Support;

namespace Quire.Controls.ExcelComponent.Model;

internal partial class ExcelComponentCalculatableWrapper
{
    private ExcelComponentEquationType _equationType;

    public ExcelComponentCalculatableWrapper(ICalculatable wrappedCalculatable)
        : base(wrappedCalculatable)
    {
    }

    public event Action<ICalculatable, ExcelComponentEquationType, ExcelComponentEquationType> ExcelComponentEquationTypeChanged;

    public ExcelComponentEquationType ExcelComponentEquationType
    {
        get
        {
            return _equationType;
        }
        private set
        {
            if (_equationType == value)
            {
                return;
            }
            ExcelComponentEquationType equationType = _equationType;
            _equationType = value;
            if (ExcelComponentEquationTypeChanged != null)
            {
                ExcelComponentEquationTypeChanged(WrappedCalculatable, equationType, _equationType);
            }
        }
    }

    public override bool IsOutputEquation => ExcelComponentEquationType == ExcelComponentEquationType.Output;

    public override bool IsInputEquation => ExcelComponentEquationType == ExcelComponentEquationType.Input;

    public override bool IsInvalidEquation => IsTypeInvalid(ExcelComponentEquationType);

    public override bool IsUnknownEquation => ExcelComponentEquationType == ExcelComponentEquationType.Unknown;

    public override Pair<bool, bool> IsFragment
    {
        get
        {
            return new Pair<bool, bool>
            {
                First = IsValidEquation,
                Second = !IsUnknownEquation
            };
        }
    }

    protected override void ClearEquationType()
    {
        ExcelComponentEquationType = ExcelComponentEquationType.Unknown;
    }

    protected override void HandleRequestEvaluation(ICalculatable calc)
    {
        if (!UpdateExcelComponentEquationType() || !IsInvalidEquation)
        {
            base.HandleRequestEvaluation(calc);
        }
    }

    public override string Rl
    {
        get
        {
            return string.Format("(fragment {0})", base.Rl);
        }
    }

    private static bool IsTypeInvalid(ExcelComponentEquationType type)
    {
        return type == ExcelComponentEquationType.Invalid;
    }

    internal bool UpdateExcelComponentEquationType()
    {
        bool isInput;
        string lhsExpression = null;
        string rhsExpression = null;
        ExcelComponentEquationType excelComponentEquationType = ExcelComponentEquationType.Invalid;
        ExcelComponentEquationType equationType = _equationType;
        if (IsSupportedWrappedCalculatable && ExcelComponentRlParser.ParseExcelComponentEquationRl(WrappedCalculatable.Rl, out isInput, out lhsExpression, out rhsExpression))
        {
            excelComponentEquationType = (isInput ? ExcelComponentEquationType.Input : ExcelComponentEquationType.Output);
        }
        LhsExpression = lhsExpression;
        RhsExpression = rhsExpression;
        ExcelComponentEquationType = excelComponentEquationType;
        if (!IsTypeInvalid(equationType))
        {
            return equationType != excelComponentEquationType;
        }
        return true;
    }
}
