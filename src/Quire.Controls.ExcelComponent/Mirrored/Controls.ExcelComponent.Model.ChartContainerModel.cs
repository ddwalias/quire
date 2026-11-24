using System;
using System.Windows;
using System.Runtime.InteropServices;
using Quire.Serialization;
using Quire.Calculation;
using Quire.FunctionalitiesLimitation;
using Quire.Automation;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Quire.Calculation.Core;
using Quire.Controls.Core;
using Quire.ChartAppLib;
using Quire.PersistentData;
using Quire.Core;
using Quire.OLE;
namespace Quire.Controls.ExcelComponent.Model;

public partial class ChartContainerModel
{
    public override string ProgId => "PTC.ChartAppOLE";

    public override bool AsyncUpdateVisibleSize => true;

    protected override string[] ExcludeElementsNames => _excludeElementsNames;

    protected override EnumFunctionalityKey ToggleActivateStateFunctionalityKey => (EnumFunctionalityKey)800;

    public bool IsTypeDefault => true;

    public global::Quire.Calculation.Core.Trace2DResults.TraceType Type => default;

    public override bool ShouldCommitAtLeastOnce => false;

    public override bool ShouldCommitIfDirty => true;

    protected override bool CheckOleActiveState => false;

    public bool IsImageFrozen => ImageFreezeSources != ImageFreezeSources.None;

    public override void Commit()
    {
        AssertDispatcherAccess();
        RemoveImageFreezeSource(ImageFreezeSources.AsynchronousCalculation);
    }

    public override global::System.Collections.Generic.IList<global::Quire.Serialization.IRegionPersistentData> SetRegionPersistentData(global::Quire.Serialization.IDeS11NProvider d11NProvider, global::Quire.PersistentData.chartContainerType chartContainer)
    {
        Unpack(d11NProvider, chartContainer.itemidref);
        return null;
    }

    public override IValue GetValue(string place)
    {
        throw new NotImplementedException();
    }

    private void SetString(string place, IValue value)
    {
        throw new AddressErrorException(place);
    }

    public void AddImageFreezeSource(ImageFreezeSources source, bool forceRun = false)
    {
        if (forceRun)
        {
            Run();
        }
        AddOrRemoveImageFreezeSource(source, add: true);
    }

    public void RemoveImageFreezeSource(ImageFreezeSources source)
    {
        AddOrRemoveImageFreezeSource(source, add: false);
    }

    private string _helpServerPath;

    private uint[] _traceIds;

    private Dictionary<uint, double[,]> _dataLimitsDict;

    public override Dictionary<Guid, Guid> GuidsConversionDict => OleUtils.ChartAppOleGuidsConversionDict;

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

