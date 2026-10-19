#nullable disable
using Quire.Mirror;

namespace Quire.Mpl.Run;

public partial class CloDblFunction
{
    [MirrorAnalyzed("Mpl.Run.CloDblFunction")]
    public double m_clo;

    [MirrorVerified("MplRunCloDblFunctionMechanicalDifferentialTests")]
    private CloDblFunction() : base(null, 0) { }
}
