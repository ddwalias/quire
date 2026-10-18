#nullable disable
using Quire.Mirror;

namespace Quire.Calculation.Core.Trace2DResults;

public partial class DoubleListResultNode
{
    [MirrorVerified("PtcCalculationCoreTrace2DResultsDoubleListResultNodeMechanicalDifferentialTests")]
    public DoubleListResultNode(IList<double> doubleValues) : base(doubleValues, -1) { }

    [MirrorVerified("PtcCalculationCoreTrace2DResultsDoubleListResultNodeMechanicalDifferentialTests")]
    public DoubleListResultNode(IList<double> doubleValues, double paddingValue, int minLength) : base(doubleValues, paddingValue, minLength) { }

    [MirrorVerified("PtcCalculationCoreTrace2DResultsDoubleListResultNodeMechanicalDifferentialTests")]
    public override bool IsMeaningfulValue(double value) => DoubleExtension.IsValid(value);

    [MirrorVerified("PtcCalculationCoreTrace2DResultsDoubleListResultNodeMechanicalDifferentialTests")]
    public override double NeutralValue => double.NaN;
}
