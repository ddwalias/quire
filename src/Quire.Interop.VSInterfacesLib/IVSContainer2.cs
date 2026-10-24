#nullable disable
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quire.VSINTERFACESLib;

[global::Quire.Mirror.MirrorOf("VSINTERFACESLib.IVSContainer2, interop.vsinterfaceslib")]
[ComImport]
[Guid("7BF3ED23-0A1A-11E0-A037-11F8E2A8C36C")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[TypeLibType(TypeLibTypeFlags.FOleAutomation)]
public interface IVSContainer2 : IVSContainer
{
	[return: MarshalAs(UnmanagedType.Interface)]
	new IVSApp GetApp();

	new int GetScale();

	[return: ComAliasName("VSINTERFACESLib.wireHWND")]
	new IntPtr GetWindow();

	[return: ComAliasName("VSINTERFACESLib.wireHWND")]
	new IntPtr GetFrameWindow();

	new Guid GetClassID();

	void GetVersion(ref uint lpVersion);
}
