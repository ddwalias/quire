using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Quire.Calculation.Core;

namespace Quire.Controls.Include;

internal partial class IncludeWorksheetCalculatableHandler
{
    private bool _hideFromCalculationContext;
    private readonly List<IncludedCalculatableWrapper> _allCalculatablesIncludedWrapped;
    private SynchronizationStatus _synchronizationStatus;
    private CalculatingStatus _calculatingStatus;
    private ICalculatableInfo _currentErrorInfo;
    private IList<SystemVariableValuesHandler> _systemVariableValuesHandlers;
    private readonly Action<string> _errorOnChild;
    private readonly Action<string, object> _systemVariableValueChangeHandler;
    public Action<ICalculatable> CalculatableAdded;
    public Action<ICalculatable> CalculatableRemoved;
    public Action CalculatingStatusChanged;
    public Action SynchronizationStatusChanged;

    public IncludeWorksheetCalculatableHandler(Action<string> errorOnChild, Action<string, object> systemVariableValueChangeHandler)
    {
        _allCalculatablesIncludedWrapped = new List<IncludedCalculatableWrapper>();
        _synchronizationStatus = (SynchronizationStatus)3;
        _calculatingStatus = (CalculatingStatus)2;
        _currentErrorInfo = null;
        _systemVariableValuesHandlers = new List<SystemVariableValuesHandler>();
        _errorOnChild = errorOnChild;
        _systemVariableValueChangeHandler = systemVariableValueChangeHandler;
    }

    internal bool ReloadedInFreemium { private get; set; }

    internal bool CanManipulateCalculatable => !ReloadedInFreemium;

    internal bool HideFromCalculationContext
    {
        get => _hideFromCalculationContext;
        set
        {
            if (_hideFromCalculationContext == value)
            {
                return;
            }
            _hideFromCalculationContext = value;
            foreach (SystemVariableValuesHandler systemVariableValuesHandler in _systemVariableValuesHandlers)
            {
                ToggleHiddenState(systemVariableValuesHandler, _hideFromCalculationContext);
            }
            foreach (IncludedCalculatableWrapper item in _allCalculatablesIncludedWrapped)
            {
                ToggleHiddenState(item, _hideFromCalculationContext);
            }
        }
    }

    private void ToggleHiddenState(SystemVariableValuesHandler calc, bool hideFromCalculationContext)
    {
        ICalculationHideableChild calculationHideableChild = calc;
        if (calculationHideableChild != null)
        {
            calculationHideableChild.HideFromCalculationContext = hideFromCalculationContext;
            if (hideFromCalculationContext)
            {
                RemoveSystemVariableValuesHandler(calc);
            }
            else
            {
                AddSystemVariableValuesHandler(calc);
            }
        }
    }

    private void ToggleHiddenState(IncludedCalculatableWrapper calc, bool hideFromCalculationContext)
    {
        if (calc.WrappedCalculatable is ICalculationHideableChild calculationHideableChild)
        {
            calculationHideableChild.HideFromCalculationContext = hideFromCalculationContext;
        }
        if (hideFromCalculationContext)
        {
            RemoveIncludedCalculatableWrapper(calc);
        }
        else
        {
            AddIncludedCalculatableWrapper(calc);
        }
    }

    internal SynchronizationStatus SynchronizationStatus
    {
        get => _synchronizationStatus;
        set
        {
            if (_synchronizationStatus != value)
            {
                _synchronizationStatus = value;
                if (SynchronizationStatusChanged != null)
                {
                    SynchronizationStatusChanged();
                }
            }
        }
    }

    internal CalculatingStatus CalculatingStatus
    {
        get => _calculatingStatus;
        set
        {
            if (_calculatingStatus != value)
            {
                _calculatingStatus = value;
                if (CalculatingStatusChanged != null)
                {
                    CalculatingStatusChanged();
                }
            }
        }
    }

    internal void RemoveIncludedCalculatableWrappers()
    {
        if (!HideFromCalculationContext)
        {
            foreach (IncludedCalculatableWrapper item in _allCalculatablesIncludedWrapped)
            {
                RemoveIncludedCalculatableWrapper(item);
            }
        }
    }

    internal void RemoveAllCalculatables()
    {
        if (!HideFromCalculationContext)
        {
            foreach (SystemVariableValuesHandler systemVariableValuesHandler in _systemVariableValuesHandlers)
            {
                RemoveSystemVariableValuesHandler(systemVariableValuesHandler);
            }
        }
        RemoveIncludedCalculatableWrappers();
    }

    internal void CleanIncludedCalculatableWrappers(bool shouldRemove)
    {
        if (shouldRemove)
        {
            RemoveIncludedCalculatableWrappers();
        }
        _allCalculatablesIncludedWrapped.Clear();
    }

