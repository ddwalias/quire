#nullable disable
#pragma warning disable
using System;
using System.ComponentModel;
using Quire.Calculation.Core;

namespace Quire.Controls.Include;

public partial class SystemVariableValuesHandler
{
    private const string EvalExpressionRlFormat = "(eval (numeric ({0})))";
    private readonly string _variableName;

    public event Action<string, object> SystemVariableValueChanged;

    public SystemVariableValuesHandler(string variableName)
    {
        _variableName = variableName;
        ConstractRl();
    }

    private void ConstractRl()
    {
        Rl = string.Format(EvalExpressionRlFormat, _variableName);
    }

    public override void SetResult(IResultNode result)
    {
        base.SetResult(result);
        object computedValue = GetComputedValue();
        if (SystemVariableValueChanged != null && computedValue != null)
        {
            SystemVariableValueChanged(_variableName, computedValue);
        }
    }

    private object GetComputedValue()
    {
        if (Result is RealSimpleResultNode)
        {
            return (Result as RealSimpleResultNode).Value;
        }
        if (Result is StringSimpleResultNode)
        {
            return (Result as StringSimpleResultNode).Value;
        }
        return null;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public bool CanHideFromCalculationContext => true;

    public bool HideFromCalculationContext { get; set; }
}
