#nullable disable
using Quire.Mirror;

namespace Quire.Calculation.Core;

public partial class StringCollection
{
    [MirrorVerified("PtcCalculationCoreStringCollectionMechanicalDifferentialTests")]
    public StringCollection() { }

    [MirrorVerified("PtcCalculationCoreStringCollectionMechanicalDifferentialTests")]
    public StringCollection(IEnumerable<string> collection) : base(collection) { }
}
