#nullable disable
using System.Runtime.InteropServices;

namespace Quire.AutomationPrivate;

[global::Quire.Mirror.MirrorOf("MathcadAutomationPrivate.IValueFactory, interop.mathcadautomationprivate")]
[ComImport]
[Guid("7BEEE417-0A1A-11E0-B6C4-11F8E2A8C36C")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[TypeLibType(TypeLibTypeFlags.FOleAutomation)]
public interface IValueFactory
{
    [return: MarshalAs(UnmanagedType.Struct)]
    object EngineToAutomation([In, MarshalAs(UnmanagedType.Struct)] object engVal);

    [return: MarshalAs(UnmanagedType.Struct)]
    object VariantToAutomation([In, MarshalAs(UnmanagedType.Struct)] object real,
        [In, MarshalAs(UnmanagedType.Struct)] object imag);

    [return: MarshalAs(UnmanagedType.Struct)]
    object WrapperToAutomation([In, MarshalAs(UnmanagedType.Struct)] object wrapVal);

    [return: MarshalAs(UnmanagedType.Struct)]
    object VSDataToAutomation([In, MarshalAs(UnmanagedType.Struct)] object vsVal);

    [return: MarshalAs(UnmanagedType.Struct)]
    object AutomationToEngine([In, MarshalAs(UnmanagedType.Struct)] object o);

    void AutomationToVariant([In, MarshalAs(UnmanagedType.Struct)] object o,
        [MarshalAs(UnmanagedType.Struct)] out object pReal,
        [MarshalAs(UnmanagedType.Struct)] out object pImag);

    [return: MarshalAs(UnmanagedType.Struct)]
    object AutomationToWrapper([In, MarshalAs(UnmanagedType.Struct)] object wrapVal);

    [return: MarshalAs(UnmanagedType.Struct)]
    object AutomationToVSData([In, MarshalAs(UnmanagedType.Struct)] object vsVal);
}
