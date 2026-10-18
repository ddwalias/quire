#nullable disable
using Quire.Mirror;

namespace Quire.Calculation.Core;

public partial class EngineErrorMessages
{
    [MirrorVerified("PtcCalculationCoreEngineErrorMessagesMechanicalDifferentialTests")]
    private static string EscapeForFormatString(string original) => original.Replace("{", "{{").Replace("}", "}}");
}
