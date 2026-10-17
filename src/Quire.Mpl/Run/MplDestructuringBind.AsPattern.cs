#nullable disable
using Quire.Mirror;

namespace Quire.Mpl.Run;

public partial class MplDestructuringBind
{
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
