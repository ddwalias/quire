#nullable disable
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quire.MathcadOld;

[global::Quire.Mirror.MirrorOf("MathcadOld.DebuggingAutoClass, interop.mathcadold")]
[ComImport]
[Guid("017000FB-1122-11DB-9380-000D56C6051A")]
[ClassInterface(ClassInterfaceType.None)]
[TypeLibType(TypeLibTypeFlags.FCanCreate)]
public class DebuggingAutoClass : IDebuggingAuto, DebuggingAuto
{
    [MethodImpl(MethodImplOptions.InternalCall)]
    [PreserveSig]
    [DispId(1)]
    public virtual extern void DoMonaMode();

    [MethodImpl(MethodImplOptions.InternalCall)]
    [PreserveSig]
    [DispId(2)]
    public virtual extern void ShowWindow();

    [MethodImpl(MethodImplOptions.InternalCall)]
    [PreserveSig]
    [DispId(3)]
    public virtual extern void HideWindow();

    [MethodImpl(MethodImplOptions.InternalCall)]
    [PreserveSig]
    [DispId(4)]
    [return: MarshalAs(UnmanagedType.BStr)]
    public virtual extern string QuickTestFile([MarshalAs(UnmanagedType.BStr)] string file_path);

    [MethodImpl(MethodImplOptions.InternalCall)]
    [PreserveSig]
    [DispId(5)]
    public virtual extern void GlobalSearchandReplace([MarshalAs(UnmanagedType.BStr)] string command);
}
