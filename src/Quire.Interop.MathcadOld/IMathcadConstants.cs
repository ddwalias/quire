#nullable disable
using System.Runtime.InteropServices;

namespace Quire.MathcadOld;

[global::Quire.Mirror.MirrorOf("MathcadOld.IMathcadConstants, interop.mathcadold")]
[ComImport]
[TypeLibType(TypeLibTypeFlags.FDual | TypeLibTypeFlags.FDispatchable)]
[Guid("7BEFCE75-0A1A-11E0-B40E-11F8E2A8C36C")]
public interface IMathcadConstants
{
    [DispId(1)]
    int TextRegion
    {
        [DispId(1)]
        get;
    }

    [DispId(2)]
    int EqRegion
    {
        [DispId(2)]
        get;
    }

    [DispId(3)]
    int MetafileRegion
    {
        [DispId(3)]
        get;
    }

    [DispId(4)]
    int BitmapRegion
    {
        [DispId(4)]
        get;
    }

    [DispId(5)]
    int OleObjectRegion
    {
        [DispId(5)]
        get;
    }
}
