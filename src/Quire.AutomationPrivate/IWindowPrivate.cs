#nullable disable
using System.Runtime.InteropServices;

namespace Quire.AutomationPrivate;

[global::Quire.Mirror.MirrorOf("MathcadAutomationPrivate.IWindowPrivate, interop.mathcadautomationprivate")]
[ComImport]
[Guid("7BEF0B23-0A1A-11E0-9EFB-11F8E2A8C36C")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[TypeLibType(TypeLibTypeFlags.FOleAutomation)]
public interface IWindowPrivate
{
    void SetView(IntPtr pView);
}
