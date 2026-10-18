#nullable disable
using Quire.Mirror;

namespace Quire.Calculation.Core.Support;

public partial class CalculatableWrapper
{
    [MirrorVerified("PtcCalculationCoreSupportCalculatableWrapperMechanicalDifferentialTests")]
    protected virtual void HandleRequestEvaluation(ICalculatable calc) => FireRequestEvaluation(this);

    [MirrorVerified("PtcCalculationCoreSupportCalculatableWrapperMechanicalDifferentialTests")]
    public event Action<ICalculatable> RequestEvaluation;

    [MirrorVerified("PtcCalculationCoreSupportCalculatableWrapperMechanicalDifferentialTests")]
    public event Func<ICalculatable, string, IResultNode> GetValueOfIdentifierBefore;

    [MirrorVerified("PtcCalculationCoreSupportCalculatableWrapperMechanicalDifferentialTests")]
    public event Action<ICalculatable> TreatAsUpToDate;
}
