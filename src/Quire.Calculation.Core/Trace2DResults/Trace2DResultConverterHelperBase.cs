#nullable disable
using Quire.Mirror;

namespace Quire.Calculation.Core.Trace2DResults;

public partial class Trace2DResultConverterHelperBase
{
    [MirrorVerified("PtcCalculationCoreTrace2DResultsTrace2DResultConverterHelperBaseMechanicalDifferentialTests")]
    protected ITrace2DResultOwner ResultsOwner { get; private set; }
}
