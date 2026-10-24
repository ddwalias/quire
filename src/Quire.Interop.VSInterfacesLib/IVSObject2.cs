#nullable disable
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quire.VSINTERFACESLib;

[global::Quire.Mirror.MirrorOf("VSINTERFACESLib.IVSObject2, interop.vsinterfaceslib")]
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("7BF3C615-0A1A-11E0-9E29-11F8E2A8C36C")]
[TypeLibType(TypeLibTypeFlags.FOleAutomation)]
public interface IVSObject2
{
	void GetControlsIDispatch([MarshalAs(UnmanagedType.IDispatch)] ref object ppDisp);
}
