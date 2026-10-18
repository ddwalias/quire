using Quire.Mirror;

namespace Quire.Calculation.Core;

public partial class RedefinitionWarningsDictionary
{
    [MirrorVerified("PtcCalculationCoreRedefinitionWarningsDictionaryCctorDifferentialTests")]
    static RedefinitionWarningsDictionary() => KeysOwnerTypes = [typeof(RedefinitionWarningMsgCodes)];
}
