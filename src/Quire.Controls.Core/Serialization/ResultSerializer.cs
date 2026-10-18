#nullable disable
using Quire.Calculation.Core;
using Quire.Mirror;

namespace Quire.Controls.Core.Serialization;

public partial class ResultSerializer
{
    [MirrorVerified("PtcControlsCoreSerializationResultSerializerMechanicalDifferentialTests")]
    private readonly Dictionary<int, EngineError> _errorMap;

    [MirrorVerified("PtcControlsCoreSerializationResultSerializerMechanicalDifferentialTests")]
    private void AddError(EngineError error, int id) => _errorMap.Add(id, error);
}
