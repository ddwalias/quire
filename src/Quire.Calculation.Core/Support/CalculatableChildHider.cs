#nullable disable
using Quire.Mirror;

namespace Quire.Calculation.Core.Support;

public partial class CalculatableChildHider
{
    [MirrorVerified("PtcCalculationCoreSupportCalculatableChildHiderChildrenMechanicalDifferentialTests")]
    public static bool IsHiddenFromCalculation(object child)
    {
        if (child is ICalculationHideableChild calculationHideableChild)
        {
            return calculationHideableChild.HideFromCalculationContext;
        }
        return false;
    }

    [MirrorVerified("PtcCalculationCoreSupportCalculatableChildHiderChildrenMechanicalDifferentialTests")]
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
                ICalculatableContainer container = child as ICalculatableContainer;
                if (calculatable != null)
                {
                    yield return calculatable;
                }
                if (container == null || child is ICalculatableAggregate)
                {
                    continue;
                }
                foreach (ICalculatable containerChild in GetContainerChildren(container))
                {
                    yield return containerChild;
                }
            }
        }
    }

    [MirrorVerified("PtcCalculationCoreSupportCalculatableChildHiderMechanicalDifferentialTests")]
    protected CalculatableChildHider() { }

    [MirrorVerified("PtcCalculationCoreSupportCalculatableChildHiderMechanicalDifferentialTests")]
    private Action<ICalculatable> _fireCalculatableAdded;

    [MirrorVerified("PtcCalculationCoreSupportCalculatableChildHiderMechanicalDifferentialTests")]
    protected virtual void HandleCalculatableAddedToChildContainer(ICalculatableContainer container, ICalculatable calc) => _fireCalculatableAdded.Invoke(calc);

    [MirrorVerified("PtcCalculationCoreSupportCalculatableChildHiderMechanicalDifferentialTests")]
    private Action<ICalculatable> _fireCalculatableRemoved;

    [MirrorVerified("PtcCalculationCoreSupportCalculatableChildHiderMechanicalDifferentialTests")]
    protected virtual void HandleCalculatableRemovedFromChildContainer(ICalculatable calc) => _fireCalculatableRemoved.Invoke(calc);

    [MirrorVerified("PtcCalculationCoreSupportCalculatableChildHiderMechanicalDifferentialTests")]
    protected Action<ICalculatable> _fireCalculatableMoved;

    [MirrorVerified("PtcCalculationCoreSupportCalculatableChildHiderMechanicalDifferentialTests")]
    protected virtual void HandleCalculatableMovedInChildContainer(ICalculatable calc) => _fireCalculatableMoved.Invoke(calc);

    [MirrorVerified("PtcCalculationCoreSupportCalculatableChildHiderMechanicalDifferentialTests")]
    protected virtual IEnumerable<ICalculatable> GetContainerChildren(ICalculatableContainer container) => container.CalculatableChildren;
}
