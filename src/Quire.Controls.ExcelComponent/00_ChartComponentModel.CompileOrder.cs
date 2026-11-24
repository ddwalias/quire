using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Quire.Calculation.Core;
using Quire.Controls.Core;
using Quire.Controls.ExcelComponent.View;

namespace Quire.Controls.ExcelComponent.Model;

// Compile-order device: all ChartComponentModel members live in this file, in Mathcad member order, so each
// closure is numbered as in Release IL (lambda ordinals count the members before its owner).
public partial class ChartComponentModel
{
    private readonly Dictionary<ChartWrapperType, SimpleChartCalculatable> _defaultLimits;

    private readonly ChartMinMaxCalculatable _chartMinMaxCalc;

    private readonly ChartMinMaxPreCalculatable _chartMinMaxPreCalc;

    private readonly SortedDictionary<uint, ChartTraceCalculatable> _traceCalculatables;

    [global::Quire.Mirror.MirrorStub, global::Quire.Mirror.MirrorBlocked("ChartComponentModel constructor order")]
    public ChartComponentModel() { }

    internal void CommitPre()
    {
        ConnectedDefaultLimits.Each(limit => limit.Commit());
    }

    internal void CommitPost()
    {
        _chartMinMaxCalc.Commit();
    }

    public override void Commit()
    {
        CalculatablesConnectedToTheEngine.Each(calc => calc.Commit());
        base.Commit();
    }

        public override string Rl
        {
            get
            {
                ComponentNodeHandle = 1u;
                return string.Format("(eval (named-component:0x{0:x8} {1} {2} %@ 0))", ComponentNodeHandle, ChartTraceCalculatable.ChartVariableWithLabel, ComponentId);
            }
        }

        protected override void FireCalculatableAdded(ICalculatable calculatable)
        {
            IChartComponentCalculatableWrapper chartComponentCalculatableWrapper = calculatable as IChartComponentCalculatableWrapper;
            if (DebugTools.AssertNotNull(chartComponentCalculatableWrapper, null).Failed)
            {
                return;
            }
            if (DebugTools.DevAssert(chartComponentCalculatableWrapper.ChartWrapperType.IsUnknownType(), null).Failed)
            {
                chartComponentCalculatableWrapper.ResetChartWrapperType();
            }
        }

        protected override void FireCalculatableRemoved(ICalculatable calculatable)
        {
            IChartComponentCalculatableWrapper chartComponentCalculatableWrapper = calculatable as IChartComponentCalculatableWrapper;
            if (DebugTools.AssertNotNull(chartComponentCalculatableWrapper, null).Failed)
            {
                return;
            }
            if (!IsDisposed)
            {
                ChartWrapperType value = chartComponentCalculatableWrapper.ResetChartWrapperType();
                HandleChildWrapperRemovedOrChartTypeChanged(chartComponentCalculatableWrapper, value);
            }
        }

        protected override void FireCalculatableMoved(ICalculatable calculatable)
        {
            IChartComponentCalculatableWrapper chartComponentCalculatableWrapper = calculatable as IChartComponentCalculatableWrapper;
            if (DebugTools.AssertNotNull(chartComponentCalculatableWrapper, null).Failed)
            {
                return;
            }
            if (chartComponentCalculatableWrapper.IsLimitExpression())
            {
                UpdateChartMinMaxCalculatable();
            }
            if (chartComponentCalculatableWrapper.IsTraceOrCommonExpression())
            {
                UpdateAllTraces(null);
            }
            base.FireCalculatableMoved(calculatable);
        }

