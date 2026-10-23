#nullable disable
using System.Runtime.InteropServices;

namespace Quire.AutomationPrivate;

[global::Quire.Mirror.MirrorOf("MathcadAutomationPrivate.IMatrixValuePrivate, interop.mathcadautomationprivate")]
[ComImport]
[Guid("7BEEBD06-0A1A-11E0-BCC5-11F8E2A8C36C")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[TypeLibType(TypeLibTypeFlags.FOleAutomation)]
public interface IMatrixValuePrivate
{
    [DispId(0x60010000)]
    bool IsRange { get; }
}