    internal void AddIncludedCalculatableWrappers()
    {
        if (!HideFromCalculationContext)
        {
            foreach (IncludedCalculatableWrapper item in _allCalculatablesIncludedWrapped)
            {
                AddIncludedCalculatableWrapper(item);
            }
        }
    }

    internal void AddSystemVariableValuesHandlers()
    {
        if (!HideFromCalculationContext)
        {
            foreach (SystemVariableValuesHandler systemVariableValuesHandler in _systemVariableValuesHandlers)
            {
                AddSystemVariableValuesHandler(systemVariableValuesHandler);
            }
        }
    }

    internal void CreateAndAddIncludedCalculatableWrappers(IEnumerable<UIElement> regions)
    {
        foreach (ICalculatable item in ExtractCalculatablesIncluded(regions))
        {
            if (item is ICalculatableAggregateWithComponent)
            {
                IncludedCalculatableAggregateWithComponentWrapper includedCalculatableAggregateWithComponentWrapper = item as IncludedCalculatableAggregateWithComponentWrapper;
                if (includedCalculatableAggregateWithComponentWrapper == null)
                {
                    includedCalculatableAggregateWithComponentWrapper = new IncludedCalculatableAggregateWithComponentWrapper(item as ICalculatableAggregateWithComponent);
                }
                else
                {
                    includedCalculatableAggregateWithComponentWrapper.IsInternalCalc = true;
                }
                _allCalculatablesIncludedWrapped.Add(includedCalculatableAggregateWithComponentWrapper);
            }
            else if (item is ICalculatableAggregate)
            {
                IncludedCalculatableAggregateWrapper includedCalculatableAggregateWrapper = item as IncludedCalculatableAggregateWrapper;
                if (includedCalculatableAggregateWrapper == null)
                {
                    includedCalculatableAggregateWrapper = new IncludedCalculatableAggregateWrapper(item as ICalculatableAggregate);
                }
                else
                {
                    includedCalculatableAggregateWrapper.IsInternalCalc = true;
                }
                _allCalculatablesIncludedWrapped.Add(includedCalculatableAggregateWrapper);
            }
            else
            {
                IncludedCalculatableWrapper includedCalculatableWrapper = item as IncludedCalculatableWrapper;
                if (includedCalculatableWrapper == null)
                {
                    includedCalculatableWrapper = new IncludedCalculatableWrapper(item);
                }
                else
                {
                    includedCalculatableWrapper.IsInternalCalc = true;
                }
                _allCalculatablesIncludedWrapped.Add(includedCalculatableWrapper);
            }
        }
        AddIncludedCalculatableWrappers();
    }

    internal IEnumerable<ICalculatable> CalculatableChildren
    {
        get
        {
            if (!HideFromCalculationContext && CanManipulateCalculatable)
            {
                foreach (SystemVariableValuesHandler systemVariableValuesHandler in _systemVariableValuesHandlers)
                {
                    yield return systemVariableValuesHandler;
                }
                foreach (IncludedCalculatableWrapper item in _allCalculatablesIncludedWrapped)
                {
                    yield return item;
                }
            }
        }
    }

    internal ICalculatableInfo CurrentErrorInfo { get; set; }

    internal bool ContainsError(EngineError error)
    {
        foreach (IncludedCalculatableWrapper item in _allCalculatablesIncludedWrapped)
        {
            if (item != null && item.ContainsError(error))
            {
                CurrentErrorInfo = item.CalculatableInfo;
                return true;
            }
        }
        return false;
    }

    internal void HookupSystemVariables()
    {
        _systemVariableValuesHandlers = new List<SystemVariableValuesHandler>
        {
            new SystemVariableValuesHandler("mc_TOL"),
            new SystemVariableValuesHandler("mc_CTOL"),
            new SystemVariableValuesHandler("mc_ORIGIN"),
            new SystemVariableValuesHandler("mc_CWD")
        };
        if (!HideFromCalculationContext)
        {
            foreach (SystemVariableValuesHandler systemVariableValuesHandler in _systemVariableValuesHandlers)
            {
                AddSystemVariableValuesHandler(systemVariableValuesHandler);
            }
        }
    }

    internal void UnHookSystemVariables()
    {
        if (!HideFromCalculationContext)
        {
            foreach (SystemVariableValuesHandler systemVariableValuesHandler in _systemVariableValuesHandlers)
            {
                RemoveSystemVariableValuesHandler(systemVariableValuesHandler);
            }
        }
        _systemVariableValuesHandlers.Clear();
    }

    internal void Commit()
    {
        if (!HideFromCalculationContext && CanManipulateCalculatable)
        {
            foreach (ICalculatable calculatableChild in CalculatableChildren)
            {
                calculatableChild.Commit();
            }
        }
    }

