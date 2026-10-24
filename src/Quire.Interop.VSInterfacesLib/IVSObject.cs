#nullable disable
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quire.VSINTERFACESLib;

[global::Quire.Mirror.MirrorOf("VSINTERFACESLib.IVSObject, interop.vsinterfaceslib")]
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[ComConversionLoss]
[Guid("7BF3C614-0A1A-11E0-8B32-11F8E2A8C36C")]
[TypeLibType(TypeLibTypeFlags.FOleAutomation)]
public interface IVSObject
{
	void SetContainerSite([MarshalAs(UnmanagedType.Interface)] IVSContainer pVSContainer);

	[return: MarshalAs(UnmanagedType.Interface)]
	IVSContainer GetContainerSite();

	VSOBJMISC GetMiscStatus();

	[return: ComAliasName("VSINTERFACESLib.wireHICON")]
	IntPtr GetIcon();

	[return: MarshalAs(UnmanagedType.BStr)]
	string GetDefaultLabel();
}
