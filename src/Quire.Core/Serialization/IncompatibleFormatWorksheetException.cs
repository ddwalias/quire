#nullable disable
using Quire.Mirror;

namespace Quire.Core.Serialization;

public partial class IncompatibleFormatWorksheetException
{
    [MirrorVerified("PtcCoreSerializationIncompatibleFormatWorksheetExceptionMechanicalDifferentialTests")]
    internal IncompatibleFormatWorksheetException() : base("This file is not compatible with your current version of Mathcad, because it is from a more recent version of Mathcad.") { }
}
