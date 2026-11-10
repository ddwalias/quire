using System;
using System.Collections.Generic;
using System.Globalization;
using Quire.Calculation.Core;
using Quire.Controls.Core;

namespace Quire.Controls.ExcelComponent.Model;

internal partial class ChartMinMaxCalculatable
{
    public string Rl => GetRl("MinMaxPost");

    public Pair<bool, bool> IsFragment => new Pair<bool, bool>
    {
        First = true,
        Second = true
    };

    public bool IsConnectedToTheEngine
    {
        get
        {
            return true;
        }
        set
        {
            DebugTools.FailAssertion(value.ToString());
        }
    }

    public EngineError Error => null;

    internal static string ConstructDefaultLimitRl(ChartWrapperType limitType, string rhsExpression)
    {
        DebugTools.DevAssert((limitType == ChartWrapperType.XMin && rhsExpression == string.Format(CultureInfo.InvariantCulture, "(op_negate (number {0}))", DefaultXMax)) || (limitType == ChartWrapperType.XMax && rhsExpression == string.Format(CultureInfo.InvariantCulture, "(number {0})", DefaultXMax)), null);
        return ChartComponentCalculatableWrapper.ConstructRl(limitType, rhsExpression);
    }

    public IChartComponentCalculatableWrapper GetLimit(ChartWrapperType type)
    {
        if (!_limitWrappers.ContainsKey(type))
        {
            return null;
        }
        return _limitWrappers[type];
    }

    public bool SetLimit(ChartWrapperType type, IChartComponentCalculatableWrapper wrapper)
    {
        if (DebugTools.DevAssert(_limitWrappers.ContainsKey(type), null).Failed)
        {
            return false;
        }
        IChartComponentCalculatableWrapper previousWrapper = _limitWrappers[type];
        _limitWrappers[type] = wrapper;
        return previousWrapper != wrapper;
    }

    internal string GetRl(string id)
    {
        string[] limitRls = new string[8];
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                limitRls[4 * j + i] = GetLimitRl(i, j);
            }
        }
        return string.Format("(fragment {0})", string.Format("(define:0x{0:x8} {1} {2})", _definitionHandle, string.Format("(subscript:0x{0:x8}", _subscriptHandle) + " " + ChartTraceCalculatable.ChartVariableWithLabel + " " + string.Format("(string \"{0}\"{1}:0x{2:x8})", id, IdLabelsExtensions.EmptyLabelRl, _idHandle) + ")", string.Format("(eval (numeric (op_matrix:0x{0:x8} (unboxed {1}) (unboxed {2}) (list {3}))))", _matrixHandle, 4, 2, string.Join(" ", limitRls))));
    }

    public void Commit()
    {
        if (RequestEvaluation != null)
        {
            RequestEvaluation(this);
        }
    }

    public void SetStatusSynchronized()
    {
        if (TreatAsUpToDate != null)
        {
            TreatAsUpToDate(this);
        }
    }

    public void SetResult(IResultNode result)
    {
        DebugTools.DevAssert(result is IEmptyResultNode, null);
    }

    public void SetError(EngineError error)
    {
        DebugTools.DevAssert(error == null, null);
    }

    public void SetRedefinitionWarnings(IList<EngineRedefInfo> warnings)
    {
        if (warnings.IsNotEmpty())
        {
            throw new NotImplementedException();
        }
    }

    public static bool IsLimitsCalculationRelatedCustomError(string errorCode)
    {
        switch (errorCode)
        {
            default:
                return errorCode == "chart_limit_has_units";
            case "chart_limits_too_close":
            case "chart_min_limit_to_be_smaller_than_max":
            case "chart_max_limit_to_be_larger_than_min":
            case "chart_limit_result_not_real_number":
                return true;
        }
    }

    private void SetLimitCalculationError(IChartComponentCalculatableWrapper wrapper, string errorMessage)
    {
        if (errorMessage.IsNotEmpty())
        {
            DebugTools.DevAssert(IsLimitsCalculationRelatedCustomError(new EngineError
            {
                Message = errorMessage
            }.ErrorCode), null);
            wrapper.SetCustomError(errorMessage);
        }
        else if (wrapper.CustomErrorType == OleWrapperCustomErrorType.Calculation)
        {
            DebugTools.DevAssert(IsLimitsCalculationRelatedCustomError(wrapper.Error.ErrorCode), null);
            wrapper.SetCustomError(null);
        }
    }

    private string GetLimitRl(int iLimitAxis, int iLimitSide)
    {
        IChartComponentCalculatableWrapper wrapper = GetWrapper(iLimitAxis, iLimitSide);
        string handleSuffix = string.Format(":0x{0:x8}", _limitHandles[iLimitAxis, iLimitSide]);
        string rlIdWithLabel = null;
        if (wrapper != null)
        {
            rlIdWithLabel = wrapper.RlIdWithLabel;
            if (!wrapper.ChartWrapperType.IsUnknownType())
            {
                DebugTools.AssertNotNull(rlIdWithLabel, null);
            }
        }
        if (rlIdWithLabel == null)
        {
            return string.Format(CultureInfo.InvariantCulture, "(number {0}{1})", double.NaN, handleSuffix);
        }
        return rlIdWithLabel + handleSuffix;
    }

    private ChartWrapperType GetLimitType(int iLimitAxis, int iLimitSide)
    {
        DebugTools.DevAssert(iLimitAxis >= 0 && iLimitAxis < 4 && iLimitSide >= 0 && iLimitSide < 2, null);
        switch (iLimitAxis)
        {
            case 0:
                if (iLimitSide != 0)
                {
                    return ChartWrapperType.XMax;
                }
                return ChartWrapperType.XMin;
            case 1:
                if (iLimitSide != 0)
                {
                    return ChartWrapperType.YMax;
                }
                return ChartWrapperType.YMin;
            case 2:
                if (iLimitSide != 0)
                {
                    return ChartWrapperType.Y2Max;
                }
                return ChartWrapperType.Y2Min;
            case 3:
                if (iLimitSide != 0)
                {
                    return ChartWrapperType.ZMax;
                }
                return ChartWrapperType.ZMin;
            default:
                return ChartWrapperType.Invalid;
        }
    }

    private IChartComponentCalculatableWrapper GetWrapper(int iLimitAxis, int iLimitSide)
    {
        if (!_limitWrappers.TryGetValue(GetLimitType(iLimitAxis, iLimitSide), out var wrapper))
        {
            return null;
        }
        return wrapper;
    }

    public void SetSynchronizationStatus(SynchronizationStatus syncStatus)
    {
    }

    public void SetCalculatingStatus(CalculatingStatus computingStatus)
    {
    }

    public void SetIdentifierLabels(IDictionary<ResolvedId, string[]> idLabelMap)
    {
    }

    public void SetAutoSelectAlgorithm(IDictionary<int, string> autoAlgorithmMap)
    {
    }
}
