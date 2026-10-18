#nullable disable
using Quire.Mirror;

namespace Quire.Mpl.Run;

public partial class let_keyword
{
    [MirrorVerified("SimpleKeywordDifferentialTests")]
    public static let_keyword singleton;

    [MirrorVerified("SimpleKeywordDifferentialTests")]
    static let_keyword()
    {
        singleton = new let_keyword();
    }

    [MirrorVerified("MplRunlet_keywordMechanicalDifferentialTests")]
    public let_keyword() : base("let", 7) { }

    [MirrorVerified("SimpleKeywordDifferentialTests")]
    public static Tok NewTok(uint handle) => new Tok(singleton, handle);
}
