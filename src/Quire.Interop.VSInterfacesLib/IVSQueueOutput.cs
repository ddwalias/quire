#nullable disable
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quire.VSINTERFACESLib;

[global::Quire.Mirror.MirrorOf("VSINTERFACESLib.IVSQueueOutput, interop.vsinterfaceslib")]
[ComImport]
[TypeLibType(TypeLibTypeFlags.FOleAutomation)]
[Guid("7BF377F4-0A1A-11E0-BDCB-11F8E2A8C36C")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IVSQueueOutput
{
	void IsReady();

	void Send([MarshalAs(UnmanagedType.Interface)] IVSDataType lpVSData);
}
