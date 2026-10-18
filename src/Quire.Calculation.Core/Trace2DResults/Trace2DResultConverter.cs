#nullable disable
using Quire.Mirror;
using Quire.Mirror;

namespace Quire.Calculation.Core.Trace2DResults;

public partial class Trace2DResultConverter
{
    [MirrorVerified("PtcCalculationCoreTrace2DResultsTrace2DResultConverterMechanicalDifferentialTests")]
    private readonly ITrace2DConverterHelper _helper;

    [MirrorVerified("PtcCalculationCoreTrace2DResultsTrace2DResultConverterMechanicalDifferentialTests")]
    public Trace2DResultConverter(ITrace2DConverterHelper helper)
    {
        _helper = helper;
    }

    [MirrorVerified("PtcCalculationCoreTrace2DResultsTrace2DResultConverterMechanicalDifferentialTests")]
    private bool ReadAsColumns => _helper.CurrentType != TraceType.Boxes;
}
