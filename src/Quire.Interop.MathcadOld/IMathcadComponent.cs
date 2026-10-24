#nullable disable
using System.Runtime.InteropServices;

namespace Quire.MathcadOld;

[global::Quire.Mirror.MirrorOf("MathcadOld.IMathcadComponent, interop.mathcadold")]
[ComImport]
[TypeLibType(TypeLibTypeFlags.FHidden | TypeLibTypeFlags.FOleAutomation)]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("7BEFCE76-0A1A-11E0-9E89-11F8E2A8C36C")]
public interface IMathcadComponent
{
    int Recalculate();

    int GetComplex([MarshalAs(UnmanagedType.BStr)] string Name, [MarshalAs(UnmanagedType.Struct)] ref object RealPart, [MarshalAs(UnmanagedType.Struct)] ref object ImagPart);

    int SetComplex([MarshalAs(UnmanagedType.BStr)] string Name, [MarshalAs(UnmanagedType.Struct)] object RealPart, [MarshalAs(UnmanagedType.Struct)] object ImagPart);

    int SaveAs([MarshalAs(UnmanagedType.BStr)] string Name);
}
