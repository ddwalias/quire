#nullable disable
using Quire.Mirror;

namespace Quire.Controls.Core.MC14Conversion;

public partial class Mathcad14TextStyle
{
    [MirrorVerified("PtcControlsCoreMC14ConversionMathcad14TextStyleMechanicalDifferentialTests")]
    private Dictionary<string, Mathcad14TextStyle> _stylesDictionary;

    [MirrorVerified("PtcControlsCoreMC14ConversionMathcad14TextStyleMechanicalDifferentialTests")]
    public Mathcad14TextStyle(Dictionary<string, Mathcad14TextStyle> stylesDictionary)
    {
        _stylesDictionary = stylesDictionary;
    }

    [MirrorVerified("PtcControlsCoreMC14ConversionMathcad14TextStyleMechanicalDifferentialTests")]
    public string Name { get; set; }

    [MirrorVerified("PtcControlsCoreMC14ConversionMathcad14TextStyleMechanicalDifferentialTests")]
    public string BaseStyleName { get; set; }
}
