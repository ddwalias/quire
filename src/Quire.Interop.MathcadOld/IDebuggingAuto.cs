#nullable disable
using System.Runtime.InteropServices;

namespace Quire.MathcadOld;

[global::Quire.Mirror.MirrorOf("MathcadOld.IDebuggingAuto, interop.mathcadold")]
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
[Guid("7BEFCE77-0A1A-11E0-89C2-11F8E2A8C36C")]
[TypeLibType(TypeLibTypeFlags.FDispatchable)]
public interface IDebuggingAuto
{
    [PreserveSig]
    [DispId(1)]
    void DoMonaMode();

    [PreserveSig]
    [DispId(2)]
    void ShowWindow();

    [PreserveSig]
    [DispId(3)]
    void HideWindow();

    [PreserveSig]
    [DispId(4)]
    [return: MarshalAs(UnmanagedType.BStr)]
    string QuickTestFile([MarshalAs(UnmanagedType.BStr)] string file_path);

    [PreserveSig]
    [DispId(5)]
    void GlobalSearchandReplace([MarshalAs(UnmanagedType.BStr)] string command);
}
