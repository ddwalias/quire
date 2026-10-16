#nullable disable
using Quire.Mirror;

namespace Quire.Mpl.Run;

public partial class deftype_keyword
{
    [MirrorVerified("MplRundeftype_keywordMechanicalDifferentialTests")]
    public deftype_keyword() : base("deftype", 2) { }

    [MirrorVerified("MplRundeftype_keywordMechanicalDifferentialTests")]
    public override Cons walk_declaration_as(MplEnv env, SCons form) => new Cons(form, null);

    [MirrorVerified("MplRundeftype_keywordMechanicalDifferentialTests")]
    public override SCons walk_definition_as(MplEnv env, SCons form) => form;
}
