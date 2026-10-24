#nullable disable
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quire.VSINTERFACESLib;

[global::Quire.Mirror.MirrorOf("VSINTERFACESLib.IVSItemContainerExec, interop.vsinterfaceslib")]
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("7BF3C617-0A1A-11E0-B396-11F8E2A8C36C")]
[TypeLibType(TypeLibTypeFlags.FOleAutomation)]
public interface IVSItemContainerExec
{
	void Error([MarshalAs(UnmanagedType.Interface)] IVSObject lpVSObject, [MarshalAs(UnmanagedType.BStr)] string pcszError);

	void Stepped([MarshalAs(UnmanagedType.Interface)] IVSObject lpVSObject, ref int lpKey);

	void GetAppState([MarshalAs(UnmanagedType.Interface)] IVSObject lpVSObject, ref VSAPPSTATE pState);

	void SetAppState([MarshalAs(UnmanagedType.Interface)] IVSObject lpVSObject, VSAPPSTATE vsState);
}
