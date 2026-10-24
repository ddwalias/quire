#nullable disable
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quire.VSINTERFACESLib;

[global::Quire.Mirror.MirrorOf("VSINTERFACESLib.IVSApp, interop.vsinterfaceslib")]
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("7BF39F04-0A1A-11E0-9B4E-11F8E2A8C36C")]
[TypeLibType(TypeLibTypeFlags.FOleAutomation)]
public interface IVSApp
{
	VSAPPSTATE GetCurrentState();

	VSAPPOPTS GetOptions();

	void SetCurrentState(VSAPPSTATE vsState);

	void SetModified(bool bModified);

	[return: MarshalAs(UnmanagedType.BStr)]
	string GetRegistryKey();

	VSAPPLICENSE GetLicenseInfo();

	void LaunchHelp([MarshalAs(UnmanagedType.BStr)] string str);
}
