#nullable disable
using System.Runtime.InteropServices;

namespace Quire.AutomationPrivate;

[global::Quire.Mirror.MirrorOf("MathcadAutomationPrivate.IMASImageCache, interop.mathcadautomationprivate")]
[ComImport]
[Guid("7BEF0B26-0A1A-11E0-9181-11F8E2A8C36C")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[TypeLibType(TypeLibTypeFlags.FOleAutomation)]
public interface IMASImageCache
{
    [return: MarshalAs(UnmanagedType.BStr)]
    string TouchFile([In, MarshalAs(UnmanagedType.BStr)] string bstrFile);

    [return: MarshalAs(UnmanagedType.BStr)]
    string AddFile([In, MarshalAs(UnmanagedType.BStr)] string bstrFile);
}
