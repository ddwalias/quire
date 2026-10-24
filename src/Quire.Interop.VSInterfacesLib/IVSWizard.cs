#nullable disable
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quire.VSINTERFACESLib;

[global::Quire.Mirror.MirrorOf("VSINTERFACESLib.IVSWizard, interop.vsinterfaceslib")]
[ComImport]
[TypeLibType(TypeLibTypeFlags.FOleAutomation)]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("7BF41433-0A1A-11E0-B101-11F8E2A8C36C")]
public interface IVSWizard
{
	void ChooseObject([ComAliasName("VSINTERFACESLib.wireHWND")] ref _RemotableHandle hwndParent, ref Guid lpclsid);

	void PreInitObject([MarshalAs(UnmanagedType.Interface)] IVSObject lpVSObject);

	void PostInitObject([MarshalAs(UnmanagedType.Interface)] IVSObject lpVSObject);
}
