#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Quire.Mirror;

namespace Quire.Calculation.Core.Support;

public partial class CalculatableChildHider
{
    private IEnumerable<object> _containerChildren;

    private Action<ICalculatable> _fireCalculatableAdded;

    private Action<ICalculatable> _fireCalculatableRemoved;

    protected Action<ICalculatable> _fireCalculatableMoved;

    private bool _adoptCalculatableContainerChildren;

    [MirrorVerified("PtcCalculationCoreSupportCalculatableChildHiderMechanicalDifferentialTests")]
    public CalculatableChildHider(IEnumerable<object> getChildren, Action<ICalculatable> fireCalculatableAdded, Action<ICalculatable> fireCalculatableRemoved, Action<ICalculatable> fireCalculatableMoved, bool adoptCalculatableContainerChildren)
    {
        Initialize(getChildren, fireCalculatableAdded, fireCalculatableRemoved, fireCalculatableMoved, adoptCalculatableContainerChildren);
    }

    [MirrorVerified("PtcCalculationCoreSupportCalculatableChildHiderMechanicalDifferentialTests")]
    protected CalculatableChildHider()
    {
    }

    [MirrorVerified("PtcCalculationCoreSupportCalculatableChildHiderMechanicalDifferentialTests")]
    protected void Initialize(IEnumerable<object> getChildren, Action<ICalculatable> fireCalculatableAdded, Action<ICalculatable> fireCalculatableRemoved, Action<ICalculatable> fireCalculatableMoved, bool adoptCalculatableContainerChildren)
    {
        _containerChildren = getChildren;
        _fireCalculatableAdded = fireCalculatableAdded;
        _fireCalculatableRemoved = fireCalculatableRemoved;
        _fireCalculatableMoved = fireCalculatableMoved;
        _adoptCalculatableContainerChildren = adoptCalculatableContainerChildren;
        _containerChildren.Each((object child) =>
        {
            SubscribeToPropertyChangedEvents(child);
        });
        _containerChildren.OfType<ICalculatableContainer>().Each((ICalculatableContainer cntr) =>
        {
            SubscribeToCalculatableContainerEvents(cntr);
        });
    }

    [MirrorVerified("PtcCalculationCoreSupportCalculatableChildHiderMechanicalDifferentialTests")]
    public static bool IsHiddenFromCalculation(object child)
    {
        if (child is ICalculationHideableChild calculationHideableChild)
        {
            return calculationHideableChild.HideFromCalculationContext;
        }
        return false;
    }

    [MirrorVerified("PtcCalculationCoreSupportCalculatableChildHiderMechanicalDifferentialTests")]
    public virtual IEnumerable<ICalculatable> CalculatableChildren
    {
        get
        {
            foreach (object child in _containerChildren)
            {
                if (IsHiddenFromCalculation(child))
                {
                    continue;
                }
                ICalculatable calculatable = child as ICalculatable;
                ICalculatableContainer cntr = child as ICalculatableContainer;
                if (calculatable != null)
                {
                    yield return calculatable;
                }
                if (cntr != null && !(child is ICalculatableAggregate))
                {
                    foreach (ICalculatable containerChild in GetContainerChildren(cntr))
                    {
                        yield return containerChild;
                    }
                }
            }
        }
    }

    [MirrorVerified("PtcCalculationCoreSupportCalculatableChildHiderMechanicalDifferentialTests")]
    public virtual void ChildAdded(object child)
    {
        SubscribeToPropertyChangedEvents(child);
        if (child is ICalculatable obj)
        {
            if (!IsHiddenFromCalculation(child))
            {
                _fireCalculatableAdded(obj);
            }
            return;
        }
        ICalculatableContainer calculatableContainer = child as ICalculatableContainer;
        if (_adoptCalculatableContainerChildren && calculatableContainer != null && !(calculatableContainer is ICalculatableAggregate))
        {
            GetContainerChildren(calculatableContainer).Each((ICalculatable childCalc) =>
            {
                _fireCalculatableAdded(childCalc);
            });
            SubscribeToCalculatableContainerEvents(calculatableContainer);
        }
    }

    [MirrorVerified("PtcCalculationCoreSupportCalculatableChildHiderMechanicalDifferentialTests")]
    public virtual void ChildRemoved(object child)
    {
        UnsubscribeFromPropertyChangedEvents(child);
        if (!IsHiddenFromCalculation(child))
        {
            ICalculatable calculatable = child as ICalculatable;
            if (calculatable != null)
            {
                _fireCalculatableRemoved(calculatable);
            }
        }
        ICalculatableContainer calculatableContainer = child as ICalculatableContainer;
        if (_adoptCalculatableContainerChildren && calculatableContainer != null && !(calculatableContainer is ICalculatableAggregate))
        {
            GetContainerChildren(calculatableContainer).Each((ICalculatable childCalc) =>
            {
                _fireCalculatableRemoved(childCalc);
            });
            UnsubscribeFromCalculatableContainerEvents(calculatableContainer);
        }
    }

