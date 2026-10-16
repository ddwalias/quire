#nullable disable
using Quire.Mirror;

namespace Quire.Mpl.Run;

public partial class defdata_keyword
{
    [MirrorVerified("MplRundefdata_keywordMechanicalDifferentialTests")]
    public defdata_keyword() : base("defdata", 3) { }

    [MirrorVerified("MplRundefdata_keywordMechanicalDifferentialTests")]
    public override Cons walk_declaration_as(MplEnv env, SCons form) => new Cons(form, null);

    [MirrorVerified("MplRundefdata_keywordMechanicalDifferentialTests")]
    public override SCons walk_definition_as(MplEnv env, SCons form) => form;
}
