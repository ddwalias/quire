using Quire.Mirror;

namespace Quire.Mpl.Run;

public partial class List_keyword
{
    [MirrorVerified("SimpleKeywordDifferentialTests")]
    public static List_keyword singleton;

    static List_keyword()
    {
        singleton = new List_keyword();
    }

    [MirrorVerified("SimpleKeywordDifferentialTests")]
    public List_keyword() : base("List", 13)
    {
    }

    [MirrorVerified("SimpleKeywordDifferentialTests")]
    public static Tok NewTok(uint handle)
    {
        return new Tok(singleton, handle);
    }
}

public partial class Tuple_keyword
{
    [MirrorVerified("SimpleKeywordDifferentialTests")]
    public static Tuple_keyword singleton;

    static Tuple_keyword()
    {
        singleton = new Tuple_keyword();
    }

    [MirrorVerified("SimpleKeywordDifferentialTests")]
    public Tuple_keyword() : base("Tuple", 14)
    {
    }

    [MirrorVerified("SimpleKeywordDifferentialTests")]
    public static Tok NewTok(uint handle)
    {
        return new Tok(singleton, handle);
    }
}