    protected override IEnumerable<string> ComponentErrors { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

        protected override void SetErrorByNodeHandle(EngineError error)
        {
            DebugTools.FailAssertion(error.NodeHandle.ToString());
            SetErrorFallback(error);
        }

        protected override void SetChildError(EngineError error)
        {
            if (error != null && error.ErrorSource is IChartMinMaxCommon)
            {
                _chartMinMaxCalc.ProcessComponentError(error);
                error = null;
            }
            base.SetChildError(error);
        }

        protected override void UpdateCustomErrorBasedOnPlacement(IWhiteboardItem item, bool placedAfterOleContainer)
        {
            ICalculatable calculatable = item as ICalculatable;
            if (DebugTools.AssertNotNull(calculatable, null).Failed)
            {
                return;
            }
            DebugTools.DevAssert(!GetWrapper(calculatable).IsOutputEquation && !placedAfterOleContainer, null);
        }

    protected override IEnumerable<ICalculatable> CalculatablesConnectedToTheEngine
    {
        get
        {
            foreach (SimpleChartCalculatable connectedDefaultLimit in ConnectedDefaultLimits)
            {
                yield return connectedDefaultLimit;
            }
            foreach (ICalculatable item in from item in WhiteboardItems.OfType<ICalculatable>()
                where GetWrapper(item).IsConnectedToTheEngine
                select item)
            {
                yield return item;
            }
            yield return _chartMinMaxPreCalc;
            foreach (ChartTraceCalculatable value in _traceCalculatables.Values)
            {
                yield return value;
            }
            yield return _chartMinMaxCalc;
        }
    }

        protected override ICalculatable CreateCalculatableWrapper(ICalculatable calc, bool arg2)
        {
            if (calc is IChartCalculatable)
            {
                DebugTools.FailAssertion(null, Array.Empty<object>());
                return calc;
            }
            if (CalculatableWrapperMap.ContainsKey(calc))
            {
                DebugTools.FailAssertion(null, Array.Empty<object>());
                return CalculatableWrapperMap[calc];
            }
            ChartComponentCalculatableWrapper chartComponentCalculatableWrapper = new ChartComponentCalculatableWrapper(calc);
            CalculatableWrapperMap[calc] = chartComponentCalculatableWrapper;
            chartComponentCalculatableWrapper.ChartTypeChanged += HandleChildWrapperChartTypeChanged;
            chartComponentCalculatableWrapper.ErrorChanged += HandleChildWrapperErrorChanged;
            return chartComponentCalculatableWrapper;
        }

        protected override ICalculatable LookupCalculatableWrapper(ICalculatable calc)
        {
            if (calc is IChartCalculatable)
            {
                return calc;
            }
            return base.LookupCalculatableWrapper(calc);
        }

        protected override void RemoveCalculatableWrapper(ICalculatable calc)
        {
            if (calc is IChartCalculatable)
            {
                DebugTools.FailAssertion(null, Array.Empty<object>());
                return;
            }
            IChartComponentCalculatableWrapper chartComponentCalculatableWrapper = GetWrapper(calc) as IChartComponentCalculatableWrapper;
            if (DebugTools.AssertNotNull(chartComponentCalculatableWrapper, null).Passed)
            {
                chartComponentCalculatableWrapper.ChartTypeChanged -= HandleChildWrapperChartTypeChanged;
                chartComponentCalculatableWrapper.ErrorChanged -= HandleChildWrapperErrorChanged;
                chartComponentCalculatableWrapper.Dispose();
            }
            CalculatableWrapperMap.Remove(calc);
            UpdateErrorAccordingToChildren();
        }

    internal override IOleContainerItem OleContainerItem
    {
        get
        {
            return base.OleContainerItem;
        }
        set
        {
            DebugTools.DevAssert(base.OleContainerItem == null && value != null, null);
            base.OleContainerItem = value;
            if (ChartContainerItem != null)
            {
                ChartContainerItem.PublicModel.CurrentTraceIds = _traceCalculatables.Keys;
                ChartContainerItem.PublicModel.OnProcessMinMax += (IResultNode result, double[,] dataLimits, StringBuilder errorBuilder) => _chartMinMaxCalc.ProcessResult(result, dataLimits, errorBuilder);
            }
        }
    }

        public override object Clone()
        {
            ChartComponentModel chartComponentModel = new ChartComponentModel();
            CopyStateTo(chartComponentModel);
            return chartComponentModel;
        }

    private IEnumerable<SimpleChartCalculatable> ConnectedDefaultLimits => _defaultLimits.Values.Where(calc => calc.IsConnectedToTheEngine);

        private ChartContainer ChartContainerItem => (ChartContainer)OleContainerItem;

    internal event Action RequestUpdateIdentifierLabels;

        private void HandleChildWrapperChartTypeChanged(IChartComponentCalculatableWrapper wrapper)
        {
            HandleChildWrapperRemovedOrChartTypeChanged(wrapper, null);
        }

        private void HandleChildWrapperRemovedOrChartTypeChanged(IChartComponentCalculatableWrapper wrapper, ChartWrapperType? prevTypeWhenRemoving)
        {
            ChartWrapperType type = prevTypeWhenRemoving ?? wrapper.PreviousChartType;
            ChartWrapperType chartWrapperType = wrapper.ChartWrapperType;
            if (chartWrapperType.IsUnknownType() && !prevTypeWhenRemoving.HasValue)
            {
                return;
            }
            DebugTools.DevAssert(!prevTypeWhenRemoving.HasValue || (wrapper.ChartWrapperType == ChartWrapperType.Unknown && wrapper.PreviousChartType == ChartWrapperType.Unknown), null);
            if (type.IsLimitType() || chartWrapperType.IsLimitType())
            {
                if (wrapper.IsConnectedToTheEngine && !chartWrapperType.IsLimitType())
                {
                    base.FireCalculatableRemoved(wrapper);
                }
                UpdateChartMinMaxCalculatable();
            }
            if (type.IsTraceOrCommonType() || chartWrapperType.IsTraceOrCommonType())
            {
                UpdateAllTraces(wrapper);
            }
            if (RequestUpdateIdentifierLabels != null)
            {
                RequestUpdateIdentifierLabels();
            }
        }

    private void UpdateAllTraces(IChartComponentCalculatableWrapper wrapperToCommit) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void UpdateChartMinMaxCalculatable() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

        private void InternalAddCalculatableAndEvaluate(IOleBaseComponentCalculatableChild calc)
        {
            base.FireCalculatableAdded(calc);
            if (!(calc is IChartComponentCalculatableWrapper chartComponentCalculatableWrapper))
            {
                calc.Commit();
            }
            else
            {
                chartComponentCalculatableWrapper.HandleRequestEvaluationAux();
            }
        }
}
