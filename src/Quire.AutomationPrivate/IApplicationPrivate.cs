#nullable disable
using System.Runtime.InteropServices;

namespace Quire.AutomationPrivate;

[global::Quire.Mirror.MirrorOf("MathcadAutomationPrivate.IApplicationPrivate, interop.mathcadautomationprivate")]
[ComImport]
[Guid("7BEF0B24-0A1A-11E0-81D7-11F8E2A8C36C")]
[TypeLibType(TypeLibTypeFlags.FOleAutomation)]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IApplicationPrivate
{
    void SetMCServer();

    uint GetProcID();

    void EnableScriptRegions([In] bool bEnable);

    bool Open([In, MarshalAs(UnmanagedType.BStr)] string pPath);

    bool OpenResourceCenter([In, MarshalAs(UnmanagedType.BStr)] string pPath);
}
