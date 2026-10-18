using Quire.Automation;
using Quire.Mirror;

namespace Quire.Calculation;

internal partial class MatrixValueProxyWrapper
{
    [MirrorVerified("PtcCalculationMatrixValueProxyWrapperCctorDifferentialTests")]
    static MatrixValueProxyWrapper()
    {
        var options = new PlainDataConversionOptions { ImaginaryThreshold = double.NaN };
        _conversionOptions = options;
    }
}
