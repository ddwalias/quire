#nullable disable
using Quire.Mirror;

namespace Quire.Mpl.Run;

public partial class declare_keyword
{
    [MirrorVerified("MplRundeclare_keywordMechanicalDifferentialTests")]
    public declare_keyword() : base("declare", 5) { }

    [MirrorVerified("MplRundeclare_keywordMechanicalDifferentialTests")]
    public override SCons walk_definition_as(MplEnv env, SCons form) => form;
}
