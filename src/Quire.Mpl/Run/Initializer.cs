#nullable disable
using Quire.Mirror;

namespace Quire.Mpl.Run;

public partial class Initializer
{
    [MirrorVerified("MplRunInitializerMechanicalDifferentialTests")]
    public void InitializeAssemblyPath(string path) => Rts.AssemblyPath = path;

    [MirrorVerified("MplRunInitializerMechanicalDifferentialTests")]
    public Initializer() { }
}
