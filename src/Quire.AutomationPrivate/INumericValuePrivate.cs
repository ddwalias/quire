#nullable disable
using System.Runtime.InteropServices;

namespace Quire.AutomationPrivate;

[global::Quire.Mirror.MirrorOf("MathcadAutomationPrivate.INumericValuePrivate, interop.mathcadautomationprivate")]
[ComImport]
[Guid("7BEEE414-0A1A-11E0-901D-11F8E2A8C36C")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[TypeLibType(TypeLibTypeFlags.FOleAutomation)]
public interface INumericValuePrivate
{
    [return: MarshalAs(UnmanagedType.BStr)]
    string UnitsXML([In] bool bSimplify = false);
}
