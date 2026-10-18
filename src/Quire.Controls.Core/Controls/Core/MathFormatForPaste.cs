#nullable disable
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class MathFormatForPaste
{
    [MirrorVerified("PtcControlsCoreMathFormatForPasteMechanicalDifferentialTests")]
    private MathFormatForPaste() { }

    [MirrorVerified("PtcControlsCoreMathFormatForPasteMechanicalDifferentialTests")]
    private static MathFormatForPaste _instance;

    [MirrorVerified("PtcControlsCoreMathFormatForPasteMechanicalDifferentialTests")]
    public static MathFormatForPaste Instance => _instance;

    [MirrorVerified("PtcControlsCoreMathFormatForPasteMechanicalDifferentialTests")]
    public IMathFormat DocumentMathFormat { get; private set; }

    [MirrorVerified("PtcControlsCoreMathFormatForPasteMechanicalDifferentialTests")]
    public IDictionary<IdLabels, ExtendedFont> DocumentLabeledIdFormat { get; private set; }
}
