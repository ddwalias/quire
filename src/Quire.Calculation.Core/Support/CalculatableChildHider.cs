#nullable disable
using Quire.Mirror;

namespace Quire.Calculation.Core.Support;

public partial class CalculatableChildHider
{
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
