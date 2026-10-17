#nullable disable
using Quire.Mirror;

namespace Quire.Mpl.Run;

public partial class DynamicType
{
    [MirrorVerified("MplRunDynamicTypeMechanicalDifferentialTests")]
    public DynamicType() { }

    [MirrorVerified("DynamicTypeMplConcat3DifferentialTests")]
    public static string MplConcat3(object o1, object o2, object o3) =>
        string.Concat(o1 == null ? "null" : o1, o2 == null ? "null" : o2, o3 == null ? "null" : o3);

    [MirrorVerified("DynamicTypeConcat2MiDifferentialTests")]
    private static System.Reflection.MethodInfo Concat2Mi()
    {
        return typeof(string).GetMethod("Concat", new System.Type[] { typeof(object), typeof(object) });
    }
}
