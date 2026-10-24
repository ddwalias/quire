#nullable disable
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quire.VSINTERFACESLib;

[global::Quire.Mirror.MirrorOf("VSINTERFACESLib.IVSQueueInput, interop.vsinterfaceslib")]
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[TypeLibType(TypeLibTypeFlags.FOleAutomation)]
[Guid("7BF377F3-0A1A-11E0-B4BB-11F8E2A8C36C")]
public interface IVSQueueInput
{
	void IsReady();

	[return: MarshalAs(UnmanagedType.Interface)]
	IVSDataType Recv();
}
