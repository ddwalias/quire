#nullable disable
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quire.VSINTERFACESLib;

[global::Quire.Mirror.MirrorOf("VSINTERFACESLib.IVSContextMenu, interop.vsinterfaceslib")]
[ComImport]
[Guid("7BF3ED27-0A1A-11E0-A1CB-11F8E2A8C36C")]
[TypeLibType(TypeLibTypeFlags.FOleAutomation)]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IVSContextMenu
{
	void QueryContextMenu([ComAliasName("VSINTERFACESLib.wireHMENU")] ref _RemotableHandle hMenu, uint indexMenu, uint idCmdFirst, uint idCmdLast, uint uFlags);

	void InvokeCommand(ref VSCMINFO lpcmi);

	void GetCommandString(uint idCmd, uint uFlags, ref uint reserved, [MarshalAs(UnmanagedType.LPWStr)] string pszName, uint cchMax);
}
