using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Quire.Calculation;
using Quire.Calculation.Core;
using Quire.ChartAppLib;
using Quire.Controls.Core;
using Quire.FunctionalitiesLimitation;
using Quire.OLE;
using Quire.PersistentData;
using Quire.Serialization;
using Quire.Automation;
using IMatrixValue = Mathcad.IMatrixValue;
using IValue = Mathcad.IValue;
using System.Runtime.InteropServices;
using System.Windows;

namespace Quire.Controls.ExcelComponent.Model;

public partial class ChartContainerModel
{
    // Compile order keeps the iterator at Mathcad's d__42 ordinal; C# counts the field-like event differently.
    private readonly global::Quire.Calculation.Core.Trace2DResults.Trace2DResultConverter _converter;
    public static readonly string ComExceptionEngineErrorCode;
    private static readonly string[] _excludeElementsNames;
    private string _helpServerPath;
    private uint[] _traceIds;
    private readonly object _dataLimitsLock;
    private Dictionary<uint, double[,]> _dataLimitsDict;
    public override string ProgId => "PTC.ChartAppOLE";
    public override Dictionary<Guid, Guid> GuidsConversionDict => OleUtils.ChartAppOleGuidsConversionDict;
    protected override void OnStartRunning()
    {
        base.OnStartRunning();
        if (IsImageFrozen)
        {
            ApplyImageFrozenState();
        }
        SetHelpServerPathIfRunning();
    }
    public string HelpServerPath
    {
        get
        {
            return _helpServerPath;
        }
        set
        {
            _helpServerPath = value;
            SetHelpServerPathIfRunning();
        }
    }
    protected override string[] ExcludeElementsNames => _excludeElementsNames;
    public override bool AsyncUpdateVisibleSize => true;
    protected override EnumFunctionalityKey ToggleActivateStateFunctionalityKey => (EnumFunctionalityKey)800;
    protected override void DoDeactivate()
    {
        ChartContainerModelUtils.WaitForChartImagesReady(new ChartContainerModel[1] { this }, null);
        base.DoDeactivate();
    }
    public override global::System.Collections.Generic.IList<global::Quire.Serialization.IRegionPersistentData> SetRegionPersistentData(global::Quire.Serialization.IDeS11NProvider d11NProvider, global::Quire.PersistentData.chartContainerType chartContainer)
    {
        Unpack(d11NProvider, chartContainer.itemidref);
        return null;
    }
    public override chartContainerType GetRegionPersistentData(IS11NProvider s11NProvider, string uniqueId, Action onServerBusy)
    {
        return new chartContainerType(SizeData)
        {
            itemidref = Pack(s11NProvider, uniqueId, ExternalObjectType.Chart, onServerBusy)
        };
    }
    public bool IsTypeDefault => true;
    public global::Quire.Calculation.Core.Trace2DResults.TraceType Type => default;
    public override void Commit()
    {
        AssertDispatcherAccess();
        RemoveImageFreezeSource(ImageFreezeSources.AsynchronousCalculation);
    }
    public override bool ShouldCommitAtLeastOnce => false;
    public override bool ShouldCommitIfDirty => true;
    public override void SetValue(string place, IValue value, IValueToIResultNodeTransform resultsTransform)
    {
        CheckInstalledAndAvailable(forceRun: true);
        switch (value.Type)
        {
            case "String":
                SetString(place, value);
                break;
            case "Matrix":
                SetMatrix(place, value, resultsTransform);
                break;
            default:
                throw new Exception(string.Format("Unsupported type {0}", value.Type));
        }
    }
    public override IValue GetValue(string place)
    {
        throw new NotImplementedException();
    }
    public Exception RemoveTrace(uint traceId)
    {
        lock (_dataLimitsLock)
        {
            if (_dataLimitsDict.IsNotEmpty())
            {
                _dataLimitsDict.Remove(traceId);
            }
        }
        try
        {
            ChartObject.deleteTraceByTraceId(ChartTraceCalculatable.TraceIdForChartApp(traceId));
            return null;
        }
        catch (Exception result)
        {
            return result;
        }
    }
    private void SetString(string place, IValue value)
    {
        throw new AddressErrorException(place);
    }
    private void SetMatrix(string place, IValue value, IValueToIResultNodeTransform resultsTransform)
    {
        DateTime? refTime = DebugTools.LogData(string.Format("ChartContainerModel.SetMatrix: {0}: Start", place));
        try
        {
            IMatrixValue matrixValue = (IMatrixValue)value;
            if (place == "MinMaxPre" || place == "MinMaxPost")
            {
                IResultNode arg = resultsTransform.Transform((IAutomationValue)value);
                if (DebugTools.AssertNotNull(OnProcessMinMax, null).Failed)
                {
                    return;
                }
                StringBuilder stringBuilder = new StringBuilder();
                double[,] array = OnProcessMinMax(arg, (place == "MinMaxPost") ? DataLimits : null, stringBuilder);
                if (array == null)
                {
                    throw new Exception(stringBuilder.ToString());
                }
                if (place == "MinMaxPost")
                {
                    AddImageFreezeSource(ImageFreezeSources.AsynchronousCalculation);
                    try
                    {
                        ChartMinMax chartMinMax = CreateEmptyChartMinMax();
                        chartMinMax.m_X = new MinMax
                        {
                            m_Min = array[0, 0],
                            m_Max = array[0, 1]
                        };
                        chartMinMax.m_Y = new MinMax
                        {
                            m_Min = array[1, 0],
                            m_Max = array[1, 1]
                        };
                        chartMinMax.m_Y2 = new MinMax
                        {
                            m_Min = array[2, 0],
                            m_Max = array[2, 1]
                        };
                        chartMinMax.m_Z = new MinMax
                        {
                            m_Min = array[3, 0],
                            m_Max = array[3, 1]
                        };
                        ChartObject.setChartMinMax(chartMinMax);
                    }
                    catch (Exception ex)
                    {
                        RemoveImageFreezeSource(ImageFreezeSources.AsynchronousCalculation);
                        if (ex is COMException)
                        {
                            throw new Exception(ComExceptionEngineErrorCode, ex);
                        }
                        throw;
                    }
                }
            }
            else if (DebugTools.DevAssert(uint.TryParse(place, out var result), null).Passed)
            {
                SetTrace(result, matrixValue, resultsTransform);
            }
        }
        finally
        {
            DebugTools.LogData(string.Format("ChartContainerModel.SetMatrix: {0}: End", place), ref refTime);
        }
    }
    public event Func<IResultNode, double[,], StringBuilder, double[,]> OnProcessMinMax;
    public IEnumerable<uint> CurrentTraceIds
    {
        get
        {
            if (_traceIds == null)
            {
                yield break;
            }
            foreach (uint traceId in _traceIds)
            {
                // The Release iterator IL stores the yielded item in a local before setting <>2__current.
                uint currentTraceId = traceId;
                yield return currentTraceId;
            }
        }
        set
        {
            IEnumerable<uint> enumerable = value.IsNotEmpty() ? value : null;
            if (_traceIds == null && enumerable == null)
            {
                return;
            }
            if (_traceIds != null && enumerable != null && _traceIds.SequenceEqual(value))
            {
                return;
            }
            _traceIds = enumerable == null ? null : enumerable.ToArray();
            Dictionary<uint, double[,]> dictionary = null;
            if (_traceIds != null && _dataLimitsDict != null)
            {
                dictionary = new Dictionary<uint, double[,]>();
                uint[] traceIds = _traceIds;
                foreach (uint key in traceIds)
                {
                    if (_dataLimitsDict.TryGetValue(key, out var dataLimits))
                    {
                        dictionary.Add(key, dataLimits);
                    }
                }
            }
            lock (_dataLimitsLock)
            {
                _dataLimitsDict = dictionary;
            }
        }
    }
}
