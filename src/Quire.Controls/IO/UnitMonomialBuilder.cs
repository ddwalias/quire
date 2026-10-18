#nullable disable
using Quire.Calculation.Core;
using Quire.Mirror;

namespace Quire.Controls.IO;

internal partial class UnitMonomialBuilder
{
    [MirrorVerified("PtcControlsIOUnitMonomialBuilderMechanicalDifferentialTests")]
    private readonly Dictionary<string, UnitDescriptor> _descriptors;

    [MirrorVerified("PtcControlsIOUnitMonomialBuilderMechanicalDifferentialTests")]
    private UnitMonomialBuilder()
    {
        _descriptors = DefaultUnitDescriptorTable.Instance;
    }
}
