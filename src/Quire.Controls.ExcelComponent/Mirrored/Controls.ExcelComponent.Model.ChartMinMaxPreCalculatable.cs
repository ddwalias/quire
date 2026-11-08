using System;
using System.Collections.Generic;
using Quire.Calculation.Core;

namespace Quire.Controls.ExcelComponent.Model;

internal partial class ChartMinMaxPreCalculatable
{
    private ChartMinMaxCalculatable _calc;

    public string Rl
    {
        get
        {
            if (DebugTools.AssertNotNull(_calc, null).Failed)
            {
                return "-PLACEHOLDER";
            }
            return _calc.GetRl("MinMaxPre");
        }
    }

    public global::Quire.Pair<bool, bool> IsFragment
    {
        get
        {
            global::Quire.Pair<bool, bool> pair = new global::Quire.Pair<bool, bool>
            {
                First = true,
                Second = true
            };
            DebugTools.DevAssert(_calc != null && _calc.IsFragment == pair, null);
            return pair;
        }
    }

    public bool IsConnectedToTheEngine
    {
        get
        {
            if (DebugTools.AssertNotNull(_calc, null).Failed)
            {
                return false;
            }
            return _calc.IsConnectedToTheEngine;
        }
        set
        {
            DebugTools.FailAssertion(value.ToString());
        }
    }

    public EngineError Error
    {
        get
        {
            DebugTools.DevAssert(_calc != null && _calc.Error == null, null);
            return null;
        }
    }

    public event Action<ICalculatable> RequestEvaluation;

    public event Func<ICalculatable, string, IResultNode> GetValueOfIdentifierBefore;

    public event Action<ICalculatable> TreatAsUpToDate;

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

    // Keep this callback-producing member after the event and two methods to preserve Mathcad's closure ordinal.
    public void SetChartMinMaxCalculatable(ChartMinMaxCalculatable calc)
    {
        _calc = calc;
        _calc.RequestEvaluation += delegate
        {
            Commit();
        };
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

    public ChartMinMaxPreCalculatable()
    {
    }

    // These Quire interface shims follow the Mathcad members to preserve the captured callback ordinal.
    void ICalculatable.SetResult(global::Quire.Calculation.Core.CalculationResult result) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    global::Quire.Calculation.Core.SynchronizationStatus ICalculatable.SynchronizationStatus => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    event EventHandler ICalculatable.RlChanged
    {
        add { }
        remove { }
    }
}
