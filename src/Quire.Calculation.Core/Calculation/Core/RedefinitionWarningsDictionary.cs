using Quire.Mirror;

namespace Quire.Calculation.Core;

public partial class RedefinitionWarningsDictionary
{
    [MirrorVerified("PtcCalculationCoreRedefinitionWarningsDictionaryCctorMechanicalDifferentialTests")]
    static RedefinitionWarningsDictionary() => KeysOwnerTypes = [typeof(RedefinitionWarningMsgCodes)];
}
