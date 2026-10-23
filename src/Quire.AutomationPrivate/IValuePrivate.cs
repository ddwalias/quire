#nullable disable
using System.Runtime.InteropServices;

namespace Quire.AutomationPrivate;

[global::Quire.Mirror.MirrorOf("MathcadAutomationPrivate.IValuePrivate, interop.mathcadautomationprivate")]
[ComImport]
[Guid("7BEEBD05-0A1A-11E0-B6F6-11F8E2A8C36C")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[TypeLibType(TypeLibTypeFlags.FOleAutomation)]
public interface IValuePrivate
{
    [DispId(0x60010000)]
    int Size { get; }

    [return: MarshalAs(UnmanagedType.BStr)]
    string XML([In] bool bSimplify = false);

    void Save([In, MarshalAs(UnmanagedType.Interface)] IStream pstm);

    void Load([In, MarshalAs(UnmanagedType.Interface)] IStream pstm);
}
