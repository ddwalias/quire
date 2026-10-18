#nullable disable
using Quire.Mirror;
using Quire.Undo;

namespace Quire.Controls.Core.IORegions;

public partial class ToggleRegionsDesignationCommand
{
    [MirrorVerified("PtcControlsCoreIORegionsToggleRegionsDesignationCommandMechanicalDifferentialTests")]
    public bool Execute() => CommandBuffer.Execute(this, null);

    [MirrorVerified("PtcControlsCoreIORegionsToggleRegionsDesignationCommandMechanicalDifferentialTests")]
    private readonly string _name;

    [MirrorVerified("PtcControlsCoreIORegionsToggleRegionsDesignationCommandMechanicalDifferentialTests")]
    public string Name => _name;
}
