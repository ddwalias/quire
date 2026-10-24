#nullable disable
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quire.VSINTERFACESLib;

[global::Quire.Mirror.MirrorOf("VSINTERFACESLib.IVSContainer, interop.vsinterfaceslib")]
[ComImport]
[Guid("7BF39F05-0A1A-11E0-A75E-11F8E2A8C36C")]
[TypeLibType(TypeLibTypeFlags.FOleAutomation)]
[ComConversionLoss]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IVSContainer
{
	[return: MarshalAs(UnmanagedType.Interface)]
	IVSApp GetApp();

	int GetScale();

	[return: ComAliasName("VSINTERFACESLib.wireHWND")]
	IntPtr GetWindow();

	[return: ComAliasName("VSINTERFACESLib.wireHWND")]
	IntPtr GetFrameWindow();

	Guid GetClassID();
}
