#nullable disable
using Quire.Mirror;

namespace Quire;

public partial class AssemblyBuildIdAttribute
{
    [MirrorVerified("PtcAssemblyBuildIdAttributeMechanicalDifferentialTests")]
    public readonly string BuildId;

    [MirrorVerified("PtcAssemblyBuildIdAttributeMechanicalDifferentialTests")]
    public AssemblyBuildIdAttribute(string buildId)
    {
        BuildId = buildId;
    }
}
