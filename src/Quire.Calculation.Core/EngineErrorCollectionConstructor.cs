using Quire.Mirror;

namespace Quire.Calculation.Core;

public partial class EngineErrorCollection
{
    [MirrorVerified("PtcCalculationCoreEngineErrorCollectionMechanicalDifferentialTests")]
    public EngineErrorCollection(IEnumerable<EngineError> errors, EngineError error) : base(errors)
    {
        base.Add(error);
    }
}
