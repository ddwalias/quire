#nullable disable
using Quire.Mirror;

namespace Quire.Calculation.Core;

public partial class ResultConverter
{
    [MirrorVerified("PtcCalculationCoreResultConverterMechanicalDifferentialTests")]
    private readonly IResultNodeFactory _newResultNodeFactory;

    [MirrorVerified("PtcCalculationCoreResultConverterMechanicalDifferentialTests")]
    public ResultConverter(IResultNodeFactory newResultNodeFactory)
    {
        _newResultNodeFactory = newResultNodeFactory;
    }
}
