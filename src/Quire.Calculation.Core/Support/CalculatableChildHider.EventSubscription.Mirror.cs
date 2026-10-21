#nullable disable
using Quire.Mirror;

namespace Quire.Calculation.Core.Support;

public partial class CalculatableChildHider
{
    [MirrorVerified("PtcCalculationCoreSupportCalculatableChildHiderEventSubscriptionMechanicalDifferentialTests")]
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

    [MirrorVerified("PtcCalculationCoreSupportCalculatableChildHiderEventSubscriptionMechanicalDifferentialTests")]
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
}
