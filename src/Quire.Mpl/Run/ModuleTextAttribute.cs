#nullable disable
using Quire.Mirror;

namespace Quire.Mpl.Run;

public partial class ModuleTextAttribute
{
    [MirrorVerified("MplRunModuleTextAttributeMechanicalDifferentialTests")]
    private string m_txt;

    [MirrorVerified("MplRunModuleTextAttributeMechanicalDifferentialTests")]
    public override string ToString() => m_txt;

    [MirrorVerified("MplRunModuleTextAttributeMechanicalDifferentialTests")]
    public ModuleTextAttribute(string s)
    {
        m_txt = s;
    }

    [MirrorVerified("MplRunModuleTextAttributeMechanicalDifferentialTests")]
    public string Text => m_txt;
}