    [MirrorVerified("PtcCalculationCoreSupportCalculatableChildHiderMechanicalDifferentialTests")]
    public virtual void ChildMoved(object child)
    {
        if (_fireCalculatableMoved == null)
        {
            return;
        }
        if (IsHiddenFromCalculation(child))
        {
            return;
        }
        ICalculatable calculatable = child as ICalculatable;
        if (calculatable != null)
        {
            _fireCalculatableMoved(calculatable);
        }
        ICalculatableContainer calculatableContainer = child as ICalculatableContainer;
        if (_adoptCalculatableContainerChildren && calculatableContainer != null && !(calculatableContainer is ICalculatableAggregate))
        {
            GetContainerChildren(calculatableContainer).Each((ICalculatable childCalc) =>
            {
                _fireCalculatableMoved(childCalc);
            });
        }
    }

    [MirrorVerified("PtcCalculationCoreSupportCalculatableChildHiderMechanicalDifferentialTests")]
    private void SubscribeToPropertyChangedEvents(object child)
    {
        ICalculationHideableChild calculationHideableChild = child as ICalculationHideableChild;
        if (calculationHideableChild == null)
        {
            return;
        }
        if (child is ICalculatable | (child is ICalculatableContainer && !(child is ICalculatableAggregate) && _adoptCalculatableContainerChildren))
        {
            calculationHideableChild.PropertyChanged += HandleCalculatablePropertyChanged;
        }
    }

    [MirrorVerified("PtcCalculationCoreSupportCalculatableChildHiderMechanicalDifferentialTests")]
    private void UnsubscribeFromPropertyChangedEvents(object child)
    {
        ICalculationHideableChild calculationHideableChild = child as ICalculationHideableChild;
        if (calculationHideableChild == null)
        {
            return;
        }
        if (child is ICalculatable | (child is ICalculatableContainer && !(child is ICalculatableAggregate) && _adoptCalculatableContainerChildren))
        {
            calculationHideableChild.PropertyChanged -= HandleCalculatablePropertyChanged;
        }
    }

    [MirrorVerified("PtcCalculationCoreSupportCalculatableChildHiderMechanicalDifferentialTests")]
    private bool SubscribeToCalculatableContainerEvents(ICalculatableContainer cntr)
    {
        if (!_adoptCalculatableContainerChildren)
        {
            return false;
        }
        if (cntr is ICalculatableAggregate)
        {
            return false;
        }
        cntr.CalculatableAdded += HandleCalculatableAddedToChildContainer;
        cntr.CalculatableRemoved += HandleCalculatableRemovedFromChildContainer;
        cntr.CalculatableMoved += HandleCalculatableMovedInChildContainer;
        return true;
    }

    [MirrorVerified("PtcCalculationCoreSupportCalculatableChildHiderMechanicalDifferentialTests")]
    private bool UnsubscribeFromCalculatableContainerEvents(ICalculatableContainer cntr)
    {
        if (!_adoptCalculatableContainerChildren)
        {
            return false;
        }
        if (cntr is ICalculatableAggregate)
        {
            return false;
        }
        cntr.CalculatableAdded -= HandleCalculatableAddedToChildContainer;
        cntr.CalculatableRemoved -= HandleCalculatableRemovedFromChildContainer;
        cntr.CalculatableMoved -= HandleCalculatableMovedInChildContainer;
        return true;
    }

    [MirrorVerified("PtcCalculationCoreSupportCalculatableChildHiderMechanicalDifferentialTests")]
    protected virtual void HandleCalculatablePropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        ICalculationHideableChild calculationHideableChild = sender as ICalculationHideableChild;
        if (calculationHideableChild == null || e.PropertyName != "HideFromCalculationContext")
        {
            return;
        }
        ICalculatable calculatable = sender as ICalculatable;
        if (calculatable != null)
        {
            HiddenStateChanged(calculatable, calculationHideableChild.HideFromCalculationContext);
            return;
        }
    }

    [MirrorVerified("PtcCalculationCoreSupportCalculatableChildHiderMechanicalDifferentialTests")]
    protected virtual void HiddenStateChanged(ICalculatable calc, bool hideFromCalculationContext)
    {
        if (hideFromCalculationContext)
        {
            _fireCalculatableRemoved(calc);
            calc.SetCalculatingStatus((CalculatingStatus)2);
            calc.SetSynchronizationStatus((SynchronizationStatus)3);
            return;
        }
        _fireCalculatableAdded(calc);
        calc.Commit();
        if (calc is ICalculatableAggregate calculatableAggregate)
        {
            calculatableAggregate.CalculatableChildren.Each((ICalculatable child) =>
            {
                child.Commit();
            });
        }
    }

    [MirrorVerified("PtcCalculationCoreSupportCalculatableChildHiderMechanicalDifferentialTests")]
    protected virtual void HandleCalculatableAddedToChildContainer(ICalculatableContainer container, ICalculatable calc)
    {
        _fireCalculatableAdded(calc);
    }

    [MirrorVerified("PtcCalculationCoreSupportCalculatableChildHiderMechanicalDifferentialTests")]
    protected virtual void HandleCalculatableRemovedFromChildContainer(ICalculatable calc)
    {
        _fireCalculatableRemoved(calc);
    }

    [MirrorVerified("PtcCalculationCoreSupportCalculatableChildHiderMechanicalDifferentialTests")]
    protected virtual void HandleCalculatableMovedInChildContainer(ICalculatable calc)
    {
        _fireCalculatableMoved(calc);
    }

    [MirrorVerified("PtcCalculationCoreSupportCalculatableChildHiderMechanicalDifferentialTests")]
    protected virtual IEnumerable<ICalculatable> GetContainerChildren(ICalculatableContainer container)
    {
        return container.CalculatableChildren;
    }
}
