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

    public CalculatableChildHider(IEnumerable<object> getChildren, Action<ICalculatable> fireCalculatableAdded, Action<ICalculatable> fireCalculatableRemoved, Action<ICalculatable> fireCalculatableMoved, bool adoptCalculatableContainerChildren)
    {
        Initialize(getChildren, fireCalculatableAdded, fireCalculatableRemoved, fireCalculatableMoved, adoptCalculatableContainerChildren);
    }

    protected CalculatableChildHider()
    {
    }

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

    public static bool IsHiddenFromCalculation(object child)
    {
        if (child is ICalculationHideableChild calculationHideableChild)
        {
            return calculationHideableChild.HideFromCalculationContext;
        }
        return false;
    }

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
                if (cntr == null || child is ICalculatableAggregate)
                {
                    continue;
                }
                foreach (ICalculatable containerChild in GetContainerChildren(cntr))
                {
                    yield return containerChild;
                }
            }
        }
    }

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

    public virtual void ChildRemoved(object child)
    {
        UnsubscribeFromPropertyChangedEvents(child);
        if (!IsHiddenFromCalculation(child) && child is ICalculatable obj)
        {
            _fireCalculatableRemoved(obj);
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

    public virtual void ChildMoved(object child)
    {
        if (_fireCalculatableMoved == null || IsHiddenFromCalculation(child))
        {
            return;
        }
        if (child is ICalculatable obj)
        {
            _fireCalculatableMoved(obj);
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

    private void SubscribeToPropertyChangedEvents(object child)
    {
        if (child is ICalculationHideableChild calculationHideableChild && ((child is ICalculatable) | (child is ICalculatableContainer && !(child is ICalculatableAggregate) && _adoptCalculatableContainerChildren)))
        {
            calculationHideableChild.PropertyChanged += HandleCalculatablePropertyChanged;
        }
    }

    private void UnsubscribeFromPropertyChangedEvents(object child)
    {
        if (child is ICalculationHideableChild calculationHideableChild && ((child is ICalculatable) | (child is ICalculatableContainer && !(child is ICalculatableAggregate) && _adoptCalculatableContainerChildren)))
        {
            calculationHideableChild.PropertyChanged -= HandleCalculatablePropertyChanged;
        }
    }

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

    protected virtual void HandleCalculatablePropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (sender is ICalculationHideableChild calculationHideableChild && !(e.PropertyName != "HideFromCalculationContext") && sender is ICalculatable calc)
        {
            HiddenStateChanged(calc, calculationHideableChild.HideFromCalculationContext);
        }
    }

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

    protected virtual void HandleCalculatableAddedToChildContainer(ICalculatableContainer container, ICalculatable calc)
    {
        _fireCalculatableAdded(calc);
    }

    protected virtual void HandleCalculatableRemovedFromChildContainer(ICalculatable calc)
    {
        _fireCalculatableRemoved(calc);
    }

    protected virtual void HandleCalculatableMovedInChildContainer(ICalculatable calc)
    {
        _fireCalculatableMoved(calc);
    }

    protected virtual IEnumerable<ICalculatable> GetContainerChildren(ICalculatableContainer container)
    {
        return container.CalculatableChildren;
    }
}