    private void AddIncludedCalculatableWrapper(IncludedCalculatableWrapper calc)
    {
        if (CanManipulateCalculatable)
        {
            FireCalculatableAdded(calc);
            calc.Commit();
            calc.SynchronizationStatusChangedEvent += HandleSynchronizationStatusChanged;
            calc.CalculatingStatusChangedEvent += HandleCalculatingStatusChanged;
            if (!calc.IsInternalCalc)
            {
                calc.SetErrorChanged += HandleSetErrorChanged;
            }
        }
    }

    private void RemoveIncludedCalculatableWrapper(IncludedCalculatableWrapper calc)
    {
        if (CanManipulateCalculatable)
        {
            calc.SynchronizationStatusChangedEvent -= HandleSynchronizationStatusChanged;
            calc.CalculatingStatusChangedEvent -= HandleCalculatingStatusChanged;
            if (!calc.IsInternalCalc)
            {
                calc.SetErrorChanged -= HandleSetErrorChanged;
            }
            FireCalculatableRemoved(calc);
        }
    }

    private void AddSystemVariableValuesHandler(SystemVariableValuesHandler item)
    {
        if (CanManipulateCalculatable)
        {
            FireCalculatableAdded(item);
            item.SystemVariableValueChanged += _systemVariableValueChangeHandler;
        }
    }

    private void RemoveSystemVariableValuesHandler(SystemVariableValuesHandler item)
    {
        if (CanManipulateCalculatable)
        {
            FireCalculatableRemoved(item);
            item.SystemVariableValueChanged -= _systemVariableValueChangeHandler;
        }
    }

    private void FireCalculatableAdded(ICalculatable calc)
    {
        if (CalculatableAdded != null)
        {
            CalculatableAdded(calc);
        }
    }

    private void FireCalculatableRemoved(ICalculatable calc)
    {
        if (CalculatableRemoved != null)
        {
            CalculatableRemoved(calc);
        }
    }

    private string UpdateCurrentErrorInfo()
    {
        ICalculatableInfo calculatableInfo = null;
        foreach (IncludedCalculatableWrapper item in _allCalculatablesIncludedWrapped)
        {
            ICalculatableInfo calculatableInfo2 = item.CalculatableInfo;
            if (calculatableInfo2 != null && calculatableInfo2.Error != null)
            {
                calculatableInfo = calculatableInfo2;
                if (calculatableInfo.ErrorCode == "premium%_only")
                {
                    break;
                }
            }
        }
        CurrentErrorInfo = calculatableInfo;
        if (CurrentErrorInfo == null)
        {
            return "CalculationSucceededErrorCode";
        }
        if (!(CurrentErrorInfo.ErrorCode == "premium%_only"))
        {
            return "IncludeControlCalculationErrors";
        }
        return "IncludeControlCalculationFreemiumErrors";
    }

    private void HandleSetErrorChanged(EngineError arg)
    {
        _errorOnChild(UpdateCurrentErrorInfo());
    }

    private void HandleCalculatingStatusChanged()
    {
        if (_allCalculatablesIncludedWrapped.Select((IncludedCalculatableWrapper wrapper) => wrapper.WrappedCalculatable as ICalculatableInfo).Any((ICalculatableInfo calculatableInfo) => calculatableInfo != null && calculatableInfo.CalculatingStatus == (CalculatingStatus)1))
        {
            CalculatingStatus = (CalculatingStatus)1;
        }
        else
        {
            CalculatingStatus = (CalculatingStatus)2;
        }
    }

    private void HandleSynchronizationStatusChanged()
    {
        if (_allCalculatablesIncludedWrapped.Select((IncludedCalculatableWrapper wrapper) => wrapper.WrappedCalculatable as ICalculatableInfo).Any((ICalculatableInfo calculatableInfo) => calculatableInfo != null && calculatableInfo.SynchronizationStatus == SynchronizationStatus.Pending))
        {
            SynchronizationStatus = SynchronizationStatus.Pending;
        }
        else
        {
            SynchronizationStatus = (SynchronizationStatus)3;
        }
    }

    private static IEnumerable<ICalculatable> ExtractCalculatablesIncluded(IEnumerable<UIElement> regions)
    {
        if (regions != null)
        {
            foreach (UIElement region in regions)
            {
                ICalculatable calculatable = region as ICalculatable;
                ICalculatableContainer calculatableContainer = region as ICalculatableContainer;
                if (calculatable != null)
                {
                    yield return calculatable;
                }
                else
                {
                    if (calculatableContainer == null)
                    {
                        continue;
                    }
                    foreach (ICalculatable calculatableChild in calculatableContainer.CalculatableChildren)
                    {
                        yield return calculatableChild;
                    }
                }
            }
        }
    }
}
