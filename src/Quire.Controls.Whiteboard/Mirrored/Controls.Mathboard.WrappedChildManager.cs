using System;
using System.Collections.Generic;
using Quire.Calculation.Core;
using Quire.Calculation.Core.Support;

namespace Quire.Controls.Mathboard;

public partial class WrappedChildManager : CalculatableChildHider
{
    protected readonly Func<ICalculatable, bool, ICalculatable> _createCalculatableWrapper;
    private readonly Func<ICalculatable, ICalculatable> _lookupCalculatableWrapper;
    private readonly Action<ICalculatable> _removeCalculatableWrapper;
    private readonly Action<ICalculatable, bool> _notifyHiddenStateChanged;
    private readonly Action _notifyContainerChildrenChanged;

    public WrappedChildManager(
        IEnumerable<object> getChildren,
        Action<ICalculatable> fireCalculatableAdded,
        Action<ICalculatable> fireCalculatableRemoved,
        Action<ICalculatable> fireCalculatableMoved,
        Func<ICalculatable, bool, ICalculatable> createCalculatableWrapper,
        Func<ICalculatable, ICalculatable> lookupCalculatableWrapper,
        Action<ICalculatable> removeCalculatableWrapper,
        Action<ICalculatable, bool> notifyHiddenStateChanged,
        Action notifyContainerChildrenChanged)
    {
        _createCalculatableWrapper = createCalculatableWrapper;
        _lookupCalculatableWrapper = lookupCalculatableWrapper;
        _removeCalculatableWrapper = removeCalculatableWrapper;
        _notifyHiddenStateChanged = notifyHiddenStateChanged;
        _notifyContainerChildrenChanged = notifyContainerChildrenChanged;
        Initialize(EffectiveChildren(getChildren), fireCalculatableAdded, fireCalculatableRemoved, fireCalculatableMoved, adoptCalculatableContainerChildren: true);
    }

    private IEnumerable<object> EffectiveChildren(IEnumerable<object> realChildren)
    {
        foreach (object realChild in realChildren)
        {
            if (realChild is ICalculatable arg)
            {
                ICalculatable calculatable = _lookupCalculatableWrapper(arg);
                if (calculatable != null)
                {
                    yield return calculatable;
                }
            }
            else
            {
                yield return realChild;
            }
        }
    }

    protected void ChildAddedCore(object child)
    {
        base.ChildAdded(child);
    }

    public override void ChildAdded(object child)
    {
        object effectiveChild = child;
        if (child is ICalculatable calculatable)
        {
            effectiveChild = _createCalculatableWrapper(calculatable, false);
        }
        if (child is ICalculatableContainer calculatableContainer && !(calculatableContainer is ICalculatableAggregate))
        {
            calculatableContainer.CalculatableChildren.Each(cntrChild =>
            {
                _createCalculatableWrapper(cntrChild, true);
            });
        }
        base.ChildAdded(effectiveChild);
    }

    protected void ChildRemovedCore(object child)
    {
        base.ChildRemoved(child);
    }

    public override void ChildRemoved(object child)
    {
        object effectiveChild = child;
        ICalculatable calculatable = child as ICalculatable;
        if (calculatable != null)
        {
            effectiveChild = _lookupCalculatableWrapper(calculatable);
        }
        base.ChildRemoved(effectiveChild);
        if (calculatable != null)
        {
            _removeCalculatableWrapper(calculatable);
        }
        if (child is ICalculatableContainer calculatableContainer && !(calculatableContainer is ICalculatableAggregate))
        {
            calculatableContainer.CalculatableChildren.Each(cntrChild =>
            {
                _removeCalculatableWrapper(cntrChild);
            });
        }
    }

    public override void ChildMoved(object child)
    {
        if (child is ICalculatable arg)
        {
            ICalculatable calculatable = _lookupCalculatableWrapper(arg);
            if (calculatable is not ICalculationHideableChild hideable || !hideable.HideFromCalculationContext)
            {
                _fireCalculatableMoved(calculatable);
            }
        }
        if (child is ICalculatableContainer calculatableContainer && !(calculatableContainer is ICalculatableAggregate))
        {
            calculatableContainer.CalculatableChildren.Each(cntrChild =>
            {
                _fireCalculatableMoved(_lookupCalculatableWrapper(cntrChild));
            });
        }
    }

    protected override void HandleCalculatableMovedInChildContainer(ICalculatable calc)
    {
        base.HandleCalculatableMovedInChildContainer(_lookupCalculatableWrapper(calc));
    }

    protected override void HandleCalculatableAddedToChildContainer(ICalculatableContainer container, ICalculatable calc)
    {
        ICalculatable wrapper = _createCalculatableWrapper(calc, true);
        base.HandleCalculatableAddedToChildContainer(container, wrapper);
        _notifyContainerChildrenChanged();
    }

    protected override void HandleCalculatableRemovedFromChildContainer(ICalculatable calc)
    {
        ICalculatable wrapper = _lookupCalculatableWrapper(calc);
        base.HandleCalculatableRemovedFromChildContainer(wrapper);
        _removeCalculatableWrapper(calc);
        _notifyContainerChildrenChanged();
    }

    protected override IEnumerable<ICalculatable> GetContainerChildren(ICalculatableContainer container)
    {
        foreach (ICalculatable calculatableChild in container.CalculatableChildren)
        {
            ICalculatable calculatable = _lookupCalculatableWrapper(calculatableChild);
            if (calculatable != null)
            {
                yield return calculatable;
            }
        }
    }

    protected override void HiddenStateChanged(ICalculatable calc, bool hideFromCalculationContext)
    {
        base.HiddenStateChanged(calc, hideFromCalculationContext);
        _notifyHiddenStateChanged(calc, hideFromCalculationContext);
    }
}
