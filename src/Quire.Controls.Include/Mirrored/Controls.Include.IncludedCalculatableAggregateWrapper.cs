#nullable disable
#pragma warning disable
using System;
using System.Collections.Generic;
using Quire.Calculation.Core;

namespace Quire.Controls.Include;

public partial class IncludedCalculatableAggregateWrapper
{
    public IncludedCalculatableAggregateWrapper(ICalculatableAggregate wrappedCalculatableAggregate)
        : base(wrappedCalculatableAggregate)
    {
        HookupAggregateContainerEvents(wrappedCalculatableAggregate);
    }

    public ICalculatableAggregate WrappedCalculatableAggregate => (ICalculatableAggregate)WrappedCalculatable;

    private void HookupAggregateContainerEvents(ICalculatableAggregate aggregate)
    {
        aggregate.CalculatableAdded += HandleCalculatableAdded;
        aggregate.CalculatableRemoved += HandleCalculatableRemoved;
        aggregate.CalculatableMoved += HandleCalculatableMoved;
    }

    private void UnhookAggregateContainerEvents(ICalculatableAggregate aggregate)
    {
        aggregate.CalculatableAdded -= HandleCalculatableAdded;
        aggregate.CalculatableRemoved -= HandleCalculatableRemoved;
        aggregate.CalculatableMoved -= HandleCalculatableMoved;
    }

    public override void Dispose()
    {
        base.Dispose();
        UnhookAggregateContainerEvents(WrappedCalculatableAggregate);
    }

    private void HandleCalculatableAdded(ICalculatableContainer container, ICalculatable newCalc)
    {
        if (CalculatableAdded != null)
        {
            CalculatableAdded(this, newCalc);
        }
    }

    private void HandleCalculatableRemoved(ICalculatable removedCalc)
    {
        if (CalculatableRemoved != null)
        {
            CalculatableRemoved(removedCalc);
        }
    }

    private void HandleCalculatableMoved(ICalculatable movedCalc)
    {
        if (CalculatableMoved != null)
        {
            CalculatableMoved(movedCalc);
        }
    }

    public event Action<ICalculatableContainer, ICalculatable> CalculatableAdded;

    public event Action<ICalculatable> CalculatableRemoved;

    public event Action<ICalculatable> CalculatableMoved;

    public IEnumerable<ICalculatable> CalculatableChildren => WrappedCalculatableAggregate.CalculatableChildren;
}