    public double[,] DataLimits
    {
        get
        {
            if (_dataLimitsDict.IsNullOrEmpty())
            {
                return null;
            }
            double[,] array = new double[4, 2];
            double[,] array2 = _dataLimitsDict.Values.First();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    array[i, j] = array2[i, j];
                }
            }
            foreach (double[,] item in _dataLimitsDict.Values.Skip(1))
            {
                for (int k = 0; k < 4; k++)
                {
                    array[k, 0] = Math.Min(array[k, 0], item[k, 0]);
                    array[k, 1] = Math.Max(array[k, 1], item[k, 1]);
                }
            }
            return array;
        }
    }

    private ImageFreezeSources ImageFreezeSources { get; set; }

    internal ChartAppOLE ChartObject => (ChartAppOLE)GetOleObject(forceRun: true);

    public bool IsImageReady
    {
        get
        {
            try
            {
                bool isOleSiteManagerRunning = IsOleSiteManagerRunning;
                bool flag = !isOleSiteManagerRunning || ChartObject.isImageReady() != 0;
                if (flag)
                {
                    DebugTools.LogData(string.Format("ChartContainerModel.IsImageReady: IsOleSiteManagerRunning: {0}, returning {1}", isOleSiteManagerRunning, flag));
                }
                return flag;
            }
            catch (Exception ex)
            {
                DebugTools.LogData(string.Format("ChartContainerModel.IsImageReady: Exception {0}, returning true", ex.Message));
                DebugTools.FailAssertion(ex);
                return true;
            }
        }
    }

    public event Func<IResultNode, double[,], StringBuilder, double[,]> OnProcessMinMax;

    protected override void OnStartRunning()
    {
        base.OnStartRunning();
        if (IsImageFrozen)
        {
            ApplyImageFrozenState();
        }
        SetHelpServerPathIfRunning();
    }

    protected override void DoDeactivate()
    {
        ChartContainerModelUtils.WaitForChartImagesReady(new ChartContainerModel[1] { this }, null);
        base.DoDeactivate();
    }

    public override chartContainerType GetRegionPersistentData(IS11NProvider s11NProvider, string uniqueId, Action onServerBusy)
    {
        return new chartContainerType(SizeData)
        {
            itemidref = Pack(s11NProvider, uniqueId, ExternalObjectType.Chart, onServerBusy)
        };
    }

    public override void SetValue(string place, IValue value, IValueToIResultNodeTransform resultsTransform)
    {
        CheckInstalledAndAvailable(forceRun: true);
        string type = value.Type;
        if (!(type == "String"))
        {
            if (!(type == "Matrix"))
            {
                throw new Exception($"Unsupported type {value.Type}");
            }
            SetMatrix(place, value, resultsTransform);
        }
        else
        {
            SetString(place, value);
        }
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

    public void ForceUnfreezeImage(bool force)
    {
        if (!IsOleSiteManagerRunning)
        {
            return;
        }
        ApplyImageFrozenState(force ? new bool?(false) : (bool?)null);
    }

    public override bool ShouldSkipUnchangedValueMultiThreaded(string place)
    {
        if (uint.TryParse(place, out var result))
        {
            lock (_dataLimitsLock)
            {
                return _dataLimitsDict.IsNotEmpty() && _dataLimitsDict.ContainsKey(result);
            }
        }
        return false;
    }

    private static ChartMinMax CreateEmptyChartMinMax()
    {
        return new ChartMinMax
        {
            m_X = new MinMax
            {
                m_Min = double.NaN,
                m_Max = double.NaN
            },
            m_Y = new MinMax
            {
                m_Min = double.NaN,
                m_Max = double.NaN
            },
            m_Y2 = new MinMax
            {
                m_Min = double.NaN,
                m_Max = double.NaN
            },
            m_Z = new MinMax
            {
                m_Min = double.NaN,
                m_Max = double.NaN
            }
        };
    }

    private void SetMatrix(string place, IValue value, IValueToIResultNodeTransform resultsTransform)
    {
        DebugTools.LogData(string.Format("ChartContainerModel.SetMatrix: {0}: Start", place));
        try
        {
            IMatrixValue value2 = (IMatrixValue)value;
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
                if (!(place == "MinMaxPost"))
                {
                    return;
                }
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
                    ChartAppOLE chartObject = ChartObject;
                    ChartMinMax data = chartMinMax;
                    chartObject.setChartMinMax(ref data);
                    return;
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
            if (DebugTools.DevAssert(uint.TryParse(place, out var result), null).Passed)
            {
                SetTrace(result, value2, resultsTransform);
            }
        }
        finally
        {
        }
    }

    private void AddOrRemoveImageFreezeSource(ImageFreezeSources source, bool add)
    {
        bool isImageFrozen = IsImageFrozen;
        if (add)
        {
            if (source != ImageFreezeSources.AsynchronousCalculation && ImageFreezeSources.HasFlag(source & ~ImageFreezeSources.AsynchronousCalculation))
            {
                DebugTools.FailAssertionDoNotThrow(source.ToString() + " => " + ImageFreezeSources);
            }
            ImageFreezeSources |= source;
        }
        else
        {
            ImageFreezeSources &= ~source;
        }
        bool isImageFrozen2 = IsImageFrozen;
        if (isImageFrozen2 == isImageFrozen)
        {
            DebugTools.LogData(string.Format("ChartContainerModel.AddOrRemoveImageFreezeSource: {0}: Skip because no change", isImageFrozen2));
        }
        else if (!IsOleSiteManagerRunning)
        {
            DebugTools.LogData(string.Format("ChartContainerModel.AddOrRemoveImageFreezeSource: {0}: Skip because not running", isImageFrozen2));
        }
        else
        {
            ApplyImageFrozenState();
        }
    }

    private void ApplyImageFrozenState(bool? frozenOverride = null)
    {
        bool shouldFreeze = frozenOverride ?? IsImageFrozen;
        DebugTools.LogData(string.Format("ChartContainerModel.ApplyImageFrozenState: {0}: Start", shouldFreeze));
        try
        {
            if (shouldFreeze)
            {
                ChartObject.setFreezeImage();
            }
            else
            {
                ChartObject.setUnFreezeImage();
            }
        }
        catch (Exception ex)
        {
            DebugTools.FailAssertion(ex);
        }
        finally
        {
            // Debug.Assert is [Conditional("DEBUG")]: the call keeps the empty finally region in Release IL.
            System.Diagnostics.Debug.Assert(true);
        }
    }

    private void SetHelpServerPathIfRunning()
    {
        if (IsOleSiteManagerRunning && HelpServerPath.IsNotEmpty())
        {
            string text = HelpServerPath;
            if (text.Contains(Urls.DefaultUrlParam))
            {
                text = text.Substring(0, HelpServerPath.LastIndexOf(Urls.DefaultUrlParam, StringComparison.Ordinal));
            }
            ChartObject.setHelpPath(text);
        }
    }

    public void SetRect(Int32Rect rectInPixels)
    {
        try
        {
            DebugTools.LogData("ChartContainer.ApplyRect: Start: " + ((Int32Rect)rectInPixels).ToString());
            ChartObject.setRect(rectInPixels.Width, rectInPixels.Height);
            DebugTools.LogData("ChartContainer.ApplyRect: End: " + ((Int32Rect)rectInPixels).ToString());
        }
        catch (Exception ex)
        {
            DebugTools.LogData("ChartContainer.ApplyRect: Failed: " + ex.Message);
            DebugTools.FailAssertion(ex);
        }
    }
}
