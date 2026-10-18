#nullable disable
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class RealNumberFormatter
{
    [MirrorVerified("PtcControlsCoreRealNumberFormatterMechanicalDifferentialTests")]
    public NumberFormat DisplayNumberFormat { get; private set; }
}
