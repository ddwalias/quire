#nullable disable
using Quire.Mirror;

namespace Quire.Mpl.Run;

public partial class ModuleInterfaceAttribute
{
    [MirrorVerified("MplRunModuleInterfaceAttributeMechanicalDifferentialTests")]
    private string m_txt;

    [MirrorVerified("MplRunModuleInterfaceAttributeMechanicalDifferentialTests")]
    public override string ToString() => m_txt;

    [MirrorVerified("MplRunModuleInterfaceAttributeMechanicalDifferentialTests")]
    public ModuleInterfaceAttribute(string s)
    {
        m_txt = s;
    }

    [MirrorVerified("MplRunModuleInterfaceAttributeMechanicalDifferentialTests")]
    public string Interface => m_txt;
}
