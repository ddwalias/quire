using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Quire.Calculation.Core;
using Quire.Controls.Core;
using Quire.PersistentData;

namespace Quire.Controls.ExcelComponent.Model;

internal partial class ChartMinMaxCalculatable
{
    internal const string MinMaxPre = "MinMaxPre";

    internal const string MinMaxPost = "MinMaxPost";

    private const double DefaultXMin = -10.0;

    private const double DefaultXMax = 10.0;

    private const double LimitsTooCloseThreshold = 1E-15;

    internal const int NumLimitAxes = 4;

    internal const int NumLimitSides = 2;

    private readonly uint _matrixHandle;

    private readonly uint _definitionHandle;

    private readonly uint _subscriptHandle;

    private readonly uint _idHandle;

    private readonly uint[,] _limitHandles = new uint[4, 2];

    private readonly Dictionary<ChartWrapperType, IChartComponentCalculatableWrapper> _limitWrappers = new Dictionary<ChartWrapperType, IChartComponentCalculatableWrapper>
    {
        {
            ChartWrapperType.XMin,
            null
        },
        {
            ChartWrapperType.XMax,
            null
        },
        {
            ChartWrapperType.YMin,
            null
        },
        {
            ChartWrapperType.YMax,
            null
        },
        {
            ChartWrapperType.Y2Min,
            null
        },
        {
            ChartWrapperType.Y2Max,
            null
        },
        {
            ChartWrapperType.ZMin,
            null
        },
        {
            ChartWrapperType.ZMax,
            null
        }
    };

    internal static string ConstructDefaultLimitRl(ChartWrapperType limitType, string rhsExpression)
    {
        DebugTools.DevAssert((limitType == ChartWrapperType.XMin && rhsExpression == string.Format(CultureInfo.InvariantCulture, "(op_negate (number {0}))", DefaultXMax)) || (limitType == ChartWrapperType.XMax && rhsExpression == string.Format(CultureInfo.InvariantCulture, "(number {0})", DefaultXMax)), null);
        return ChartComponentCalculatableWrapper.ConstructRl(limitType, rhsExpression);
    }

    public ChartMinMaxCalculatable()
    {
        uint num = 1u;
        _matrixHandle = num++;
        _definitionHandle = num++;
        _subscriptHandle = num++;
        _idHandle = num++;
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                _limitHandles[i, j] = num++;
            }
        }
    }

    internal double[,] ProcessResult(IResultNode result, double[,] dataLimits, StringBuilder errorBuilder) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    internal void ProcessComponentError(EngineError error)
    {
        if (DebugTools.DevAssert(error != null && error.ErrorSource is IChartMinMaxCommon, null).Failed)
        {
            return;
        }
        IChartComponentCalculatableWrapper chartComponentCalculatableWrapper = null;
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                if (error.NodeHandle == _limitHandles[i, j])
                {
                    chartComponentCalculatableWrapper = GetWrapper(i, j);
                    break;
                }
            }
        }
        if (chartComponentCalculatableWrapper != null)
        {
            if (!chartComponentCalculatableWrapper.ChartWrapperType.IsUnknownType() && DebugTools.DevAssert(chartComponentCalculatableWrapper.Error != null && chartComponentCalculatableWrapper.CustomErrorType == OleWrapperCustomErrorType.None, null).Passed)
            {
                _limitWrappers.Values.Where(limit => limit != null && limit.CustomErrorType == OleWrapperCustomErrorType.Calculation).Each(limit =>
                {
                    limit.SetCustomError(null);
                });
            }
        }
        else
        {
            DebugTools.DevAssert(error.NodeHandle == _definitionHandle, null);
        }
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

    public event Action<ICalculatable> RequestEvaluation;

    public event Func<ICalculatable, string, IResultNode> GetValueOfIdentifierBefore;

    public event Action<ICalculatable> TreatAsUpToDate;

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

    public string Rl => GetRl("MinMaxPost");

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

    public void SetSynchronizationStatus(SynchronizationStatus syncStatus)
    {
    }

    public void SetCalculatingStatus(CalculatingStatus computingStatus)
    {
    }

    public void SetResult(IResultNode result)
    {
        DebugTools.DevAssert(result is IEmptyResultNode, null);
    }

    public void SetError(EngineError error)
    {
        DebugTools.DevAssert(error == null, null);
    }

    public void SetIdentifierLabels(IDictionary<ResolvedId, string[]> idLabelMap)
    {
    }

    public void SetAutoSelectAlgorithm(IDictionary<int, string> autoAlgorithmMap)
    {
    }

    public void SetRedefinitionWarnings(IList<EngineRedefInfo> warnings)
    {
        if (warnings.IsNotEmpty())
        {
            throw new NotImplementedException();
        }
    }

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
}
