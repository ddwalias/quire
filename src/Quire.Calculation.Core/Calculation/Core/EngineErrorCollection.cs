#nullable disable
using System.Collections.Generic;
using Quire.Mirror;

namespace Quire.Calculation.Core;

public partial class EngineErrorCollection
{
    [MirrorVerified("PtcCalculationCoreEngineErrorCollectionMechanicalDifferentialTests")]
    public EngineErrorCollection() { }

    public EngineErrorCollection(IEnumerable<EngineError> errors, EngineError error)
        : base(errors)
    {
        Add(error);
    }
}
