#nullable disable
using Quire.Mirror;

namespace Quire.Mpl.Run;

public partial class MplDestructuringBind
{
    [MirrorVerified("MplDestructuringBindDifferentialTests")]
    public static readonly Sym sym_destructuring;

    [MirrorVerified("MplDestructuringBindDifferentialTests")]
    public static readonly MplDestructuringBind singleton;

    static MplDestructuringBind()
    {
        sym_destructuring = new Sym("destructuring_bind", "MplMacro");
        singleton = new MplDestructuringBind();
    }

    [MirrorVerified("MplDestructuringBindDifferentialTests")]
    private MplDestructuringBind()
        : base(sym_destructuring)
    {
    }

    [MirrorVerified("MplDestructuringBindDifferentialTests")]
    public override Sexpr ExpandCall(MplEnv e, Sexpr form)
    {
        SCons scons = form.scons;
        if (scons.Length != 4)
        {
            throw new AppException("illegal macro call", form.handle);
        }
        _ = scons.first;
        Sexpr pat = as_pattern(scons.second);
        Sexpr third = scons.third;
        Sexpr fourth = scons.fourth;
        Sexpr x = define_keyword.MakeDefine(form.handle, pat, third);
        return locally_keyword.MakeLocally(form.handle, SCons.list1(x), fourth);
    }

    [MirrorVerified("MplDestructuringBindDifferentialTests")]
    public static Sexpr as_pattern(Sexpr s)
    {
        switch (s.m_tag)
        {
            case 1:
            {
                SCons sCons = s as SCons;
                if (sCons.Field1.m_tag == 6)
                {
                    return (sCons.Field1 as Ca).Field1;
                }

                Sexpr x = as_pattern(sCons.Field1);
                Sexpr x2 = as_pattern(sCons.Field2);
                return SCons.list3(SCons.token, x, x2);
            }
            case 0:
                return s.quote;
            case 5:
                return (s as Cm).Field1;
            default:
                return s;
        }
    }
}
