#nullable disable
using Quire.Mirror;

namespace Quire;

public partial class AssemblyStreamAttribute
{
    [MirrorVerified("PtcAssemblyStreamAttributeMechanicalDifferentialTests")]
    public readonly string Stream;

    [MirrorVerified("PtcAssemblyStreamAttributeMechanicalDifferentialTests")]
    public AssemblyStreamAttribute(string stream)
    {
        Stream = stream;
    }
}
