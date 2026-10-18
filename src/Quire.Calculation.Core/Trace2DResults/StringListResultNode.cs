#nullable disable
using Quire.Mirror;
using Quire.Mirror;

namespace Quire.Calculation.Core.Trace2DResults;

public partial class StringListResultNode
{
    [MirrorVerified("PtcCalculationCoreTrace2DResultsStringListResultNodeMechanicalDifferentialTests")]
    public StringListResultNode(IList<string> data) : base(data, -1) { }

    [MirrorVerified("PtcCalculationCoreTrace2DResultsStringListResultNodeMechanicalDifferentialTests")]
    public StringListResultNode(IList<string> data, string paddingValue, int minLength) : base(data, paddingValue, minLength) { }

    [MirrorVerified("PtcCalculationCoreTrace2DResultsStringListResultNodeMechanicalDifferentialTests")]
    public override string NeutralValue => null;

    [MirrorVerified("PtcCalculationCoreTrace2DResultsStringListResultNodeMechanicalDifferentialTests")]
    public override bool IsMeaningfulValue(string value) => value != null;
}
