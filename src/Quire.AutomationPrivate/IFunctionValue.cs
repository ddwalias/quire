#nullable disable
using System.Runtime.InteropServices;

namespace Quire.AutomationPrivate;

[global::Quire.Mirror.MirrorOf("MathcadAutomationPrivate.IFunctionValue, interop.mathcadautomationprivate")]
[ComImport]
[Guid("7BEEE415-0A1A-11E0-AD06-11F8E2A8C36C")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[TypeLibType(TypeLibTypeFlags.FOleAutomation)]
public interface IFunctionValue
{
    [DispId(0x60010000)]
    int Arity { get; }
}
