using System;
using System.Windows;
using Quire.Serialization;
using Quire.Calculation;
using Quire.FunctionalitiesLimitation;

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
}
